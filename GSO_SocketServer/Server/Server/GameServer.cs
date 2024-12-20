using Google.Protobuf.Protocol;
using Server.Database.Handler;
using Server.Game.Quest;
using Server.Web;
using Server.Web.Service;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCommonLibrary.DTO.GameServer;
using WebCommonLibrary.DTO.Performance;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

namespace Server.Server
{
    public class GameServer : ServerNetworkService
    {

        protected override async Task<bool> OnStart()
        {
            Program.database.initializeAndLoadData();
            
            QuestSystem.Instance.Initialize();

            BattleGameRoom room = new BattleGameRoom();
            this.SetChannel(true, room, 0);
            room.Start();

            mCoreWorkThread.Start();
            mGameLogicTimer.Start();

#if DOCKER
            try
            {
                Program.web.initializeServiceAndResource();

                //준비 완료 요청
                RequestReadyMatchRes requestReady = await Program.web.ServerManager.PostRequestReady();
                if (requestReady.error_code != WebErrorCode.None)
                {
                    return false;
                }
                Console.WriteLine("Server is ready");


                //준비 대기
                Console.WriteLine("Wait for start match");
                while (true)
                {
                    AllocateMatchRes allocateMatch = await Program.web.ServerManager.PostWaitForAllocateMatch();
              
                    if (allocateMatch != null && allocateMatch.error_code == WebErrorCode.None)
                    {
                        Console.WriteLine($"[StartMatch] player count : {allocateMatch.players.Count}");
                        Console.WriteLine("{");
                        foreach (int player in allocateMatch.players)
                        {
                            Console.WriteLine($"\tPlayer UID : {player}");
                        }
                        Console.WriteLine("}");
                        room.connectPlayer = allocateMatch.players;

                        break;
                    }

                    Thread.Sleep(1000);
                }

                StartMatchRes startMatch = await Program.web.ServerManager.PostStartMatch();
                if(startMatch.error_code != WebErrorCode.None)
                {
                    Console.WriteLine("게임 시작 요청에서 에러가 발생하였습니다.");
                }

            }
            catch (Exception ex)
            {
                //TaskError는 GameServerManager(WEB) 에서 대기하고 있던 토큰이 취소되면서 발생
                Console.WriteLine($"[OnStart] : {ex.Message}");
                return false;
            }
#else
            await Task.Yield();
#endif

          


            return true;
        }
        protected override async Task<bool> OnStop()
        {

#if DOCKER
            //게임 종료 정보
            BattleGameRoom battleGame =  gameRoom as BattleGameRoom;
            if (battleGame == null)
            {
                Console.WriteLine("게임 룸의 정보가 없습니다.");
            }

            //플레이어 레이팅 집계 요청
            PlayerRatingRes playerRating = await Program.web.Lobby.PostPlayerRating(battleGame.MatchInfo);
            if(playerRating.error_code != WebErrorCode.None)
            {
                Console.WriteLine("플레이어의 레이팅이 집계되지 못하였습니다.");
            }

            //종료 요청
            await Program.web.ServerManager.PostShutdown();
#else
            await Task.Yield();
#endif

            return true;
        }
    }
}
