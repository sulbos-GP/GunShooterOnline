using Google.Protobuf;
using Google.Protobuf.Protocol;
using LiteNetLib;
using Newtonsoft.Json;
using Server;
using Server.Data;
using Server.Database.Game;
using Server.Database.Handler;
using Server.Game;
using Server.Game.Object;
using Server.Game.Utils;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Error;

class PacketHandler
{
    //GWANHO TEMP
    private static int cnt = 0;

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
            p.gameRoom = Program.gameserver.gameRoom as BattleGameRoom;
            //바꾼 부분

#if DOCKER
            //이거 uid를 검사해서 올바르게 넣어주면 됨
            if(p.gameRoom.connectPlayer.Count > 0 )
            {
                if(p.gameRoom.connectPlayer.Contains(enterGamePacket.Credential.Uid) == true)
                {
                    p.UID = enterGamePacket.Credential.Uid;
                }
                else { 
                
                    Console.WriteLine("connectPlayer Contains not p.UID ");
                }

            }
            else
            {
                Console.WriteLine("p.gameRoom.connectPlayer.Count is 0");
            }

#else

            p.UID = ++cnt;
#endif

            //p.stat

        }


        clientSession.Room = Program.gameserver.gameRoom as BattleGameRoom;
        clientSession.MyPlayer = p;

        BattleGameRoom room = (BattleGameRoom)Program.gameserver.gameRoom; //나중에 null로 바꿔도 참조가능

        room.Push(room.EnterGame, clientSession.MyPlayer);
        ObjectManager.Instance.DebugObjectDics();
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

    ////////////////////////////////////////////
    //                                        //
    //               INVENTORY                //
    //                                        //
    ////////////////////////////////////////////

    /// <summary>
    /// 인벤 버튼을 눌렀을때 
    /// </summary>
    internal static void C_LoadInventoryHandler(PacketSession session, IMessage message)
    {
        //인벤토리 열었을떄
        ClientSession clientSession = session as ClientSession;
        C_LoadInventory packet = (C_LoadInventory)message;
        Console.WriteLine($"[C_LoadInventoryHandler]");

        Player player = clientSession.MyPlayer;
        if (packet.SourceObjectId == 0)
        {
            player.gameRoom.Push(player.gameRoom.LoadInventoryHandler, player);
        }
        else
        {
            player.gameRoom.Push(player.gameRoom.LoadInventoryHandler, player, packet.SourceObjectId);
        }

    }

    internal static void C_CloseInventoryHandler(PacketSession session, IMessage message)
    {
        //인벤토리 열었을떄
        ClientSession clientSession = session as ClientSession;
        C_CloseInventory packet = (C_CloseInventory)message;
        Console.WriteLine($"[C_CloseInventoryHandler]");

        Player player = clientSession.MyPlayer;
        if (packet.SourceObjectId == 0)
        {
            player.gameRoom.Push(player.gameRoom.CloseInventoryHandler, player);
        }
        else
        {
            player.gameRoom.Push(player.gameRoom.CloseInventoryHandler, player, packet.SourceObjectId);

        }
        
    }

    internal static void C_SearchItemHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_SearchItem packet = (C_SearchItem)message;
        Console.WriteLine($"[C_SearchItemHandler]");

        Player player = clientSession.MyPlayer;
        player.gameRoom.Push(player.gameRoom.SearchItemHandler, player, packet.SourceObjectId, packet.SourceItemId);

    }

    internal static void C_MergeItemHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_MergeItem packet = (C_MergeItem)message;
        Console.WriteLine($"[C_MergeItemHandler]");

        Player player = clientSession.MyPlayer;

        player.gameRoom.Push(player.gameRoom.MergeItemHandler, player, packet.SourceObjectId, packet.DestinationObjectId, packet.MergedObjectId, packet.CombinedObjectId, packet.MergeNumber);

    }

    internal static void C_DevideItemHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_DevideItem packet = (C_DevideItem)message;
        Console.WriteLine($"[C_DevideItemHandler]");

        Player player = clientSession.MyPlayer;
        player.gameRoom.Push(player.gameRoom.DevideItemHandler, player, packet.SourceObjectId, packet.DestinationObjectId, packet.SourceItemId, packet.DestinationGridX, packet.DestinationGridY, packet.DestinationRotation, packet.DevideNumber);
    }

    internal static void C_MoveItemHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_MoveItem packet = (C_MoveItem)message;
        Console.WriteLine($"[C_MoveItemHandler]");

        Player player = clientSession.MyPlayer;
        player.gameRoom.Push(player.gameRoom.MoveItemHandler, player, packet.SourceObjectId, packet.DestinationObjectId, packet.SourceMoveItemId, packet.DestinationGridX, packet.DestinationGridY, packet.DestinationRotation);
    }

    internal static void C_DeleteItemHandler(PacketSession session, IMessage message)
    {
        //인벤토리 열었을떄
        ClientSession clientSession = session as ClientSession;
        C_DeleteItem packet = (C_DeleteItem)message;
        Console.WriteLine($"[C_DeleteItemHandler]");

        Player player = clientSession.MyPlayer;
        player.gameRoom.Push(player.gameRoom.DeleteItemHandler, player, packet.SourceObjectId, packet.DeleteItemId);
    }

    internal static void C_RaycastShootHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_RaycastShoot packet = (C_RaycastShoot)message;
        //Console.WriteLine($"C_RaycastShootHandler0");

        Player player = clientSession.MyPlayer;
        player.gameRoom.Push(player.gameRoom.HandleRayCast, player, new Vector2(packet.StartPosX, packet.StartPosY), new Vector2(packet.DirX, packet.DirY), packet.Length);
    }

    internal static void C_ExitGameHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_ExitGame packet = (C_ExitGame)message;
        Console.WriteLine($"C_ExitPacketHandler");

        Player player = clientSession.MyPlayer;


        player.gameRoom.Push(player.gameRoom.HandleExitGame, player, packet.ExitId);
    }

    internal static void C_LoadGameHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_LoadGame packet = (C_LoadGame)message;
        Console.WriteLine($"C_LoadGameHandler");


        Player player = clientSession.MyPlayer;

        player.gameRoom.Push(player.gameRoom.HandleClientLoadGame, player, packet.ExitId);


    }
}
