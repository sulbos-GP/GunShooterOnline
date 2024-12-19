using Collision.Shapes;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Models.GameDB;

namespace Server.Game
{
    public class ExitZone : GameObject
    {
        private IJob _job;

        private const int EXIT_TIME = 5000;
        private const double EXIT_DISTANCE = 1.5;

        private Dictionary<int, int> playerTickCounts = new Dictionary<int, int>();

        public ExitZone()
        {
            ObjectType = GameObjectType.Exitzone;
            /*float width = 2;
            float left = 2;
            float top = 2;*/
            float width = 1;
            float left = 1;
            float top = 1;

            Polygon rectangle = ShapeManager.CreateCenterSquare(left, top, width);
            rectangle.Parent = this;
            currentShape = rectangle;

        }

        public void Init(Vector2 pos)
        {
            CellPos = pos;
            info.Name = "ExitZone" + Id;

        }

        public override void Update()
        {

            if (gameRoom != null)
                _job = gameRoom.PushAfter(Program.ServerIntervalTick, Update);

            Vector2 exitZonePos = CellPos;
            foreach ((int playerId, int interactiveTick) in playerTickCounts)
            {
                Player player = ObjectManager.Instance.Find<Player>(playerId);
                if(player == null)
                {
                    OnLevaeExitZone(player, "player is null");
                    break;
                }

                if(player.isInteractive == false)
                {
                    OnLevaeExitZone(player, "player interative is false");
                    break;
                }

                Vector2 playerPos = player.CellPos;
                double distance = Math.Sqrt(Math.Pow(playerPos.X - exitZonePos.X, 2) + Math.Pow(playerPos.Y - exitZonePos.Y, 2));
                if (distance >= EXIT_DISTANCE)
                {
                    OnLevaeExitZone(player, $"player distance over {EXIT_DISTANCE}");
                    break;
                }
                
            }
            
        }

        public void OnEnterExitZone(Player enterPlayer)
        {
            bool isFind = playerTickCounts.TryGetValue(enterPlayer.Id, out int tickCount);
            if (isFind == true)
            {
                return;
            }
            playerTickCounts.Add(enterPlayer.Id, Environment.TickCount);

            enterPlayer.isInteractive = true;
            enterPlayer.gameRoom.PushAfter(EXIT_TIME, HandleExit, enterPlayer);
        }

        public void OnLevaeExitZone(Player leavePlayer, string cause)
        {
            Console.WriteLine($"OnLevaeExitZone : {cause}");
            bool isFind = playerTickCounts.TryGetValue(leavePlayer.Id, out int tickCount);
            if (isFind == true)
            {
                S_ExitGame exitPacket = new S_ExitGame()
                {
                    IsSuccess = false,
                    RetryTime = (tickCount + EXIT_TIME) - Environment.TickCount,
                    PlayerId = leavePlayer.Id,
                    ExitId = this.Id
                };
                leavePlayer.gameRoom.BroadCast(exitPacket);

                playerTickCounts.Remove(leavePlayer.Id);
            }
        }

        private void HandleExit(Player exitPlayer)
        {
            bool isFind = playerTickCounts.TryGetValue(exitPlayer.Id, out int tickCount);
            if (isFind == false)
            {
                return;
            }
            playerTickCounts.Remove(exitPlayer.Id);

            if(tickCount + EXIT_TIME > Environment.TickCount)
            {
                //OnLevaeExitZone(exitPlayer, $"tick count over {Environment.TickCount - (tickCount + EXIT_TIME)}");
                return;
            }

            if (exitPlayer.gameRoom.MatchInfo.TryGetValue(exitPlayer.UID, out MatchOutcome outcome) == true)
            {
                outcome.escape += 1;
            }

            //인벤토리 및 장비 세이브
            exitPlayer.gear.Save().Wait();
            exitPlayer.inventory.Save().Wait();

            //Play관련 이벤트 버스
            EventBus.Publish(EEventBusType.Play, exitPlayer, "PLAY_OUT");

            //웹에 플레이어 메타데이터 보내기
            exitPlayer.gameRoom.PostPlayerStats(exitPlayer.Id);

            //오브젝트 매니저의 딕셔너리에서 플레이어의 인벤토리(그리드, 아이템)와 플레이어를 제거
            //exitPlayer.inventory.ClearInventory();
            ObjectManager.Instance.Remove(exitPlayer.inventory.Id);

            S_ExitGame exitPacket = new S_ExitGame()
            {
                IsSuccess = true,
                PlayerId = exitPlayer.Id,
                ExitId = this.Id
            };
            exitPlayer.gameRoom.BroadCast(exitPacket);

            S_Despawn despawnPacket = new S_Despawn();
            despawnPacket.ObjcetIds.Add(exitPlayer.Id);
            exitPlayer.gameRoom.BroadCast(despawnPacket);


            exitPlayer.gameRoom.LeaveGame(exitPlayer.Id);

            //사람 전부 나가면 gameserver.Stop();
            //gameserver.Stop();

            //stop 부분에 모든 남아있는 플레이어 처리!!!!
        }

    }
}
