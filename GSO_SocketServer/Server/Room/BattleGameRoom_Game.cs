using Google.Protobuf.Protocol;
using LiteNetLib;
using Newtonsoft.Json.Bson;
using Server.Database.Data;
using Server.Game;
using Server.Game.Object.Item;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server
{
    public partial class BattleGameRoom
    {
        public List<object> Escapes = new List<object>();

        public void HandleMove(Player player, C_Move packet)
        {
            if (player == null)
                return;

            //검사--------------------

            //Console.WriteLine("HandleMove" + packet.PositionInfo.PosX + ", " + packet.PositionInfo.PosY);

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
            DB_StorageUnit oldMergedUnit = mergedlItem.ConvertInventoryUnit();
            PS_ItemInfo oldMergedItemInfo = mergedlItem.ConvertItemInfo(player.Id);

            ItemObject combinedItem = ObjectManager.Instance.Find<ItemObject>(combinedObjectId);
            Storage destinationStorage = GetStorageWithScanItem(player, destinationObjectId, combinedItem);
            if (sourceStorage == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            DB_StorageUnit oldCombinedUnit = combinedItem.ConvertInventoryUnit();
            PS_ItemInfo oldCombinedInfo = combinedItem.ConvertItemInfo(player.Id);

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
                int maxAmount = destinationStorage.CheackMaxAmount(combinedItem, mergeNumber);

                ItemObject tempItem = new ItemObject(combinedItem);
                //CombinedItem의 MergeNumber만큼 감소
                int lessAmount = destinationStorage.DecreaseAmount(combinedItem, maxAmount);

                //MergedItem에 수량 증가
                int moreAmount = sourceStorage.IncreaseAmount(mergedlItem, maxAmount);

                //CombinedItem의 수량을 MergeNumber만큼 감소하였을때 음수가 나올 경우
                //MergeItem의 수량을 MergeNumber만큼 증가하였을때 Limit을 넘은 경우
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

                bool isMerge = false;
                if (IsInventory(sourceObjectId) && IsInventory(destinationObjectId))
                {
                    Inventory inventory = player.inventory;
                    isMerge = await inventory.MergeItem(oldMergedUnit, mergedlItem.ConvertInventoryUnit(), oldCombinedUnit, combinedItem.ConvertInventoryUnit());
                }
                else if (IsInventory(sourceObjectId))
                {
                    Inventory inventory = player.inventory;
                    isMerge = await inventory.MergeItem(oldMergedUnit, mergedlItem.ConvertInventoryUnit());
                }
                else if (IsInventory(destinationObjectId))
                {
                    Inventory inventory = player.inventory;
                    isMerge = await inventory.MergeItem(oldCombinedUnit, combinedItem.ConvertInventoryUnit());
                }
                else
                {
                    isMerge = true;
                }

                if (false == isMerge)
                {
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
                    return;
                }
                else
                {
                    packet.IsSuccess = true;
                    packet.MergedItem = mergedlItem.ConvertItemInfo(player.Id);
                    packet.CombinedItem = combinedItem.ConvertItemInfo(player.Id);
                    player.Session.Send(packet);
                    return;
                }
            }
        }

        internal async void DevideItemHandler(Player player, int sourceObjectId, int destinationObjectId, int sourceItemId, int destinationGridX, int destinationGridY, int destinationRotation, int devideNumber)
        {

            S_DevideItem packet = new S_DevideItem();
            packet.SourceObjectId = sourceObjectId;
            packet.DestinationObjectId = destinationObjectId;

            ItemObject sourceItem = ObjectManager.Instance.Find<ItemObject>(sourceItemId);
            DB_StorageUnit oldSourceUnit = sourceItem.ConvertInventoryUnit();
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

                if (false == destinationStorage.IsHaveAmount(sourceItem, devideNumber))
                {
                    packet.IsSuccess = false;
                    packet.SourceItem = oldSourceItemInfo;
                    player.Session.Send(packet);
                    return;
                }

                DB_UnitAttributes devideAttributes = sourceItem.Attributes;
                devideAttributes.amount = devideNumber;

                ItemObject devideItem = new ItemObject(player.Id, destinationGridX, destinationGridY, destinationRotation, devideAttributes);
                DB_StorageUnit oldDevideUnit = devideItem.ConvertInventoryUnit();
                PS_ItemInfo oldDevideItemInfo = devideItem.ConvertItemInfo(player.Id);

                //미리 나눠진 아이템의 공간이 확보되어 있는지 확인한다
                if (false == destinationStorage.InsertItem(devideItem))
                {
                    packet.IsSuccess = false;
                    packet.SourceItem = oldSourceItemInfo;
                    player.Session.Send(packet);
                    return;
                }

                //SourcelItem의 수량을 DevideNumber만큼 감소
                int lessAmount = sourceStorage.DecreaseAmount(sourceItem, devideNumber);

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

                bool isDevide = false;
                if (IsInventory(sourceObjectId) && IsInventory(destinationObjectId))
                {
                    Inventory inventory = player.inventory;
                    isDevide = await inventory.DevideItem(oldSourceUnit, sourceItem.ConvertInventoryUnit(), devideItem.ConvertInventoryUnit());
                }
                else if (IsInventory(sourceObjectId))
                {
                    Inventory inventory = player.inventory;
                    isDevide = await inventory.DevideItem(true, oldSourceUnit, sourceItem.ConvertInventoryUnit());
                }
                else if (IsInventory(destinationObjectId))
                {
                    Inventory inventory = player.inventory;
                    isDevide = await inventory.DevideItem(false, oldDevideUnit, devideItem.ConvertInventoryUnit());
                }
                else
                {
                    isDevide = true;
                }

                if (false == isDevide)
                {
                    if (lessAmount == 0)
                    {
                        destinationStorage.DeleteItem(devideItem);
                        sourceStorage.InsertItem(tempItem);
                    }

                    packet.IsSuccess = false;
                    packet.SourceItem = oldSourceItemInfo;                          //클라이언트에 남아있는 기존 아이템
                    packet.DestinationItem = tempItem.ConvertItemInfo(player.Id);   //서버에서 다시 생성한 새로운 아이템
                    player.Session.Send(packet);
                    return;
                }
                else
                {
                    packet.IsSuccess = true;
                    packet.SourceItem = sourceItem.ConvertItemInfo(player.Id);
                    packet.DestinationItem = devideItem.ConvertItemInfo(player.Id);
                    player.Session.Send(packet);
                    return;
                }
            
            }
        }

        internal async void MoveItemHandler(Player player, int sourceObjectId, int destinationObjectId, int sourceMoveItemId, int destinationGridX, int destinationGridY, int destinationRotation)
        {
            
            S_MoveItem packet = new S_MoveItem();

            ItemObject sourceMovelItem = ObjectManager.Instance.Find<ItemObject>(sourceMoveItemId);
            PS_ItemInfo sourceMoveItemInfo = sourceMovelItem.ConvertItemInfo(player.Id);

            Storage sourceStorage = GetStorageWithScanItem(player, sourceObjectId, sourceMovelItem);
            if(sourceStorage == null)
            {
                packet.IsSuccess = false;
                packet.SourceMoveItem = sourceMoveItemInfo;
                player.Session.Send(packet);
                return;
            }

            Storage destinationStorage = GetStorage(player, destinationObjectId);
            if(sourceStorage == null)
            {
                packet.IsSuccess = false;
                packet.SourceMoveItem = sourceMoveItemInfo;
                player.Session.Send(packet);
                return;
            }

            ItemObject tempItemObject = new ItemObject(sourceMovelItem);
            bool isDelete = false;
            {
                if (IsInventory(sourceObjectId))
                {
                    Inventory inventory = player.inventory;
                    isDelete = await inventory.DeleteItem(sourceMovelItem);
                }
                else
                {
                    isDelete = sourceStorage.DeleteItem(sourceMovelItem);
                }
            }

            ItemObject newItem = new ItemObject(player.Id, destinationGridX, destinationGridY, destinationRotation, sourceMovelItem.Attributes);
            bool isInsert = false;
            {
                if (IsInventory(destinationObjectId))
                {
                    Inventory inventory = player.inventory;
                    isInsert = await inventory.InsertItem(newItem);
                }
                else
                {
                    isInsert = destinationStorage.InsertItem(newItem);
                }

                if(false == isInsert)
                {
                    if (IsInventory(sourceObjectId))
                    {
                        Inventory inventory = player.inventory;
                        await inventory.InsertItem(tempItemObject);
                    }
                    else
                    {
                        sourceStorage.InsertItem(tempItemObject);
                    }
                }

            }

            if (isInsert == false || isDelete == false)
            {
                packet.IsSuccess = false;
                packet.SourceObjectId = sourceObjectId;
                packet.SourceMoveItem = sourceMoveItemInfo;
                packet.DestinationMoveItem = tempItemObject.ConvertItemInfo(player.Id);
                player.Session.Send(packet);
                return;
            }

            packet.IsSuccess = true;
            packet.SourceObjectId = sourceObjectId;
            packet.DestinationObjectId = destinationObjectId;
            packet.SourceMoveItem = sourceMoveItemInfo;
            packet.DestinationMoveItem = newItem.ConvertItemInfo(player.Id);
            player.Session.Send(packet);
        }

        internal async void DeleteItemHandler(Player player, int sourceObjectId, int deleteItemId)
        {
            S_DeleteItem packet = new S_DeleteItem();

            ItemObject deleteItem = ObjectManager.Instance.Find<ItemObject>(deleteItemId);
            PS_ItemInfo deleteInfo = deleteItem.ConvertItemInfo(player.Id);

            Storage storage = GetStorageWithScanItem(player, sourceObjectId, deleteItem);
            if (storage == null)
            {
                packet.IsSuccess = false;
                packet.DeleteItem = deleteInfo;
                player.Session.Send(packet);
                return;
            }

            bool isDelete = false;
            if (IsInventory(sourceObjectId))
            {
                Inventory inventory = player.inventory;
                isDelete = await inventory.DeleteItem(deleteItem);
            }
            else
            {
                isDelete = storage.DeleteItem(deleteItem);
            }

            if (false == isDelete)
            {
                packet.IsSuccess = false;
                packet.SourceObjectId = sourceObjectId;
                packet.DeleteItem = deleteInfo;
                player.Session.Send(packet);
                return;
            }

            packet.IsSuccess = true;
            packet.SourceObjectId = sourceObjectId;
            packet.DeleteItem = deleteInfo;
            player.Session.Send(packet);
        }

        public bool IsInventory(int storageObjectId)
        {
            return storageObjectId == 0;
        }

        public Storage GetStorage(Player player, int storageObjectId)
        {
            Storage storage = new Storage();
            if (storageObjectId == 0)
            {
                Inventory inventory = player.inventory;
                if (inventory == null)
                {
                    return null;
                }

                return inventory.storage;
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

        ////////////////////////////////////////////
        //                                        //
        //               ?????????                //
        //                                        //
        ////////////////////////////////////////////

        internal void HandleRayCast(Player attacker, Vector2 pos, Vector2 dir, float length)
        {

            // TODO : 삭제,  일단 레이를 좀 앞에서 쓰기

            RaycastHit2D hit2D = RaycastManager.Raycast(pos+ pos*dir *0.5f ,dir, length);
            
            if(hit2D.Collider == null)
            {
                return;
            }

            GameObject hitObject = hit2D.Collider.Parent;
            if (hitObject == null)
            {
                Console.WriteLine("HandleRayCast null");
                return;
            }


            if(hitObject.ObjectType == GameObjectType.Player || hitObject.ObjectType == GameObjectType.Monster)
            {
                CreatureObj creatureObj = hitObject as CreatureObj;


                //TODO : 공격력  attacker 밑에 넣기 240814지승현
                creatureObj.OnDamaged(attacker, 3);

                S_ChangeHp ChangeHpPacket = new S_ChangeHp();
                ChangeHpPacket.ObjectId = hitObject.Id;
                ChangeHpPacket.Hp = creatureObj.Hp;

                Console.WriteLine("attacker Id :" + attacker.Id + ", " + "HIT ID " + hitObject.Id + "HIT Hp : "+ hitObject.Hp);

                BroadCast(ChangeHpPacket);
            }

            S_RaycastHit packet = new S_RaycastHit();
            packet.HitObjectId = hitObject.Id;
            packet.RayId = hit2D.rayID;
            packet.Distance = hit2D.distance;
            packet.HitPointX = hit2D.hitPoint.Value.X;
            packet.HitPointY = hit2D.hitPoint.Value.Y;


            BroadCast(packet);

        }

        internal void HandleExitGame(Player player, int exitId)
        {

            //오브젝트 매니저의 딕셔너리에서 플레이어의 인벤토리(그리드, 아이템)와 플레이어를 제거
            player.inventory.ClearInventory();
            ObjectManager.Instance.Remove(player.inventory.Id);

            ObjectManager.Instance.Remove(player.Id);

            S_ExitGame packet = new S_ExitGame()
            {
                PlayerId = player.Id,
                ExitId = exitId
            };

            BroadCast(packet);
        }
    }
}