using Google.Protobuf;
using Google.Protobuf.Protocol;
using LiteNetLib;
using Server;
using Server.Data;
using Server.Game;
using Server.Game.Utils;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

class PacketHandler
{
    internal static void C_DeleteItemHandler(PacketSession session, IMessage message)
    {
        //인벤토리 열었을떄
        ClientSession clientSession = session as ClientSession;
        C_DeleteItem packet = (C_DeleteItem)message;
        Console.WriteLine($"C_DeleteItemHandler {packet.PlayerId}");

        Player player = clientSession.MyPlayer;

        player.gameRoom.HandleItemDelete(player, packet.PlayerId, packet.ItemId);
    }

    internal static void C_EnterGameHandler(PacketSession session, IMessage message)
    {
        Console.WriteLine("C_EnterGameHandler");
        ClientSession clientSession = session as ClientSession;
        C_EnterGame enterGamePacket = (C_EnterGame)message;
        Player p = ObjectManager.Instance.Add<Player>();

        {
            p.Session = clientSession;
            p.info.Name = enterGamePacket.Name + clientSession.SessionId;
            p.info.PositionInfo.PosX = 0;
            p.info.PositionInfo.PosY = 0;
        }

        clientSession.MyPlayer = p;



        
        BattleGameRoom room = (BattleGameRoom)Program.mNetworkService.gameRoom; //나중에 null로 바꿔도 참조가능

        room.Push(room.EnterGame, clientSession.MyPlayer);

    }

    internal static void C_LoadInventoryHandler(PacketSession session, IMessage message)
    {
        //인벤토리 열었을떄
        ClientSession clientSession = session as ClientSession;
        C_LoadInventory packet = (C_LoadInventory)message;
        Console.WriteLine($"C_LoadInventoryHandler {packet.PlayerId}");

        Player player = clientSession.MyPlayer;

        player.gameRoom.HandleItemLoad(player, packet.PlayerId,  packet.InventoryId);
        

    }

    /*public static void C2S_ChatHandler(PacketSession session, IPacket packet)
	{
        C2S_Chat pkt = packet as C2S_Chat;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine("Server Recv[{0}] : {1}", session.mPeer.Port, pkt.chat);

        S2C_Chat chat = new S2C_Chat();
        chat.chat = pkt.chat;
        clientSession.Send(chat.Write(), DeliveryMethod.ReliableOrdered);
    }*/

    internal static void C_MoveHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;

        C_Move move = (C_Move)message;
        BattleGameRoom room = clientSession.Room; //나중에 null로 바꿔도 참조가능

        room.Push(room.HandleMove , clientSession.MyPlayer, move);


        

    }

    internal static void C_MoveItemHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_MoveItem packet = (C_MoveItem)message;
        Console.WriteLine($"C_MoveItemHandler {packet.PlayerId}");

        Player player = clientSession.MyPlayer;
        player.gameRoom.HandleItemMove(player, packet.ItemId, packet.ItemPosX, packet.ItemPosY);
    }

    internal static void C_RaycastShootHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_RaycastShoot packet = (C_RaycastShoot)message;
        Console.WriteLine($"C_RaycastShootHandler0");

        Player player = clientSession.MyPlayer;
        player.gameRoom.Push(player.gameRoom.HandleRayCast, player, new Vector2(packet.StartPosX, packet.StartPosY), new Vector2(packet.DirX, packet.DirY), packet.Length);
    }



   
}
