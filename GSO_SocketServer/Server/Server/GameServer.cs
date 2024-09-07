using Google.Protobuf.Protocol;
using Server.Database.Handler;
using Server.Web;
using Server.Web.Service;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.DTO.GameServer;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

namespace Server.Server
{
    public class GameServer : ServerNetworkService
    {



        protected override async Task<bool> OnStart()
        {
            Program.database.initializeAndLoadData();

            BattleGameRoom room = new BattleGameRoom();
            this.SetChannel(true, room, 0);
            gameRoom = room;
            mCoreWorkThread.Start();
            mGameLogicTimer.Start();


#if DOCKER
            Program.web.initializeServiceAndResource();

            //준비 완료 요청
            RequestReadyMatchRes requestReady = await Program.web.ServerManager.PostRequestReady();
            if(requestReady.error_code != WebErrorCode.None)
            {
                return false;
            }
            Console.WriteLine("Server is ready");

            //준비 대기
            MatchPlayersRes matchPlayer = await Program.web.ServerManager.PostWaitForMatchPlayers();
            Console.WriteLine($"[MatchPlayer] Count : {matchPlayer.players.Count}");
            Console.WriteLine("{");
            foreach (int player in matchPlayer.players)
            {
                Console.WriteLine($"\tPlayer UID : {player}");
            }
            Console.WriteLine("}");
            room.connectPlayer = matchPlayer.players;

#else

#endif

            return true;
        }
        protected override async Task<bool> OnStop()
        {

#if DOCKER
            //게임 종료 정보
            Dictionary<int, MatchOutcomeInfo> outcome = new Dictionary<int, MatchOutcomeInfo>();

            BattleGameRoom battleGame =  gameRoom as BattleGameRoom;

            if (battleGame != null)
            {
                await Program.web.Lobby.PostMatchOutcome(battleGame.MatchInfo);

            }


            //종료 요청
            await Program.web.ServerManager.PostShutdown();
#else

#endif

            return true;
        }
    }
}
