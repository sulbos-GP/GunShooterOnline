using Google.Protobuf.Protocol;
using LiteNetLib;
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
            Inventory inventory = player.inventory;
            if(inventory == null)
            {
                return;
            }

            S_LoadInventory packet = new S_LoadInventory();
            foreach(PS_ItemInfo item in inventory.GetInventoryItems())
            {
                packet.ItemInfos.Add(item);
            }

            player.Session.Send(packet);
        }

        internal async void MergeItemHandler(Player player, int mergedObjectId, int combinedObjectId, int mergeNumber)
        {
            S_MergeItem packet = new S_MergeItem();

            Inventory inventory = player.inventory;
            if (inventory == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            ItemObject mergeItem = ObjectManager.Instance.Find<ItemObject>(mergedObjectId);
            ItemObject combinedItem = ObjectManager.Instance.Find<ItemObject>(combinedObjectId);

            if (mergeItem == null || combinedItem == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            if(false == await inventory.MergeItem(mergeItem, combinedItem, mergeNumber))
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            packet.IsSuccess = true;
            packet.MergedItem = mergeItem.Info;
            packet.CombinedItem = combinedItem.Info;
            player.Session.Send(packet);
        }

        internal async void DevideItemHandler(Player player, int totalObjectId, int gridX, int gridY, int rotation, int devideNumber)
        {

            S_DevideItem packet = new S_DevideItem();

            Inventory inventory = player.inventory;
            if (inventory == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            ItemObject totalItem = ObjectManager.Instance.Find<ItemObject>(totalObjectId);
            if (totalItem == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            ItemObject newItem = await inventory.DevideItem(totalItem, gridX, gridY, rotation, devideNumber);
            if (newItem == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            packet.IsSuccess = true;
            packet.TotalItem = totalItem.Info;
            packet.NewItem = newItem.Info;
            player.Session.Send(packet);
        }

        internal void MoveItemHandler(Player player, int targetObjectId, PS_ItemInfo moveItem, int gridX, int gridY)
        {

        }

        internal async void DeleteItemHandler(Player player, int deleteItemId)
        {

            S_DeleteItem packet = new S_DeleteItem();

            Inventory inventory = player.inventory;
            if (inventory == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            ItemObject deleteItem = ObjectManager.Instance.Find<ItemObject>(deleteItemId);
            if (deleteItem == null)
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }
            PS_ItemInfo itemInfo = deleteItem.Info;

            if(false == await inventory.DeleteItem(deleteItem))
            {
                packet.IsSuccess = false;
                player.Session.Send(packet);
                return;
            }

            packet.IsSuccess = true;
            packet.DeleteItem = itemInfo;
            player.Session.Send(packet);
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