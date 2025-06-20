﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using LiteNetLib;
using Newtonsoft.Json;
using Server;
using Server.Data;
using Server.Database.Game;
using Server.Database.Handler;
using Server.Game;
using Server.Game.Object;
using Server.Game.Object.Item;
using Server.Game.Utils;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCommonLibrary.DTO.GameServer;
using WebCommonLibrary.DTO.Performance;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

public class PacketHandler
{
    //GWANHO TEMP
    public static int cnt = 1;



    internal static void C_EnterGameHandler(PacketSession session, IMessage message)
    {
        // (인게임)1명 입장 패킷
        ClientSession clientSession = session as ClientSession;
        C_EnterGame enterGamePacket = (C_EnterGame)message;
        Console.WriteLine($"C_EnterGameHandler");

        if (clientSession.MyPlayer == null)
        {
            Console.WriteLine(clientSession.MyPlayer is null);
            //Thread.Sleep(1000);
        }
        Player player = clientSession.MyPlayer;

        // TODO : Credential 확인 작업 , packet.Credential.Uid


        player.gameRoom.Push(player.gameRoom.HandleClientLoadGame, player);
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

        C_Move packet = (C_Move)message;
        BattleGameRoom room = clientSession.Room; //나중에 null로 바꿔도 참조가능


        room.Push(room.HandleMove, clientSession.MyPlayer, packet.PositionInfo);

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

        //player.gameRoom.DeleteItemHandler(player, packet.SourceObjectId, packet.DeleteItemId);
    }

    internal static void C_RaycastShootHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_RaycastShoot packet = (C_RaycastShoot)message;
        //Console.WriteLine($"C_RaycastShootHandler0");

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        player.gameRoom.Push(player.gameRoom.HandleRayCast, player, new Vector2(packet.StartPosX, packet.StartPosY), new Vector2(packet.DirX, packet.DirY));
    }

    internal static void C_ExitGameHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_ExitGame packet = (C_ExitGame)message;

        Console.WriteLine($"C_ExitPacketHandler");

        Player player = clientSession.MyPlayer;

        if (player == null) //Lobby
        {
            return;
        }

        if (packet.IsNormal == true)
        {
            player.gameRoom.Push(player.gameRoom.HandleExitGame, player, packet.ExitId);

        }
        else
        {
            player.gameRoom.Push(player.gameRoom.LeaveGame, player.Id);
            player.gameRoom.HandleClientLeave(player);
        }

    }

    internal static void C_JoinServerHandler(PacketSession session, IMessage message)
    {
        Console.WriteLine("C_JoinServerHandler");

        //Task.Delay(1000).Wait();



        //접속 요청
        ClientSession clientSession = session as ClientSession;
        C_JoinServer packet = (C_JoinServer)message;

        //인증 과정

        if (packet.Credential == null)
        {
            Console.WriteLine("packet.Credential is null -> debug mode");
        }

        GameRoom gameRoom = Program.gameserver.gameRoom;
        if (gameRoom.sessions.Contains(clientSession) == false) //클라이언트 세션이 없으면
        {
            gameRoom.sessions.Add(clientSession); //추가
        }
        else //있으면
        {
            return;
        }



        GameState state = gameRoom.CurrentGameState;
        if (state == GameState.ALIVE)
        {

            Player p = ObjectManager.Instance.Add<Player>();
            {
                p.Session = clientSession;
                p.info.Name = packet.Name + cnt;
                p.info.PositionInfo.PosX = 0;
                p.info.PositionInfo.PosY = 0;
                //p.gameRoom = Program.gameserver.gameRoom as BattleGameRoom;



#if DOCKER
            //이거 uid를 검사해서 올바르게 넣어주면 됨
            if(p.gameRoom.connectPlayer.Count > 0 )
            {
                if(p.gameRoom.connectPlayer.Contains(packet.Credential.Uid) == true)
                {
                    p.UID = packet.Credential.Uid;
                     p.credential = packet.Credential;
            
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

                p.UID = cnt++;
#endif

                //p.stat

            }


            clientSession.Room = Program.gameserver.gameRoom as BattleGameRoom;
            clientSession.MyPlayer = p;

            BattleGameRoom room = (BattleGameRoom)Program.gameserver.gameRoom; //나중에 null로 바꿔도 참조가능
            Console.WriteLine("!@#");
            // enter로 이동
            room.Push(room.HandleJoin, packet.Credential, clientSession.MyPlayer);
            ObjectManager.Instance.DebugObjectDics();
        }
        else if (state == GameState.LOADING)
        {
            S_Error errorPacket = new S_Error();
            errorPacket.ErrorCode = ErrorType.ServerLoading;
            clientSession.Send(errorPacket);

        }
        else if (state == GameState.INGAME)
        {
            S_Error errorPacket = new S_Error();
            errorPacket.ErrorCode = ErrorType.MatchmakingFailed;
            clientSession.Send(errorPacket);

        }
        else
        {
            Console.WriteLine("ERROR GAME SERVER DEAD");
        }


    }

    internal static void C_ChangeAppearanceHandler(PacketSession session, IMessage message)
    {
        //만약 서버쪽에서도 현재 들고있는 총을 참조해야할때 해당 변수의 변경은 여기서 진행
        //(단 여기서 주는 gunId는 총의 종류만 알수 있는 마스터 코드임. 해당 총의 오브젝트 id가 필요한 경우 성훈에게 말할것)

        ClientSession clientSession = session as ClientSession;
        C_ChangeAppearance packet = (C_ChangeAppearance)message;
        Console.WriteLine($"[C_ChangeAppearance]");
        Console.WriteLine($"playerid : {packet.ObjectId}");
        //Console.WriteLine($"gunId : {packet.GunId}"); //총의 마스터 아이디 -> 아이템 오브젝트 아이디
        Player player = clientSession.MyPlayer;

        if (player != null)
        {
            player.gameRoom.Push(player.gameRoom.ChangeAppearance, player, packet.ObjectId, packet.GunType);
        }

    }

    internal static void C_InputDataHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = session as ClientSession;
        C_InputData packet = (C_InputData)message;
        //Console.WriteLine($"[C_InputData]");

        Player player = clientSession.MyPlayer;

        if( player == null )
        {
            Console.WriteLine("ERROR : MyPlayer is null");
            return;
        }

        player.gameRoom.Push(player.gameRoom.HandleInputData, player, packet);
    }

    internal static void C_PongHandler(PacketSession session, IMessage message)
    {
        //C_Pong packet = (C_Pong)message;
        ClientSession clientSession = session as ClientSession;

        ulong last;
        if (session.LastTick.TryGetValue(session.Rindex++, out last) == true)
        {
            session.RTT = (uint)(LogicTimer.Tick - last) / 2;
            //Console.WriteLine("1. session.RTT : " + session.RTT);
        }
        else
        {
            Console.WriteLine("C_PongHandler 에러");
        }

        //Console.WriteLine($"2. Pong RTT = {session.RTT}  ");

        S_Ping s_Ping = new S_Ping();
        s_Ping.IsEnd = true;
        s_Ping.Tick = LogicTimer.Tick + session.RTT;

        //Console.WriteLine($"3. current = {LogicTimer.Tick} Predict = {LogicTimer.Tick + session.RTT}  ");

        clientSession.Send(s_Ping);

    }
}
