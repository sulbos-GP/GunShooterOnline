using Docker.DotNet.Models;
using GameServerManager.Repository;
using GameServerManager.Repository.Interfaces;
using GameServerManager.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameServerManager.Servicies
{
    public class SessionBackgroundService : BackgroundService
    {
        private PeriodicTimer? mTimer = null;
        private const double mInitialDelaySeconds = 10;                     //10초 이후에 체크
        private const double mHealthDelaySecond = 5;                        //5초마다 실행
        private const int    mMaxRetries = 3;                               //3번 이상 타임 아웃

        private const double mPeriodSecond = 1;                             //5초마다 실행

        private readonly IDockerService mDockerService;
        private readonly ISessionMemory mSessionMemory;

        public SessionBackgroundService(ISessionMemory sessionMemory, IDockerService dockerService)
        {
            mDockerService = dockerService;
            mSessionMemory = sessionMemory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            mTimer = new PeriodicTimer(TimeSpan.FromSeconds(mPeriodSecond));

            while (await mTimer.WaitForNextTickAsync(stoppingToken))
            {
                stopwatch.Restart();

                //모든 컨테이너 불러오기
                var matchStatus = await mSessionMemory.GetAllMatchStatus();
                if (matchStatus == null)
                {
                    continue;
                }

                //현재 Allocate 상태만 따로 빼기
                var allocateMatchs = matchStatus
                    .Where(status => status.Value.state == Models.EMatchState.Allocated)
                    .ToDictionary(status => status.Key, status => status.Value);

                if (allocateMatchs == null)
                {
                    continue;
                }

                long nowAge = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                foreach (var match in allocateMatchs)
                {

                    long elapsedSeconds = nowAge - match.Value.age;

                    //초기화 시간이 지났는지 확인
                    if(elapsedSeconds < mInitialDelaySeconds)
                    {
                        continue;
                    }

                    //Health 간격이 되었는지
                    if (elapsedSeconds % mHealthDelaySecond != 0)
                    {
                        continue;
                    }

                    //Health 체크 하기
                    var isHealthy = await mDockerService.CheckContainerHealth(match.Key);
                    if(isHealthy == true)
                    {
                        match.Value.retries = 0;
                        await mSessionMemory.UpdateMatchStatus(match.Key, match.Value);
                        continue;
                    }

                    if(mMaxRetries != ++match.Value.retries)
                    {
                        await mSessionMemory.UpdateMatchStatus(match.Key, match.Value);
                        continue;
                    }

                    //MAX를 넘기면 재시작 해버린다
                    await mDockerService.RestartContainer(match.Key);

                }

                stopwatch.Stop();
                TimeSpan elapsed = TimeSpan.FromSeconds(mPeriodSecond) - stopwatch.Elapsed;
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
