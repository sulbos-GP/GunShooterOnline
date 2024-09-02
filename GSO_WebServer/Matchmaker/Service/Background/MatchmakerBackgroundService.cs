using System.Diagnostics;
using Matchmaker.Service.Interfaces;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.Error;

namespace Matchmaker.Service.Background
{
    public class MatchmakerBackgroundService : BackgroundService
    {
        private PeriodicTimer?  mTimer = null;
        private const double    mPeriodicSecond = 5;                        //5초마다 매칭 검색
        private const int       mPlayerCapacity = 5;                        //최대 매치될 인원
        private const int       mMaxWaitTimeCount = 120;                    //최대로 기다릴 수 있는 시간 (X초 이상 기다린 플레이어 모두 참여)
        private const long      mExpandingRatingRangeTimeCount = 10;        //X초마다 레이팅 범위 증가

        private const double mExpandingRatingRange = 50.0f;
        private const double mMaxRating = 3000.0f;
        private const double mMinRating = 0.0f;

        private readonly IMatchmakerService mMatchmakerService;
        private readonly IGameServerManagerService mGameServerManagerService;

        public MatchmakerBackgroundService(IMatchmakerService matchmakerService, IGameServerManagerService gameServerManagerService)
        {
            mMatchmakerService = matchmakerService;
            mGameServerManagerService = gameServerManagerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            mTimer = new PeriodicTimer(TimeSpan.FromSeconds(mPeriodicSecond));
            while (await mTimer.WaitForNextTickAsync(stoppingToken))
            {
                stopwatch.Restart();

                //대기열에 있는 모든 플레이어들을 불러온다
                var (error, players) = await mMatchmakerService.ScanPlayers();
                if (error != WebErrorCode.None)
                {
                    continue;
                }

                if (players == null)
                {
                    continue;
                }

                var nowTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                foreach (var player in players)
                {

                    if(player.Value.ticket == null)
                    {
                        continue;
                    }

                    PlayerInfo playerInfo = player.Value;
                    TicketInfo ticketInfo = playerInfo.ticket;

                    //현재 시간에서 매치 시작한 시간 나눔
                    long elapsedSeconds = nowTimestamp - ticketInfo.match_start_time;
                    long count = elapsedSeconds / mExpandingRatingRangeTimeCount;

                    //카운트 만큼 범위를 넓혀줌
                    double expanding = mExpandingRatingRange * count;
                    double maxRating = Math.Clamp(playerInfo.rating + expanding, playerInfo.rating, mMaxRating);
                    double minRating = Math.Clamp(playerInfo.rating - expanding, mMinRating, playerInfo.rating);

                    //일단은 max값은 증가하지 않고 60이 최대로 변경한다
                    //Rating범위 내의 X명의 상대 플레이어를 찾는다, 오래있었는지 여부에 따라 범위값이 증가한다
                    //e.g.) rating=1700, deviation=200일 경우 플레이어의 능력이 구간[1500 ~ 1900]에 있다고 95% 확신 

                    //내가 하고 싶은것이 일단 저 능력구간을 최대한 맞춰주고
                    //진짜 안잡힌다면 deviation을 높여줌으로써 구간을 늘려주도록 하고 싶음
                    //Console.WriteLine($"{player.uid}[{count}] : max:{maxRating}, min:{minRating}");

                    //Capacity명의 비슷한 플레이어들 선별
                    (error, var keys) = await mMatchmakerService.FindMatchByRating(minRating, maxRating, mPlayerCapacity);
                    if (error != WebErrorCode.None)
                    {
                        continue;
                    }

                    if(keys == null)
                    {
                        continue;
                    }

                    //방 있는지 확인
                    (error, var profile) = await mGameServerManagerService.FetchMatchInfo();
                    if(profile == null)
                    {
                        continue;
                    }

                    Console.WriteLine("MatchInfo");
                    Console.WriteLine("{");
                    Console.WriteLine($"\tID      : {profile.container_id}");
                    Console.WriteLine($"\tWORLD   : {profile.world}");
                    Console.WriteLine($"\tH_IP    : {profile.host_ip}");
                    Console.WriteLine($"\tH_PORT  : {profile.host_port}");
                    Console.WriteLine($"\tC_PORT  : {profile.container_port}");
                    Console.WriteLine("}");

                    //해당 클라이언트에게 방 정보 전송
                    error = await mMatchmakerService.NotifyMatchSuccess(keys, profile);
                    if (error != WebErrorCode.None)
                    {
                        continue;
                    }

                    //성공적으로 보냈다면 매칭 큐에서 제거 한다
                    error = await mMatchmakerService.RemoveMatchQueue(keys);
                    if (error != WebErrorCode.None)
                    {
                        continue;
                    }

                }

                stopwatch.Stop();
                TimeSpan elapsed = TimeSpan.FromSeconds(mPeriodicSecond) - stopwatch.Elapsed;
                if (elapsed.Microseconds <= 0)
                {
                    Console.WriteLine("Background over time: " + elapsed.TotalMilliseconds + " ms");
                }
            }
        }

        public override void Dispose()
        {

            base.Dispose();

            if (mTimer != null)
            {
                mTimer.Dispose();
            }

        }

    }
}
