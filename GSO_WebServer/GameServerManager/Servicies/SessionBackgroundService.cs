using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameServerManager.Servicies
{
    public class SessionBackgroundService : BackgroundService
    {
        private PeriodicTimer? mTimer = null;
        private const double mPeriodSecond = 1;                           //1초마다 실행

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            mTimer = new PeriodicTimer(TimeSpan.FromSeconds(mPeriodSecond));

            while (await mTimer.WaitForNextTickAsync(stoppingToken))
            {
                stopwatch.Restart();

                //DO SOMETHINGS

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
