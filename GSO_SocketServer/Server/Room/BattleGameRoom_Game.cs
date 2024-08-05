using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

            Console.WriteLine("HandleMove" + packet.PositionInfo.PosX + ", " + packet.PositionInfo.PosY);

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


            mMap.ApplyMove(player,
                new Vector2Int((int)Math.Round(packet.PositionInfo.PosX), (int)Math.Round(packet.PositionInfo.PosY)));

            BroadCast(player.CurrentRoomId, resMovePacket);
        }

        internal void HandleItemDelete(Player player, int playerId, int itemId)
        {
            //TODO : 그리드로 옮기기
            //TODO : playerId -> ownerId box같이 내꺼 아닌것도 버릴수 있게
            ItemObject item =  ObjectManager.Instance.Find<ItemObject>(itemId);


            player.inventory.instantGrid[0].DeleteItemFromSlot(item);


        }

        internal void HandleItemLoad(Player player, int objectId, int inventoryId )
        {
            InvenDataInfo targetData = null;
            if(player.Id == inventoryId) 
            {
                //플레이어의 인벤토리
                Player targetPlayer = ObjectManager.Instance.Find<Player>(objectId);
                targetData = targetPlayer.inventory.invenData;
            }
            else
            {
                //루터블 오브젝트의 인벤토리
                RootableObject box = ObjectManager.Instance.Find<RootableObject>(inventoryId);
                Console.WriteLine($"handle box id : {box.Id}");
                targetData = box.Inventory.invenData;
            }
            
            //Player targetPlayer = ObjectManager.Instance.Find<Player>(objectId);

            S_LoadInventory s_LoadInventory = new S_LoadInventory()
            {
                PlayerId = objectId,
                InventoryId = inventoryId,
                InvenData = targetData //targetPlayer.inventory.invenData,

            };

            Console.WriteLine(s_LoadInventory.InvenData.GridData.Count);

            BroadCast(RoomId, s_LoadInventory);

        }

        /*internal void HandleItemMove(Player player, int itemId, int itemPosX, int itemPosY)
        {
            player.inventory.MoveItem(itemId, itemPosX, itemPosY);

            //S_
            S_MoveItem s_MoveItem = new S_MoveItem()
            {
                PlayerId = player.Id,
                ItemId = itemId,
                ItemPosX = itemPosX,
                ItemPosY = itemPosY,

            };


            BroadCast(RoomId, );

            // BroadCast()
        }*/
        internal void HandleItemMove(Player player, object  _packet)
        {
            //player.inventory.MoveItem(itemId, itemPosX, itemPosY);
            C_MoveItem packet = (C_MoveItem)_packet;
            //S_
            S_MoveItem s_MoveItem = new S_MoveItem()
            {
                PlayerId = player.Id,
                ItemId = packet.ItemId,
                ItemPosX = packet.ItemPosX,
                ItemPosY = packet.ItemPosY,
                ItemRotate = packet.ItemRotate,
                GridId = packet.GridId,

                LastItemPosX = packet.LastItemPosX,
                LastItemPosY = packet.LastItemPosY,
                LastItemRotate = packet.LastItemRotate,
                //TODO : MoveItem 결과
                //LastInventoryId = packet.in
                LastGridId = packet.LastGridId,
            };


            BroadCast(RoomId, s_MoveItem);

            // BroadCast()
        }







        internal void HandleRayCast(Player attacker, Vector2 pos, Vector2 dir, float length)
        {
            RaycastHit2D hit = RaycastManager.Raycast(pos,dir, length);


            GameObject go = hit.Collider.Parent;
            if (go == null)
            {
                Console.WriteLine("HandleRayCast null");
                return;
            }


            if(go.ObjectType == GameObjectType.Player || go.ObjectType == GameObjectType.Monster)
            {
                CreatureObj creatureObj = go as CreatureObj;

                creatureObj.OnDamaged(attacker, attacker.Attack);

            }

            S_RaycastHit packet = new S_RaycastHit();
            packet.HitObjectId = hit.Id;
            packet.Distance = hit.distance;
            packet.HitPointX = hit.hitPoint.Value.X;
            packet.HitPointY = hit.hitPoint.Value.Y;

            
            BroadCast(RoomId, packet);

        }
    }
    
    
}