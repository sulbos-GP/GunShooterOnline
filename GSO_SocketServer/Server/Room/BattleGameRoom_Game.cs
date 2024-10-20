using Google.Protobuf.Protocol;
using Server.Database.Handler;
using Server.Game;
using Server.Game.Object.Gear;
using Server.Game.Object.Item;
using Server.Server;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models.GameDB;

namespace Server
{
    public partial class BattleGameRoom
    {
        
        public void HandleMove(Player player, C_Move packet)
        {
            if (player == null)
                return;

            //검사--------------------

            //Console.WriteLine(player.info.Name + packet.PositionInfo.PosX + ", " + packet.PositionInfo.PosY);

            var movePosInfo = packet.PositionInfo; //C요청

            //player.info.PositionInfo.State = movePosInfo.State;
            player.info.PositionInfo.DirY = movePosInfo.DirY;
            player.info.PositionInfo.DirX = movePosInfo.DirX;
            player.info.PositionInfo.PosX = movePosInfo.PosX;
            player.info.PositionInfo.PosY = movePosInfo.PosY;
            player.info.PositionInfo.RotZ = movePosInfo.RotZ;


            //다른플레이어에게 알려줌
            var resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.info.ObjectId;
            resMovePacket.PositionInfo = packet.PositionInfo;

            player.CellPos = new Vector2(packet.PositionInfo.PosX, packet.PositionInfo.PosY);

            //TODO : 삭제
            map.ApplyMove(player,
                new Vector2Int((int)Math.Round(packet.PositionInfo.PosX), (int)Math.Round(packet.PositionInfo.PosY)));

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
            if(inventory == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            foreach(PS_ItemInfo item in inventory.storage.GetItems(player.Id))
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

            if(true == box.IsOpen())
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
        }

        internal void SearchItemHandler(Player player, int sourceObjectId, int sourceItemId)
        {
            S_SearchItem packet = new S_SearchItem();

            ItemObject sourcelItem = ObjectManager.Instance.Find<ItemObject>(sourceItemId);
            PS_ItemInfo sourceItemInfo = sourcelItem.ConvertItemInfo(player.Id);

            Storage sourceStorage = GetStorage(player, sourceObjectId);
            if (sourceStorage == null || sourcelItem == null || -1 == sourceStorage.ScanItem(sourcelItem))
            {
                packet.IsSuccess = false;
                packet.SourceObjectId = sourceObjectId;
                packet.SourceItem = sourceItemInfo;
                player.Session.Send(packet);
                return;
            }

            if(true == sourcelItem.IsViewer(player.Id))
            {
                packet.IsSuccess = false;
                packet.SourceObjectId = sourceObjectId;
                packet.SourceItem = sourceItemInfo;
                player.Session.Send(packet);
                return;
            }
            sourcelItem.AddViewer(player.Id, player.Session.mPeer.RoundTripTime);

            packet.IsSuccess = true;
            packet.SourceObjectId = sourceObjectId;
            packet.SourceItem = sourceItemInfo;
            player.Session.Send(packet);
        }

        internal async void MergeItemHandler(Player player, int sourceObjectId, int destinationObjectId, int mergedObjectId, int combinedObjectId, int mergeNumber)
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
            DB_ItemUnit oldMergedUnit = mergedlItem.Unit;
            PS_ItemInfo oldMergedItemInfo = mergedlItem.ConvertItemInfo(player.Id);

            ItemObject combinedItem = ObjectManager.Instance.Find<ItemObject>(combinedObjectId);
            Storage destinationStorage = GetStorageWithScanItem(player, destinationObjectId, combinedItem);
            if (sourceStorage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            DB_ItemUnit oldCombinedUnit = combinedItem.Unit;
            PS_ItemInfo oldCombinedInfo = combinedItem.ConvertItemInfo(player.Id);

            //머지하는 아이템 이름이 다를경우
            if (mergedlItem.ItemId != combinedItem.ItemId)
            {
                packet.IsSuccess = false;
                packet.MergedItem = oldMergedItemInfo;
                packet.CombinedItem = oldCombinedInfo;
                player.Session.Send(packet);
                return;
            }

            {

                if (false == destinationStorage.IsHaveAmount(combinedItem, mergeNumber))
                {
                    packet.IsSuccess = false;
                    packet.MergedItem = oldMergedItemInfo;
                    packet.CombinedItem = oldCombinedInfo;
                    player.Session.Send(packet);
                    return;
                }

                //인벤토리에 넣을 수 있는 최대 수량
                int maxAmount = 0;
                if (sourceObjectId == destinationObjectId)
                {
                    maxAmount = mergeNumber;
                }
                else
                {
                    maxAmount = sourceStorage.CheackMaxAmount(mergedlItem, mergeNumber);
                }

                ItemObject tempItem = new ItemObject(combinedItem);
                int lessAmount = destinationStorage.DecreaseAmount(combinedItem, maxAmount);
                int moreAmount = sourceStorage.IncreaseAmount(mergedlItem, maxAmount);

                if (lessAmount == -1 || moreAmount == -1)
                {
                    packet.IsSuccess = false;
                    packet.MergedItem = oldMergedItemInfo;
                    packet.CombinedItem = oldCombinedInfo;
                    player.Session.Send(packet);
                    return;
                }
                //Combined의 수량을 전부 소진한 경우
                if (lessAmount == 0)
                {
                    bool isDelete = destinationStorage.DeleteItem(combinedItem);
                    if (false == isDelete)
                    {
                        //CombinedItem의 수량 감소에 성공했을 테니까 기존에 정보로 되돌려준다
                        combinedItem.Amount = oldCombinedInfo.Amount;

                        packet.IsSuccess = false;
                        packet.MergedItem = oldMergedItemInfo;
                        packet.CombinedItem = oldCombinedInfo;
                        player.Session.Send(packet);
                        return;
                    }
                }

                //DB
                using (var database = DatabaseHandler.GameDB)
                {
                    using (var transaction = database.GetConnection().BeginTransaction())
                    {
                        try
                        {
                            if (IsInventory(sourceObjectId))
                            {
                                Inventory inventory = player.inventory;
                                await inventory.UpdateItemAttributes(mergedlItem, database, transaction);
                            }
                            else if (IsGear(sourceObjectId))
                            {
                                Gear gear = player.gear;
                                await gear.UpdateItemAttributes(mergedlItem, database, transaction);
                            }

                            if (IsInventory(destinationObjectId))
                            {
                                Inventory inventory = player.inventory;
                                if (combinedItem.Amount > 0)
                                {
                                    await inventory.UpdateItemAttributes(combinedItem, database, transaction);
                                }
                                else
                                {
                                    await inventory.DeleteItem(combinedItem, database, transaction);
                                }
                            }
                            else if (IsGear(destinationObjectId))
                            {
                                Gear gear = player.gear;
                                if (combinedItem.Amount > 0)
                                {
                                    await gear.UpdateItemAttributes(combinedItem, database, transaction);
                                }
                                else
                                {
                                    await gear.DeleteGear((EGearPart)destinationObjectId, combinedItem, database, transaction);
                                }
                            }

                            transaction.Commit();

                            packet.IsSuccess = true;
                            packet.MergedItem = mergedlItem.ConvertItemInfo(player.Id);
                            packet.CombinedItem = combinedItem.ConvertItemInfo(player.Id);
                            player.Session.Send(packet);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"[MergeItem] : {e.Message.ToString()}");
                            transaction.Rollback();

                            if (lessAmount == 0)
                            {
                                sourceStorage.InsertItem(tempItem);

                                packet.CombinedItem = tempItem.ConvertItemInfo(player.Id);
                            }
                            else
                            {
                                destinationStorage.DecreaseAmount(mergedlItem, maxAmount);
                                sourceStorage.IncreaseAmount(combinedItem, lessAmount);

                                packet.CombinedItem = combinedItem.ConvertItemInfo(player.Id);
                            }

                            packet.IsSuccess = false;
                            packet.MergedItem = mergedlItem.ConvertItemInfo(player.Id);
                            player.Session.Send(packet);
                        }
                    }
                }
            }
        }

        internal async void DevideItemHandler(Player player, int sourceObjectId, int destinationObjectId, int sourceItemId, int destinationGridX, int destinationGridY, int destinationRotation, int devideNumber)
        {

            S_DevideItem packet = new S_DevideItem();
            packet.SourceObjectId = sourceObjectId;
            packet.DestinationObjectId = destinationObjectId;

            ItemObject sourceItem = ObjectManager.Instance.Find<ItemObject>(sourceItemId);
            DB_ItemUnit oldSourceUnit = sourceItem.Unit;
            PS_ItemInfo oldSourceItemInfo = sourceItem.ConvertItemInfo(player.Id);

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
                ItemObject tempItem = new ItemObject(sourceItem);

                if (false == sourceStorage.IsHaveAmount(sourceItem, devideNumber))
                {
                    packet.IsSuccess = false;
                    packet.SourceItem = oldSourceItemInfo;
                    player.Session.Send(packet);
                    return;
                }

                DB_ItemUnit devideUnit = new DB_ItemUnit()
                {
                    attributes = new DB_UnitAttributes()
                    {
                        item_id = sourceItem.ItemId,
                        amount = devideNumber,
                        durability = 0,
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
                devideUnit.attributes.amount = devideNumber;

                ItemObject devideItem = new ItemObject(player.Id, devideUnit);

                //SourcelItem의 수량을 DevideNumber만큼 감소
                int lessAmount = sourceStorage.DecreaseAmount(sourceItem, devideNumber);

                //미리 나눠진 아이템의 공간이 확보되어 있는지 확인한다
                if (EStorageError.None != destinationStorage.InsertItem(devideItem))
                {

                    sourceStorage.IncreaseAmount(sourceItem, devideNumber);

                    packet.IsSuccess = false;
                    packet.SourceItem = oldSourceItemInfo;
                    player.Session.Send(packet);
                    return;
                }

                //SourcelItem 수량을 DevideNumber만큼 감소하였을때 음수가 나올 경우
                if (lessAmount == -1)
                {
                    packet.IsSuccess = false;
                    packet.SourceItem = oldSourceItemInfo;
                    player.Session.Send(packet);
                    return;
                }
                //SourcelItem의 수량을 전부 소진한 경우
                if (lessAmount == 0)
                {
                    bool isDelete = sourceStorage.DeleteItem(sourceItem);
                    if (false == isDelete)
                    {
                        //나눠진 아이템을 삭제한다
                        destinationStorage.DeleteItem(devideItem);

                        //CombinedItem의 수량 감소에 성공했을 테니까 기존에 정보로 되돌려준다
                        sourceItem.Amount = oldSourceItemInfo.Amount;

                        packet.IsSuccess = false;
                        packet.SourceItem = oldSourceItemInfo;
                        player.Session.Send(packet);
                        return;
                    }
                }

                using (var database = DatabaseHandler.GameDB)
                {
                    using (var transaction = database.GetConnection().BeginTransaction())
                    {
                        try
                        {
                            if (IsInventory(sourceObjectId))
                            {
                                Inventory inventory = player.inventory;
                                if (sourceItem.Amount > 0)
                                {
                                    await inventory.UpdateItemAttributes(sourceItem, database, transaction);
                                }
                                else
                                {
                                    await inventory.DeleteItem(sourceItem, database, transaction);
                                }
                            }
                            else if (IsGear(sourceObjectId))
                            {
                                Gear gear = player.gear;
                                if (sourceItem.Amount > 0)
                                {
                                    await gear.UpdateItemAttributes(sourceItem, database, transaction);
                                }
                                else
                                {
                                    await gear.DeleteGear((EGearPart)sourceObjectId, sourceItem, database, transaction);
                                }
                            }
                            
                            if (IsInventory(destinationObjectId))
                            {
                                Inventory inventory = player.inventory;
                                await inventory.InsertItem(devideItem, database, transaction);
                            }
                            else if (IsGear(destinationObjectId))
                            {
                                Gear gear = player.gear;
                                await gear.InsertGear((EGearPart)destinationObjectId, devideItem, database, transaction);
                            }

                            transaction.Commit();

                            packet.IsSuccess = true;
                            packet.SourceItem = sourceItem.ConvertItemInfo(player.Id);
                            packet.DestinationItem = devideItem.ConvertItemInfo(player.Id);
                            player.Session.Send(packet);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"[DevideItem] : {e.Message.ToString()}");
                            transaction.Rollback();

                            if (lessAmount == 0)
                            {
                                destinationStorage.DeleteItem(devideItem);
                                sourceStorage.InsertItem(tempItem);
                            }

                            packet.IsSuccess = false;
                            packet.SourceItem = oldSourceItemInfo;                          //클라이언트에 남아있는 기존 아이템
                            packet.DestinationItem = tempItem.ConvertItemInfo(player.Id);   //서버에서 다시 생성한 새로운 아이템
                            player.Session.Send(packet);
                        }
                    }
                }           
            }
        }

        internal async void MoveItemHandler(Player player, int sourceObjectId, int destinationObjectId, int sourceMoveItemId, int destinationGridX, int destinationGridY, int destinationRotation)
        {
            
            S_MoveItem packet = new S_MoveItem();
            packet.SourceObjectId = sourceObjectId;
            packet.DestinationObjectId = destinationObjectId;

            ItemObject sourceMovelItem = ObjectManager.Instance.Find<ItemObject>(sourceMoveItemId);
            Storage sourceStorage = GetStorageWithScanItem(player, sourceObjectId, sourceMovelItem);
            if(sourceStorage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            PS_ItemInfo oldSourceMoveItemInfo = sourceMovelItem.ConvertItemInfo(player.Id);

            Storage destinationStorage = GetStorage(player, destinationObjectId);
            if(sourceStorage == null)
            {
                packet.IsSuccess = false;
                packet.SourceMoveItem = oldSourceMoveItemInfo;
                packet.DestinationMoveItem = oldSourceMoveItemInfo;
                player.Session.Send(packet);
                return;
            }

            bool isDelete = sourceStorage.DeleteItem(sourceMovelItem);
            if(false == isDelete)
            {
                packet.IsSuccess = false;
                packet.SourceMoveItem = oldSourceMoveItemInfo;
                packet.DestinationMoveItem = oldSourceMoveItemInfo;
                player.Session.Send(packet);
            }

            DB_ItemUnit moveUnit = new DB_ItemUnit()
            {
                storage = new DB_StorageUnit()
                { 
                    grid_x = destinationGridX,
                    grid_y = destinationGridY,
                    rotation = destinationRotation,
                    unit_attributes_id = 0
                },
                attributes = sourceMovelItem.Attributes,
            };
            ItemObject moveItem = new ItemObject(player.Id, moveUnit);

            EStorageError isInsert = destinationStorage.InsertItem(moveItem);
            if (EStorageError.None != isInsert)
            {
                sourceStorage.InsertItem(sourceMovelItem);

                packet.IsSuccess = false;
                packet.SourceMoveItem = oldSourceMoveItemInfo;
                packet.DestinationMoveItem = sourceMovelItem.ConvertItemInfo(player.Id);
                player.Session.Send(packet);
            }

            using (var database = DatabaseHandler.GameDB)
            {
                using (var transaction = database.GetConnection().BeginTransaction())
                {
                    try
                    {
                        if (IsInventory(sourceObjectId))
                        {
                            Inventory inventory = player.inventory;
                            isDelete = await inventory.DeleteItem(sourceMovelItem, database, transaction);
                        }
                        else if (IsGear(sourceObjectId))
                        {
                            Gear gear = player.gear;
                            isDelete = await gear.DeleteGear((EGearPart)sourceObjectId, sourceMovelItem,database, transaction);
                        }

                        if (IsInventory(destinationObjectId))
                        {
                            Inventory inventory = player.inventory;
                            await inventory.InsertItem(moveItem, database, transaction);
                        }
                        else if (IsGear(destinationObjectId))
                        {
                            Gear gear = player.gear;
                            await gear.InsertGear((EGearPart)destinationObjectId, moveItem, database, transaction);
                        }

                        transaction.Commit();

                        packet.IsSuccess = true;
                        packet.SourceMoveItem = oldSourceMoveItemInfo;
                        packet.DestinationMoveItem = moveItem.ConvertItemInfo(player.Id);
                        player.Session.Send(packet);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[MoveItem] : {e.Message.ToString()}");
                        transaction.Rollback();

                        destinationStorage.DeleteItem(moveItem);
                        sourceStorage.InsertItem(sourceMovelItem);

                        packet.IsSuccess = false;
                        packet.SourceMoveItem = oldSourceMoveItemInfo;
                        packet.DestinationMoveItem = sourceMovelItem.ConvertItemInfo(player.Id);
                        player.Session.Send(packet);
                    }
                }
            }

        }

      /*  internal async Task<ItemObject> DeleteItemHandler(Player player, int sourceObjectId, int deleteItemId)
        {
            S_DeleteItem packet = new S_DeleteItem();

            ItemObject deleteItem = ObjectManager.Instance.Find<ItemObject>(deleteItemId);
            PS_ItemInfo deleteInfo = deleteItem.ConvertItemInfo(player.Id);

            Storage storage = GetStorageWithScanItem(player, sourceObjectId, deleteItem);
            if (storage == null)
            {
                packet.IsSuccess = false;
                packet.DeleteItem = deleteInfo;
                packet.SourceObjectId = sourceObjectId;
                player.Session.Send(packet);
                return null;
            }

            bool isDelete = storage.DeleteItem(deleteItem);
            if(false == isDelete)
            {
                packet.IsSuccess = false;
                packet.DeleteItem = deleteInfo;
                packet.SourceObjectId = sourceObjectId;
                player.Session.Send(packet);
                return null;
            }

            return deleteItem;

            using (var database = DatabaseHandler.GameDB)
            {
                try
                {

                    if (IsInventory(sourceObjectId))
                    {
                        Inventory inventory = player.inventory;
                        isDelete = await inventory.DeleteItem(deleteItem, database);
                    }
                    else if (IsGear(sourceObjectId))
                    {
                        Gear gear = player.gear;
                        isDelete = await gear.DeleteGear((EGearPart)sourceObjectId, deleteItem, database);
                    }

                    packet.IsSuccess = true;
                    packet.SourceObjectId = sourceObjectId;
                    packet.DeleteItem = deleteInfo;
                    player.Session.Send(packet);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[DeleteItem] : {e.Message.ToString()}");

                    packet.IsSuccess = false;
                    packet.DeleteItem = deleteInfo;
                    packet.SourceObjectId = sourceObjectId;
                    player.Session.Send(packet);
                }
            }

        }*/

        //메모리에서 아이템 지우기
        internal ItemObject FindAndDeleteItem(Player player, int sourceObjectId, int deleteItemId, out PS_ItemInfo deleteInfo)
        {
            ItemObject deleteItem = ObjectManager.Instance.Find<ItemObject>(deleteItemId);
            deleteInfo = deleteItem.ConvertItemInfo(player.Id);

            Storage storage = GetStorageWithScanItem(player, sourceObjectId, deleteItem);
            if (storage == null)
            {
                return null;
            }

            bool isDeleted = storage.DeleteItem(deleteItem);
            if (!isDeleted)
            {
                return null;
            }

            return deleteItem;
        }

        //패킷 전송 및 데이터베이스 작업
        internal async Task HandleDeleteItemResult(Player player, int sourceObjectId, ItemObject deleteItem, PS_ItemInfo deleteInfo)
        {
            S_DeleteItem packet = new S_DeleteItem();
            using (var database = DatabaseHandler.GameDB)
            {
                try
                {
                    bool isDeleted = false;

                    if (IsInventory(sourceObjectId))
                    {
                        Inventory inventory = player.inventory;
                        isDeleted = await inventory.DeleteItem(deleteItem, database);
                    }
                    else if (IsGear(sourceObjectId))
                    {
                        Gear gear = player.gear;
                        isDeleted = await gear.DeleteGear((EGearPart)sourceObjectId, deleteItem, database);
                    }

                    packet.IsSuccess = isDeleted;
                    packet.SourceObjectId = sourceObjectId;
                    packet.DeleteItem = deleteInfo;
                    player.Session.Send(packet);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[DeleteItem] : {e.Message}");

                    packet.IsSuccess = false;
                    packet.DeleteItem = deleteInfo;
                    packet.SourceObjectId = sourceObjectId;
                    player.Session.Send(packet);
                }
            }
        }

        internal async Task<ItemObject> DeleteItemHandler(Player player, int sourceObjectId, int deleteItemId)
        {
            PS_ItemInfo deleteInfo;
            ItemObject deleteItem = FindAndDeleteItem(player, sourceObjectId, deleteItemId, out deleteInfo);

            if (deleteItem == null)
            {
                // 삭제 실패 시 패킷 전송
                S_DeleteItem packet = new S_DeleteItem
                {
                    IsSuccess = false,
                    DeleteItem = deleteInfo,
                    SourceObjectId = sourceObjectId
                };
                player.Session.Send(packet);
                return null;
            }

            // 삭제 성공 시 데이터베이스 처리 및 결과 전송
            await HandleDeleteItemResult(player, sourceObjectId, deleteItem, deleteInfo);

            return deleteItem;
        }











        public bool IsInventory(int storageObjectId)
        {
            return storageObjectId == 0;
        }

        public bool IsGear(int storageObjectId)
        {
            return 0 < storageObjectId && storageObjectId <= 7;
        }

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
            else if(0 < storageObjectId && storageObjectId <= 7)
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
            if(storage == null)
            {
                return null;
            }

            if(scanItem == null)
            {
                return null;
            }

            if (-1 == storage.ScanItem(scanItem))
            {
                return null;
            }

            if(false == scanItem.IsViewer(player.Id))
            {
                return null;
            }

            return storage;
        }

        internal void ChangeAppearance(Player player,int targetId, int gunId)
        {
            player.gun.SetGunData(gunId);
            
            S_ChangeAppearance packet = new S_ChangeAppearance()
            {
                ObjectId = targetId,
                GunId = player.gun.UsingGunData.item_id
            };

            BroadCast(packet);
        }

        internal async void HandleInputData(Player player, bool isReload)
        {
            if (isReload)
            {
                await player.gun.Reload();
            }
        }

        ////////////////////////////////////////////
        //                                        //
        //               ?????????                //
        //                                        //
        ////////////////////////////////////////////

        internal void HandleRayCast(Player attacker, Vector2 pos, Vector2 dir)
        {
            if (attacker.gun.UsingGunState != GunState.Shootable) 
            {
                return;
            }
            attacker.gun.Fire(attacker,pos, dir);   
        }

        public void HandleExitGame(Player player, int exitId)
        {

            //오브젝트 매니저의 딕셔너리에서 플레이어의 인벤토리(그리드, 아이템)와 플레이어를 제거
            player.inventory.ClearInventory();
            ObjectManager.Instance.Remove(player.inventory.Id);

            ObjectManager.Instance.Remove(player.Id);

            S_ExitGame exitPacket = new S_ExitGame()
            {
                PlayerId = player.Id,
                ExitId = exitId
            };
            BroadCast(exitPacket);

            S_Despawn despawnPacket = new S_Despawn();
            despawnPacket.ObjcetIds.Add(player.Id);
            BroadCast(despawnPacket);


            //사람 전부 나가면 gameserver.Stop();
            //gameserver.Stop();

            //stop 부분에 모든 남아있는 플레이어 처리!!!!
        }



        List<Player> tempPlayer = new List<Player>();

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
#else
            if (tempPlayer.Count == 5) //접속할 인원에 따라 변경
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
                if(_playerDic.TryAdd(p.Id, p) == true)
                {

                }
                else
                {
                    Console.WriteLine("GameStart ERROR");
                }
            }


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

            foreach (var m in _monsterDic.Values)
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


        }

        
    }
}