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


            map.ApplyMove(player,
                new Vector2Int((int)Math.Round(packet.PositionInfo.PosX), (int)Math.Round(packet.PositionInfo.PosY)));

            BroadCast(resMovePacket);
        }

        internal void HandleItemDelete(Player player, int playerId, int itemId)
        {
            //TODO : 그리드로 옮기기
            //TODO : playerId -> ownerId box같이 내꺼 아닌것도 버릴수 있게
            ItemObject item =  ObjectManager.Instance.Find<ItemObject>(itemId);

            item.ownerGrid.DeleteItemFromSlot(item);
            ObjectManager.Instance.Remove(itemId);

            S_DeleteItem s_DeleteItem = new S_DeleteItem();
            s_DeleteItem.ItemId = itemId;
            s_DeleteItem.PlayerId = playerId;

            BroadCast(s_DeleteItem);
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
                targetData = box.inventory.invenData;
            }
            
            //Player targetPlayer = ObjectManager.Instance.Find<Player>(objectId);

            S_LoadInventory s_LoadInventory = new S_LoadInventory()
            {
                PlayerId = objectId,
                InventoryId = inventoryId,
                InvenData = targetData //targetPlayer.inventory.invenData,

            };

            Console.WriteLine(s_LoadInventory.InvenData.GridData.Count);

            BroadCast(s_LoadInventory);

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

            ItemObject target = ObjectManager.Instance.Find<ItemObject>(packet.ItemId);
            Grid targetGrid = null;
            if (packet.PlayerId == packet.InventoryId)
            {
                //플레이어의 그리드로 옮김
                ObjectManager.Instance.Find<Player>(packet.InventoryId).inventory.instantGrid.TryGetValue(packet.GridId, out targetGrid);
            }
            else
            {
                ObjectManager.Instance.Find<RootableObject>(packet.InventoryId).inventory.instantGrid.TryGetValue(packet.GridId, out targetGrid);
            }

            if (targetGrid == null)
            {
                Console.WriteLine("해당 그리드가 존재하지 않음");
                return;
            }

            target.ownerGrid.ownerInventory.MoveItem(packet.ItemId, packet.ItemPosX, packet.ItemPosY, packet.ItemRotate, targetGrid);

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


            BroadCast(s_MoveItem);

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

                //creatureObj.OnDamaged(attacker, attacker.Attack);

            }

            S_RaycastHit packet = new S_RaycastHit();
            packet.HitObjectId = hit.Id;
            packet.Distance = hit.distance;
            packet.HitPointX = hit.hitPoint.Value.X;
            packet.HitPointY = hit.hitPoint.Value.Y;

            
            BroadCast(packet);

        }

        internal void HandleExitGame(Player player, int exitId)
        {

            //오브젝트 매니저의 딕셔너리에서 플레이어의 인벤토리(그리드, 아이템)와 플레이어를 제거
            foreach (GridDataInfo grid in player.inventory.invenData.GridData)
            {
                foreach (ItemDataInfo itemData in grid.ItemList)
                {
                    ObjectManager.Instance.Remove(itemData.ItemId);
                }
            }
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