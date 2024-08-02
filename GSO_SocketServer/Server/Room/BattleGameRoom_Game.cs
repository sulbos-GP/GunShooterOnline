using Google.Protobuf.Protocol;
using Server.Game;
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

            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    player.inventory.instantGrid[0].gridSlot[item.itemDataInfo.ItemPosX + x, item.itemDataInfo.ItemPosY + y] = 0;
                }
            }



        }

        internal void HandleItemLoad(Player player, int playerId, int inventoryId )
        {
            S_LoadInventory s_LoadInventory = new S_LoadInventory();

            s_LoadInventory.PlayerId = player.Id;
            s_LoadInventory.InventoryId = inventoryId;


            Player targetPlayer = ObjectManager.Instance.Find<Player>(playerId);


            s_LoadInventory.InvenData = targetPlayer.inventory.invenData;

            //BroadCast s_LoadInventory
        }

        internal void HandleItemMove(Player player, int itemId, int itemPosX, int itemPosY)
        {
            player.inventory.MoveItem(itemId, itemPosX, itemPosY);

            //S_



       





            // BroadCast()
        }
    }
}