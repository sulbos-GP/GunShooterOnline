using MatchmakingServer.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using GSO_WebServerLibrary;
using MatchmakingServer.Models;

namespace MatchmakingServer.Background
{
    public class BackgroundMatchMaking : BackgroundService
    {
        private PeriodicTimer?  mTimer = null;
        private const double    mDelaySecond = 1;                           //1초마다 매칭 검색
        private const int       mMaxPlayerNumber = 10;                      //최대 매치될 인원
        private const long      mMaxWaitTimeCount = 120;                    //최대로 기다릴 수 있는 시간
        private const long      mMaxExpandingRatingRangeTimeCount = 100;    //최대로 늘어날 수 있는 범위 증가
        private const long      mExpandingRatingRangeTimeCount = 5;         //5초마다 레이팅 증가

        private const double    mMaxDeviationRatingRange = 2.0;             //최대 증가 가능한 범위

        private readonly IMatchingQueue mMatchingQueue;

        public BackgroundMatchMaking(IMatchingQueue matchingQueue)
        {
            mMatchingQueue = matchingQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            mTimer = new PeriodicTimer(TimeSpan.FromSeconds(mDelaySecond));
            while (await mTimer.WaitForNextTickAsync(stoppingToken))
            {
                stopwatch.Restart();

                //TODO : 방이 있는지 확인한다


                //대기열에 있는 모든 플레이어들을 불러온다
                var scanResult = await mMatchingQueue.ScanPlayers();
                if(scanResult.Item1 != WebErrorCode.None)
                {
                    continue;
                }
                
                var players = scanResult.Item2;
                if(players == null)
                {
                    continue;
                }

                List<MatchQueueInfo> outMatchQueue = new List<MatchQueueInfo>();
                foreach (var player in players)
                {

                    //일단은 max값은 증가하지 않고 60이 최대로 변경한다
                    long count = Math.Clamp(++player.count, 1, mMaxExpandingRatingRangeTimeCount);

                    //Rating범위 내의 X명의 상대 플레이어를 찾는다, 오래있었는지 여부에 따라 범위값이 증가한다
                    //e.g.) rating=1700, deviation=200일 경우 플레이어의 능력이 구간[1500 ~ 1900]에 있다고 95% 확신 
                    
                    //내가 하고 싶은것이 일단 저 능력구간을 최대한 맞춰주고
                    //진짜 안잡힌다면 deviation을 높여줌으로써 구간을 늘려주도록 하고 싶음

                    double maxExpanding = player.deviation;
                    double expanding    = Math.Clamp(maxExpanding / (mMaxExpandingRatingRangeTimeCount / count), 0, maxExpanding);

                    double maxRating = player.rating + expanding;
                    double minRating = player.rating - expanding;

                    //Console.WriteLine($"{player.uid}[{count}] : max:{maxRating}, min:{minRating}");

                    var findResult = await mMatchingQueue.FindMatch(minRating, maxRating, mMaxPlayerNumber);
                    if (findResult.Item1 == WebErrorCode.FindMatchFailFindPossibleOpponents)
                    {
                        //상대를 못찾았을 경우
                        player.count = count;
                        await mMatchingQueue.UpdatePlayer(player);
                        continue;
                    }
                    else if(findResult.Item1 != WebErrorCode.None)
                    {
                        continue;
                    }

                    //10명의 비슷한 플레이어들 선별
                    var matchPlayers = findResult.Item2;
                    if (matchPlayers == null)
                    {
                        continue;
                    }

                    //TODO : 지정된 방으로 보낼수 있도록 한다
                    Console.WriteLine($"MatchSuccess Rating({minRating} ~ {maxRating})");
                    Console.WriteLine("{");

                    foreach (var matchPlayer in matchPlayers)
                    {
                        Console.WriteLine($"\t ID:{matchPlayer.uid}\tCNT:{matchPlayer.count}\tRating:{matchPlayer.rating}\tTime:{matchPlayer.time}");
                    }

                    Console.WriteLine("}");

                    //범위에 일치하는 X명을 찾았다면 대기열에서 제거한다
                    var removeResult = await mMatchingQueue.RemovePlayers(matchPlayers);
                    if(removeResult.Item2 == false)
                    {
                        continue;
                    }

                }

                if(outMatchQueue.Count > 0)
                {
                    await mMatchingQueue.RemovePlayers(outMatchQueue.ToArray());
                }

                stopwatch.Stop();
                TimeSpan elapsed = TimeSpan.FromSeconds(mDelaySecond) - stopwatch.Elapsed;
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
