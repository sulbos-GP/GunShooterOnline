using Google.Protobuf.Protocol;
using Server.Database.Handler;
using Server.Game;
using Server.Game.Object;
using Server.Game.Object.Gear;
using Server.Game.Object.Item;
using Server.Server;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;
using static Humanizer.In;

namespace Server
{
    public partial class BattleGameRoom
    {

        public void HandleMove(CreatureObj creature, PositionInfo movePosInfo)
        {
            if (creature == null || creature.IsDead == true)
                return;

            //검사--------------------

            //Console.WriteLine(player.info.Name + packet.PositionInfo.PosX + ", " + packet.PositionInfo.PosY);
            //Console.WriteLine($"{creature.info.Name} + {movePosInfo.PosX} , {movePosInfo.PosY}");

            //player.info.PositionInfo.State = movePosInfo.State;
            creature.info.PositionInfo.DirY = movePosInfo.DirY;
            creature.info.PositionInfo.DirX = movePosInfo.DirX;
            creature.info.PositionInfo.PosX = movePosInfo.PosX;
            creature.info.PositionInfo.PosY = movePosInfo.PosY;
            creature.info.PositionInfo.RotZ = movePosInfo.RotZ;


            //다른플레이어에게 알려줌
            var resMovePacket = new S_Move();
            resMovePacket.ObjectId = creature.info.ObjectId;
            resMovePacket.PositionInfo = movePosInfo;

            creature.CellPos = new Vector2(movePosInfo.PosX, movePosInfo.PosY);


            //TODO : 삭제
            map.ApplyMove(creature,
                new Vector2Int((int)Math.Round(movePosInfo.PosX), (int)Math.Round(movePosInfo.PosY)));
            BroadCast(resMovePacket);
        }

        ////////////////////////////////////////////
        //                                        //
        //               INVENTORY                //
        //                                        //
        ////////////////////////////////////////////

        internal void LoadInventoryHandler(Player player)
        {
            S_LoadInventory packet = new S_LoadInventory();

            Inventory inventory = player.inventory;
            if (inventory == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            foreach (PS_ItemInfo item in inventory.storage.GetItems(player.Id))
            {
                packet.ItemInfos.Add(item);
            }

            Gear gear = player.gear;
            if (inventory == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            foreach (PS_GearInfo item in gear.GetPartItems(player.Id))
            {
                packet.GearInfos.Add(item);
            }

            packet.IsSuccess = true;
            packet.SourceObjectId = 0;
            player.Session.Send(packet);
        }

        internal void LoadInventoryHandler(Player player, int sourceObjectId)
        {
            S_LoadInventory packet = new S_LoadInventory();

            BoxObject box = ObjectManager.Instance.Find<BoxObject>(sourceObjectId);
            if (box == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            if (true == box.IsOpen())
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            box.Open();

            foreach (PS_ItemInfo item in box.storage.GetItems(player.Id))
            {
                packet.ItemInfos.Add(item);
            }

            packet.IsSuccess = true;
            packet.SourceObjectId = sourceObjectId;
            player.Session.Send(packet);
        }

        internal void CloseInventoryHandler(Player player)
        {
            S_CloseInventory packet = new S_CloseInventory();

            packet.IsSuccess = true;
            packet.SourceObjectId = 0;
            player.Session.Send(packet);
            return;
        }

        internal void CloseInventoryHandler(Player player, int sourceObjectId)
        {

            S_CloseInventory packet = new S_CloseInventory();

            BoxObject box = ObjectManager.Instance.Find<BoxObject>(sourceObjectId);
            if (box == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            if (false == box.IsOpen())
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            box.Close();

            packet.IsSuccess = true;
            packet.SourceObjectId = sourceObjectId;
            player.Session.Send(packet);

            if (box.storage.ItemCount == 0)
            {
                BattleGameRoom room = player.gameRoom;
                if (room == null)
                {
                    return;
                }

                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjcetIds.Add(box.Id);
                room.BroadCast(despawnPacket);

                room.map.rootableObjects.Remove(box);

                ObjectManager.Instance.Remove(box.Id);
            }
        }

        internal void SearchItemHandler(Player player, int sourceObjectId, int sourceItemId)
        {
            S_SearchItem packet = new S_SearchItem();

            ItemObject sourcelItem = ObjectManager.Instance.Find<ItemObject>(sourceItemId);
            PS_ItemInfo sourceItemInfo = sourcelItem.ConvertItemInfo(player.Id);

            Storage sourceStorage = GetStorage(player, sourceObjectId);
            if (sourceStorage == null || sourcelItem == null || false == sourceStorage.ScanItem(sourcelItem))
            {
                packet.IsSuccess = false;
                packet.SourceObjectId = sourceObjectId;
                packet.SourceItem = sourceItemInfo;
                player.Session.Send(packet);
                return;
            }

            if (true == sourcelItem.IsViewer(player.Id))
            {
                packet.IsSuccess = false;
                packet.SourceObjectId = sourceObjectId;
                packet.SourceItem = sourceItemInfo;
                player.Session.Send(packet);
                return;
            }
            sourcelItem.AddViewer(player.Id, player.Session.mPeer.RoundTripTime);

            EventBus.Publish(EEventBusType.Collect, player, sourcelItem);

            packet.IsSuccess = true;
            packet.SourceObjectId = sourceObjectId;
            packet.SourceItem = sourceItemInfo;
            player.Session.Send(packet);
        }

        internal void MergeItemHandler(Player player, int sourceObjectId, int destinationObjectId, int mergedObjectId, int combinedObjectId, int mergeNumber)
        {
            S_MergeItem packet = new S_MergeItem();
            packet.SourceObjectId = sourceObjectId;
            packet.DestinationObjectId = destinationObjectId;

            ItemObject mergedlItem = ObjectManager.Instance.Find<ItemObject>(mergedObjectId);
            Storage sourceStorage = GetStorageWithScanItem(player, sourceObjectId, mergedlItem);
            if (sourceStorage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            PS_ItemInfo oldMergedItemInfo = mergedlItem.ConvertItemInfo(player.Id);
            packet.MergedItem = oldMergedItemInfo;

            ItemObject combinedItem = ObjectManager.Instance.Find<ItemObject>(combinedObjectId);
            Storage destinationStorage = GetStorageWithScanItem(player, destinationObjectId, combinedItem);
            if (sourceStorage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            PS_ItemInfo oldCombinedInfo = combinedItem.ConvertItemInfo(player.Id);
            packet.CombinedItem = oldCombinedInfo;

            {
                //머지하는 아이템 이름이 다를경우
                if (mergedlItem.ItemId != combinedItem.ItemId)
                {
                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }

                //남은 아이템 양이 음수일 경우
                int lessAmount = mergedlItem.Amount - mergeNumber;
                if (lessAmount < 0)
                {
                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }

                //합친 아이템이 최대 수량을 넘길 경우
                int combinedAmount = combinedItem.Amount + mergeNumber;
                if(combinedItem.Data.amount < combinedAmount)
                {
                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }

                //
                if(false == sourceStorage.DecreaseAmount(mergedlItem, mergeNumber))
                {
                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }

                //
                if(false == destinationStorage.IncreaseAmount(combinedItem, mergeNumber))
                {

                    sourceStorage.IncreaseAmount(mergedlItem, mergeNumber);

                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }

                //Combined의 수량을 전부 소진한 경우
                if (lessAmount == 0)
                {
                    bool isDelete = sourceStorage.DeleteItem(mergedlItem);
                    if (false == isDelete)
                    {

                        sourceStorage.IncreaseAmount(mergedlItem, mergeNumber);
                        destinationStorage.DecreaseAmount(combinedItem, mergeNumber);

                        packet.IsSuccess = false;
                        player.Session.Send(packet);
                        return;
                    }
                    else
                    {
                        ObjectManager.Instance.Remove(mergedlItem.Id);
                    }
                }

                packet.IsSuccess = true;
                packet.MergedItem = mergedlItem.ConvertItemInfo(player.Id);
                packet.CombinedItem = combinedItem.ConvertItemInfo(player.Id);
                player.Session.Send(packet);

                //DB
                //using (var database = DatabaseHandler.GameDB)
                //{
                //    using (var transaction = database.GetConnection().BeginTransaction())
                //    {
                //        try
                //        {
                //            if (IsInventory(sourceObjectId))
                //            {
                //                Inventory inventory = player.inventory;
                //                await inventory..ItemAttributes(mergedlItem, database, transaction);
                //            }
                //            else if (IsGear(sourceObjectId))
                //            {
                //                Gear gear = player.gear;
                //                await gear.UpdateItemAttributes(mergedlItem, database, transaction);
                //            }

                //            if (IsInventory(destinationObjectId))
                //            {
                //                Inventory inventory = player.inventory;
                //                if (combinedItem.Amount > 0)
                //                {
                //                    await inventory.UpdateItemAttributes(combinedItem, database, transaction);
                //                }
                //                else
                //                {
                //                    await inventory.DeleteItem(combinedItem, database, transaction);
                //                }
                //            }
                //            else if (IsGear(destinationObjectId))
                //            {
                //                Gear gear = player.gear;
                //                if (combinedItem.Amount > 0)
                //                {
                //                    await gear.UpdateItemAttributes(combinedItem, database, transaction);
                //                }
                //                else
                //                {
                //                    await gear.DeleteGear((EGearPart)destinationObjectId, combinedItem, database, transaction);
                //                }
                //            }

                //            transaction.Commit();

                //            packet.IsSuccess = true;
                //            packet.MergedItem = mergedlItem.ConvertItemInfo(player.Id);
                //            packet.CombinedItem = combinedItem.ConvertItemInfo(player.Id);
                //            player.Session.Send(packet);
                //        }
                //        catch (Exception e)
                //        {
                //            Console.WriteLine($"[MergeItem] : {e.Message.ToString()}");
                //            transaction.Rollback();

                //            if (lessAmount == 0)
                //            {
                //                sourceStorage.InsertItem(tempItem);

                //                packet.CombinedItem = tempItem.ConvertItemInfo(player.Id);
                //            }
                //            else
                //            {
                //                destinationStorage.DecreaseAmount(mergedlItem, maxAmount);
                //                sourceStorage.IncreaseAmount(combinedItem, lessAmount);

                //                packet.CombinedItem = combinedItem.ConvertItemInfo(player.Id);
                //            }

                //            packet.IsSuccess = false;
                //            packet.MergedItem = mergedlItem.ConvertItemInfo(player.Id);
                //            player.Session.Send(packet);
                //        }
                //    }
                //}
            }
        }

        internal void DevideItemHandler(Player player, int sourceObjectId, int destinationObjectId, int sourceItemId, int destinationGridX, int destinationGridY, int destinationRotation, int devideNumber)
        {

            S_DevideItem packet = new S_DevideItem();
            packet.SourceObjectId = sourceObjectId;
            packet.DestinationObjectId = destinationObjectId;

            ItemObject sourceItem = ObjectManager.Instance.Find<ItemObject>(sourceItemId);
            PS_ItemInfo oldSourceItemInfo = sourceItem.ConvertItemInfo(player.Id);
            packet.SourceItem = oldSourceItemInfo;

            Storage sourceStorage = GetStorageWithScanItem(player, sourceObjectId, sourceItem);
            if (sourceStorage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            Storage destinationStorage = GetStorage(player, destinationObjectId);
            if (sourceStorage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            {
                //남은 양이 작거나 모두 옮길 경우
                int lessAmount = sourceItem.Amount - devideNumber;
                if(lessAmount <= 0)
                {
                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }

                //SourcelItem의 수량을 DevideNumber만큼 감소
                if(false == sourceStorage.DecreaseAmount(sourceItem, devideNumber))
                {
                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }

                //나눈 아이템 임시 생성
                DB_ItemUnit devideUnit = new DB_ItemUnit()
                {
                    attributes = new DB_UnitAttributes()
                    {
                        item_id = sourceItem.ItemId,
                        amount = devideNumber,
                        durability = sourceItem.Durability,
                        unit_storage_id = null,
                    },

                    storage = new DB_StorageUnit()
                    {
                        grid_x = destinationGridX,
                        grid_y = destinationGridY,
                        rotation = destinationRotation,
                        unit_attributes_id = 0,
                    }
                };
                ItemObject devideItem = ObjectManager.Instance.Add<ItemObject>();
                devideItem.Init(player, devideUnit);

                //미리 나눠진 아이템의 공간이 확보되어 있는지 확인한다
                if (false == destinationStorage.InsertItem(devideItem))
                {

                    sourceStorage.IncreaseAmount(sourceItem, devideNumber);

                    ObjectManager.Instance.Remove(devideItem.Id);

                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }

                packet.IsSuccess = true;
                packet.SourceItem = sourceItem.ConvertItemInfo(player.Id);
                packet.DestinationItem = devideItem.ConvertItemInfo(player.Id);
                player.Session.Send(packet);

                //using (var database = DatabaseHandler.GameDB)
                //{
                //    using (var transaction = database.GetConnection().BeginTransaction())
                //    {
                //        try
                //        {
                //            if (IsInventory(sourceObjectId))
                //            {
                //                Inventory inventory = player.inventory;
                //                if (sourceItem.Amount > 0)
                //                {
                //                    await inventory.UpdateItemAttributes(sourceItem, database, transaction);
                //                }
                //                else
                //                {
                //                    await inventory.DeleteItem(sourceItem, database, transaction);
                //                }
                //            }
                //            else if (IsGear(sourceObjectId))
                //            {
                //                Gear gear = player.gear;
                //                if (sourceItem.Amount > 0)
                //                {
                //                    await gear.UpdateItemAttributes(sourceItem, database, transaction);
                //                }
                //                else
                //                {
                //                    await gear.DeleteGear((EGearPart)sourceObjectId, sourceItem, database, transaction);
                //                }
                //            }

                //            if (IsInventory(destinationObjectId))
                //            {
                //                Inventory inventory = player.inventory;
                //                await inventory.InsertItem(devideItem, database, transaction);
                //            }
                //            else if (IsGear(destinationObjectId))
                //            {
                //                Gear gear = player.gear;
                //                await gear.InsertGear((EGearPart)destinationObjectId, devideItem, database, transaction);
                //            }

                //            transaction.Commit();

                //            packet.IsSuccess = true;
                //            packet.SourceItem = sourceItem.ConvertItemInfo(player.Id);
                //            packet.DestinationItem = devideItem.ConvertItemInfo(player.Id);
                //            player.Session.Send(packet);
                //        }
                //        catch (Exception e)
                //        {
                //            Console.WriteLine($"[DevideItem] : {e.Message.ToString()}");
                //            transaction.Rollback();

                //            if (lessAmount == 0)
                //            {
                //                destinationStorage.DeleteItem(devideItem);
                //                sourceStorage.InsertItem(tempItem);
                //            }

                //            packet.IsSuccess = false;
                //            packet.SourceItem = oldSourceItemInfo;                          //클라이언트에 남아있는 기존 아이템
                //            packet.DestinationItem = tempItem.ConvertItemInfo(player.Id);   //서버에서 다시 생성한 새로운 아이템
                //            player.Session.Send(packet);
                //        }
                //    }
                //}
            }
        }

        internal void MoveItemHandler(Player player, int sourceObjectId, int destinationObjectId, int sourceMoveItemId, int destinationGridX, int destinationGridY, int destinationRotation)
        {

            S_MoveItem packet = new S_MoveItem();
            packet.SourceObjectId = sourceObjectId;
            packet.DestinationObjectId = destinationObjectId;

            ItemObject sourceMovelItem = ObjectManager.Instance.Find<ItemObject>(sourceMoveItemId);
            Storage sourceStorage = GetStorageWithScanItem(player, sourceObjectId, sourceMovelItem);
            if (sourceStorage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            PS_ItemInfo oldSourceMoveItemInfo = sourceMovelItem.ConvertItemInfo(player.Id);

            Storage destinationStorage = GetStorage(player, destinationObjectId);
            if (sourceStorage == null)
            {
                packet.IsSuccess = false;
                packet.SourceMoveItem = oldSourceMoveItemInfo;
                packet.DestinationMoveItem = oldSourceMoveItemInfo;
                player.Session.Send(packet);
                return;
            }

            bool isDelete = sourceStorage.DeleteItem(sourceMovelItem);
            if (false == isDelete)
            {
                packet.IsSuccess = false;
                packet.SourceMoveItem = oldSourceMoveItemInfo;
                packet.DestinationMoveItem = oldSourceMoveItemInfo;
                player.Session.Send(packet);
                return;
            }

            sourceMovelItem.X = destinationGridX;
            sourceMovelItem.Y = destinationGridY;
            sourceMovelItem.Rotate = destinationRotation;

            if (destinationObjectId == (int)EGearPart.Backpack)
            {
                //가방을 장착 하였을 경우
                List<ItemObject> items = player.inventory.storage.Items;

                player.gear.GetPart(EGearPart.Backpack).ClearStorage();
                destinationStorage.InsertItem(sourceMovelItem);

                if (false == player.inventory.MakeInventory())
                {
                    sourceStorage.InsertItem(sourceMovelItem);

                    player.gear.GetPart(EGearPart.Backpack).ClearStorage();
                    player.gear.CreateDefaultBackpack();
                    player.inventory.InitInventory();
                    foreach (ItemObject item in items)
                    {
                        player.inventory.storage.InsertItem(item);
                    }

                    packet.IsSuccess = false;
                    packet.SourceMoveItem = oldSourceMoveItemInfo;
                    packet.DestinationMoveItem = oldSourceMoveItemInfo;
                    player.Session.Send(packet);
                    return;
                }
            }
            else
            {
                bool isPlacement = destinationStorage.InsertItem(sourceMovelItem);
                if (false == isPlacement)
                {
                    sourceStorage.InsertItem(sourceMovelItem);

                    packet.IsSuccess = false;
                    packet.SourceMoveItem = oldSourceMoveItemInfo;
                    packet.DestinationMoveItem = oldSourceMoveItemInfo;
                    player.Session.Send(packet);
                    return;
                }
            }

            
            if (sourceObjectId == (int)EGearPart.Backpack)
            {
                //가방을 이동한 경우
                List<ItemObject> items = player.inventory.storage.Items;

                player.gear.CreateDefaultBackpack();

                if (false == player.inventory.MakeInventory())
                {
                    player.gear.GetPart(EGearPart.Backpack).ClearStorage();

                    sourceStorage.InsertItem(sourceMovelItem);
                    player.inventory.InitInventory();
                    foreach (ItemObject item in items)
                    {
                        player.inventory.storage.InsertItem(item);
                    }

                    destinationStorage.DeleteItem(sourceMovelItem);

                    packet.IsSuccess = false;
                    packet.SourceMoveItem = oldSourceMoveItemInfo;
                    packet.DestinationMoveItem = oldSourceMoveItemInfo;
                    player.Session.Send(packet);
                    return;
                }
            }
            


            packet.IsSuccess = true;
            packet.SourceMoveItem = oldSourceMoveItemInfo;
            packet.DestinationMoveItem = sourceMovelItem.ConvertItemInfo(player.Id);
            player.Session.Send(packet);

            //using (var database = DatabaseHandler.GameDB)
            //{
            //    using (var transaction = database.GetConnection().BeginTransaction())
            //    {
            //        try
            //        {
            //            if (IsInventory(sourceObjectId))
            //            {
            //                Inventory inventory = player.inventory;
            //                isDelete = await inventory.DeleteItem(sourceMovelItem, database, transaction);
            //            }
            //            else if (IsGear(sourceObjectId))
            //            {
            //                Gear gear = player.gear;
            //                isDelete = await gear.DeleteGear((EGearPart)sourceObjectId, sourceMovelItem, database, transaction);
            //            }

            //            if (IsInventory(destinationObjectId))
            //            {
            //                Inventory inventory = player.inventory;
            //                await inventory.InsertItem(moveItem, database, transaction);
            //            }
            //            else if (IsGear(destinationObjectId))
            //            {
            //                Gear gear = player.gear;
            //                await gear.InsertGear((EGearPart)destinationObjectId, moveItem, database, transaction);
            //            }

            //            transaction.Commit();

            //            packet.IsSuccess = true;
            //            packet.SourceMoveItem = oldSourceMoveItemInfo;
            //            packet.DestinationMoveItem = moveItem.ConvertItemInfo(player.Id);
            //            player.Session.Send(packet);
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine($"[MoveItem] : {e.Message.ToString()}");
            //            transaction.Rollback();

            //            destinationStorage.DeleteItem(moveItem);
            //            sourceStorage.InsertItem(sourceMovelItem);

            //            packet.IsSuccess = false;
            //            packet.SourceMoveItem = oldSourceMoveItemInfo;
            //            packet.DestinationMoveItem = sourceMovelItem.ConvertItemInfo(player.Id);
            //            player.Session.Send(packet);
            //        }
            //    }
            //}

        }

        internal void DeleteItemHandler(Player player, int sourceObjectId, int deleteItemId)
        {
            S_DeleteItem packet = new S_DeleteItem();

            ItemObject deleteItem = ObjectManager.Instance.Find<ItemObject>(deleteItemId);
            PS_ItemInfo deleteInfo = deleteItem.ConvertItemInfo(player.Id);

            packet.DeleteItem = deleteInfo;
            packet.SourceObjectId = sourceObjectId;

            Storage storage = GetStorageWithScanItem(player, sourceObjectId, deleteItem);
            if (storage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            bool isDelete = storage.DeleteItem(deleteItem);
            if (false == isDelete)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            //가방을 삭제 했을 경우
            if (sourceObjectId == (int)EGearPart.Backpack)
            {
                List<ItemObject> items = player.inventory.storage.Items;

                player.gear.CreateDefaultBackpack();
                
                if(false == player.inventory.MakeInventory())
                {
                    player.gear.GetPart(EGearPart.Backpack).ClearStorage();

                    storage.InsertItem(deleteItem);
                    player.inventory.InitInventory();
                    foreach (ItemObject item in items)
                    {
                        player.inventory.storage.InsertItem(item);
                    }

                    packet.IsSuccess = false;
                    player.Session.Send(packet);
                    return;
                }
            }

            BattleGameRoom room = player.gameRoom;
            if(room != null)
            {
                BoxObject boxObject = ObjectManager.Instance.Add<BoxObject>();
                boxObject.CellPos = player.CellPos;
                boxObject.SetItemObject(deleteItem);
                room.map.rootableObjects.Add(boxObject);

                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(boxObject.info);
                room.BroadCast(spawnPacket);
            }

            packet.IsSuccess = true;
            packet.SourceObjectId = sourceObjectId;
            packet.DeleteItem = deleteInfo;
            player.Session.Send(packet);
        }

        //메모리에서 아이템 지우기
        //internal ItemObject FindAndDeleteItem(Player player, int sourceObjectId, int deleteItemId, out PS_ItemInfo deleteInfo)
        //{
        //    ItemObject deleteItem = ObjectManager.Instance.Find<ItemObject>(deleteItemId);
        //    deleteInfo = deleteItem.ConvertItemInfo(player.Id);

        //    Storage storage = GetStorageWithScanItem(player, sourceObjectId, deleteItem);
        //    if (storage == null)
        //    {
        //        return null;
        //    }

        //    bool isDeleted = storage.DeleteItem(deleteItem);
        //    if (!isDeleted)
        //    {
        //        return null;
        //    }

        //    return deleteItem;
        //}

        ////패킷 전송 및 데이터베이스 작업
        //internal async Task HandleDeleteItemResult(Player player, int sourceObjectId, ItemObject deleteItem, PS_ItemInfo deleteInfo)
        //{
        //    S_DeleteItem packet = new S_DeleteItem();
        //    using (var database = DatabaseHandler.GameDB)
        //    {
        //        try
        //        {
        //            bool isDeleted = false;

        //            if (IsInventory(sourceObjectId))
        //            {
        //                Inventory inventory = player.inventory;
        //                isDeleted = await inventory.DeleteItem(deleteItem, database);
        //            }
        //            else if (IsGear(sourceObjectId))
        //            {
        //                Gear gear = player.gear;
        //                isDeleted = await gear.DeleteGear((EGearPart)sourceObjectId, deleteItem, database);
        //            }

        //            packet.IsSuccess = isDeleted;
        //            packet.SourceObjectId = sourceObjectId;
        //            packet.DeleteItem = deleteInfo;
        //            player.Session.Send(packet);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine($"[DeleteItem] : {e.Message}");

        //            packet.IsSuccess = false;
        //            packet.DeleteItem = deleteInfo;
        //            packet.SourceObjectId = sourceObjectId;
        //            player.Session.Send(packet);
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="sourceObjectId">Player is 0</param>
        /// <param name="deleteItemId"></param>
        //internal async void DeleteItemHandler(Player player, int sourceObjectId, int deleteItemId)
        //{
        //    PS_ItemInfo deleteInfo;
        //    ItemObject deleteItem = FindAndDeleteItem(player, sourceObjectId, deleteItemId, out deleteInfo);

        //    if (deleteItem == null)
        //    {
        //        // 삭제 실패 시 패킷 전송
        //        S_DeleteItem packet = new S_DeleteItem
        //        {
        //            IsSuccess = false,
        //            DeleteItem = deleteInfo,
        //            SourceObjectId = sourceObjectId
        //        };
        //        player.Session.Send(packet);
        //    }

        //    // 삭제 성공 시 데이터베이스 처리 및 결과 전송
        //    await HandleDeleteItemResult(player, sourceObjectId, deleteItem, deleteInfo);

        //}

        public Storage GetStorage(Player player, int storageObjectId)
        {
            Storage storage = new Storage();
            if (0 == storageObjectId)
            {
                Inventory inventory = player.inventory;
                if (inventory == null)
                {
                    return null;
                }

                return inventory.storage;
            }
            else if (0 < storageObjectId && storageObjectId <= 7)
            {
                Gear gear = player.gear;
                if (gear == null)
                {
                    return null;
                }

                return gear.GetPart((EGearPart)storageObjectId);
            }
            else
            {
                BoxObject box = ObjectManager.Instance.Find<BoxObject>(storageObjectId);
                if (box == null)
                {
                    return null;
                }

                return box.storage;
            }
        }

        public Storage GetStorageWithScanItem(Player player, int storageObjectId, ItemObject scanItem)
        {
            Storage storage = GetStorage(player, storageObjectId);
            if (storage == null)
            {
                return null;
            }

            if (scanItem == null)
            {
                return null;
            }

            if (false == storage.ScanItem(scanItem))
            {
                return null;
            }

            if (false == scanItem.IsViewer(player.Id))
            {
                return null;
            }

            return storage;
        }

        internal void ChangeAppearance(Player player, int targetId, PS_GearInfo info)
        {
            S_ChangeAppearance packet = new S_ChangeAppearance();
            packet.ObjectId = targetId;
            if (info.Item.ObjectId == 0) //총을 들고 있지 않음
            {
                player.weapon.ResetGun();
                //packet.GunId = 0;
                packet.GunType = null;
            }
            else
            {
                player.weapon.SetGunData(info); //TODO : 0 or 1
                //packet.GunId = player.weapon.GetCurrentWeapon().GunData.item_id;
                packet.GunType = new PS_GearInfo()
                {
                    Item = player.weapon.GetCurrentWeapon().gunItemData.ConvertItemInfo(player.Id),
                    Part = player.weapon.GetCurrentWeaponGearPart()
                };
            }

            BroadCast(packet);
        }

        internal void HandleInputData(Player player, C_InputData packet)
        {
            if (packet.Reload)
            {
                player.weapon.Reload();
            }

            if (packet.ItemId != 0)
            {
                //성훈
                player.UseQuickSlot(packet.ItemId, packet.ItemSoltId);

            }

        }

        ////////////////////////////////////////////
        //                                        //
        //               ?????????                //
        //                                        //
        ////////////////////////////////////////////

        internal void HandleRayCast(Player attacker, Vector2 pos, Vector2 dir)
        {

            attacker.weapon.Fire(attacker, pos, dir);
        }

        public void HandleExitGame(Player player, int exitId)
        {
            ExitZone exitZone = ObjectManager.Instance.Find<ExitZone>(exitId);
            exitZone.OnEnterExitZone(player);
        }


        public void HandleForceExit(Player exitPlayer) //강제 종료
        {
        
            
            /*  if (exitPlayer.gameRoom.MatchInfo.TryGetValue(exitPlayer.UID, out MatchOutcome outcome) == true)
            {
                outcome.escape += 1;
            }*/

            //Play관련 이벤트 버스
            EventBus.Publish(EEventBusType.Play, exitPlayer, "PLAY_OUT");

            //웹에 플레이어 메타데이터 보내기
            exitPlayer.gameRoom.PostPlayerStats(exitPlayer.Id);

            //오브젝트 매니저의 딕셔너리에서 플레이어의 인벤토리(그리드, 아이템)와 플레이어를 제거
            ObjectManager.Instance.Remove(exitPlayer.inventory.Id);

            LeaveGame(exitPlayer.Id);

            //TODO 승현 : Death 처리?
            S_ExitGame exitPacket = new S_ExitGame()
            {
                IsSuccess = true,
                PlayerId = exitPlayer.Id,
                //ExitId = this.Id
            };
            exitPlayer.gameRoom.BroadCast(exitPacket);

            S_Despawn despawnPacket = new S_Despawn();
            despawnPacket.ObjcetIds.Add(exitPlayer.Id);
            exitPlayer.gameRoom.BroadCast(despawnPacket);
        }





        List<Player> tempPlayer;

        public void HandleClientLoadGame(Player player)
        {

            //로드 끝났어
            if (tempPlayer.Contains(player) == true)
            {
                Console.WriteLine("Error tempPlayer conation player ");
            }
            else
            {
                tempPlayer.Add(player);
            }

#if DOCKER
            if (tempPlayer.Count == connectPlayer.Count) //connectPlayer.Count
            {
                //전부 모임
                Console.WriteLine($"connectPlayer.Count : {connectPlayer.Count}, " +
                    $"tempPlayer.Count : {tempPlayer.Count}");
                Console.WriteLine("All Player is Ready to Start Game");

                GameStart();
            }
            else
            {
                Console.WriteLine("All Player is not Ready !!");
                //아직 다 못모임
                S_WaitingStatus status = new S_WaitingStatus()
                {
                    CurrentPlayers = tempPlayer.Count,
                    RequiredPlayers = connectPlayer.Count
                };


                foreach (var t in tempPlayer)
                {
                    t.Session.Send(status);
                }
            }
#elif RELEASE
            if (tempPlayer.Count == 2)
            {
                Console.WriteLine("connectPlayer.Count  is zero. -> only use Debug ");
                GameStart();

            }
#else
            if (tempPlayer.Count == 1) // 초코파이 접속할 인원에 따라 변경
            {
                Console.WriteLine("connectPlayer.Count  is zero. -> only use Debug ");
                GameStart();

            }
#endif





        }


        private void GameStart()
        {
            Console.WriteLine("============ GameStart ============");
            map.SpawnPlayers(tempPlayer.ToArray());

            foreach (Player p in tempPlayer)
            {
                if (_playerDic.TryAdd(p.Id, p) == true)
                {

                }
                else
                {
                    Console.WriteLine("GameStart ERROR");
                }
            }

            int count = 2;
            foreach (AISpawnZone zone in map.aispawnZones)
            {
                if(count-- <= 0)
                      continue;


                if(count == 1)
                {
                    RangeEnemy enemy = ObjectManager.Instance.Add<RangeEnemy>();
                    {
                        //enemy.info.Name = "AI";
                        enemy.CellPos = zone.CellPos;
                        enemy.gameRoom = Program.gameserver.gameRoom as BattleGameRoom;

                    }
                    enemy.Init(zone.CellPos);
                    EnterGame(enemy);

                }
                else
                {
                    MeleeEnemy enemy = ObjectManager.Instance.Add<MeleeEnemy>();
                    {
                        //enemy.info.Name = "AI";
                        enemy.CellPos = zone.CellPos;
                        enemy.gameRoom = Program.gameserver.gameRoom as BattleGameRoom;

                    }
                    enemy.Init(zone.CellPos);
                    EnterGame(enemy);
                }
            
            
            
            }



            /*  EnemyAI enemy = ObjectManager.Instance.Add<EnemyAI>();
              {
                  //enemy.info.Name = "AI";
                  enemy.CellPos = map.aispawnZones[0].CellPos;
                  enemy.gameRoom = Program.gameserver.gameRoom as BattleGameRoom;

              }
              enemy.Init(map.aispawnZones[0].CellPos);
              EnterGame(enemy);*/

            foreach (Player p in tempPlayer)
            {

                EnterGame(p);
            }


           
            S_GameStart s_GameStart = new S_GameStart()
            {
                RoomId = this.RoomId,
                StartTime = System.Environment.TickCount,

            };

            foreach (var p in _playerDic.Values)
            {
                s_GameStart.Objects.Add(p.info);

                CreatureObj creatureObj = p as CreatureObj;
                if (creatureObj != null)
                {
                    var ChangePacket = new S_ChangeHp();
                    ChangePacket.ObjectId = creatureObj.Id;
                    ChangePacket.Hp = creatureObj.Hp;
                    BroadCast(ChangePacket);
                }

            }

            foreach (var m in _enemyDic.Values)
            {
                s_GameStart.Objects.Add(m.info);


                CreatureObj creatureObj = m as CreatureObj;
                if (creatureObj != null)
                {
                    var ChangePacket = new S_ChangeHp();
                    ChangePacket.ObjectId = creatureObj.Id;
                    ChangePacket.Hp = creatureObj.Hp;
                    BroadCast(ChangePacket);
                }
            }

            foreach (var s in _skillObjDic.Values)
            {
                s_GameStart.Objects.Add(s.info);
            }

            BroadCast(s_GameStart);

            foreach (var p in _playerDic.Values)
            {
                EventBus.Publish(EEventBusType.Play, p, "PLAY_IN");
            }

            IsGameStarted = true;





        }

        public void HandleJoin(CredentiaInfo credentiaInfo, Player player)
        {
            Console.WriteLine("HandleJoin : S_JoinServer");
            //s 와 인증 확인

            S_JoinServer joinServer = new S_JoinServer()
            {
                Connected = true,
            };

            player.Session.Send(joinServer);

            //PushAfter(100 ,CheakPing(player.Session));
             PushAfter(400 ,CheakPing, player.Session);
             //CheakPing( player.Session);
        }

      


    }
}