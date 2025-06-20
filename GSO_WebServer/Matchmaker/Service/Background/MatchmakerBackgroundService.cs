﻿using System.Diagnostics;
using Matchmaker.Service.Interfaces;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.Error;
using GSO_WebServerLibrary.Utils;
using WebCommonLibrary.Enum;

namespace Matchmaker.Service.Background
{
    public class MatchmakerBackgroundService : BackgroundService
    {
        private PeriodicTimer?  mTimer = null;
        private const double    mPeriodicMilliseconds = 1000;               //1초마다 매칭 검색
        private const int       mPlayerCapacity = 1;                        //최대 매치될 인원
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
            mTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(mPeriodicMilliseconds));
            while (await mTimer.WaitForNextTickAsync(stoppingToken))
            {
                stopwatch.Restart();

                //플레이어들의 상태를 체크한다
                var leavePlayers = await mMatchmakerService.CheckPlayersLeavingQueue();
                if (leavePlayers != null && leavePlayers.Count != 0)
                {
                    foreach (var (leaveKey, leaveTicket) in leavePlayers)
                    {
                        await mMatchmakerService.NotifyMatchFailed(leaveTicket, WebErrorCode.PopPlayersExitSuccess);

                        await mMatchmakerService.RemoveMatchQueue(leaveKey);
                    }
                }

                //가장 오래 기다린 플레이어를 선택한다
                var (error, longestPlayer) = await mMatchmakerService.GetLongestWaitingPlayer();
                if (error != WebErrorCode.None || longestPlayer == null || longestPlayer.ticket == null)
                {
                    continue;
                }

                double longestPlayerRating = longestPlayer.rating;
                Ticket longestPlayerticket = longestPlayer.ticket;

                //현재 시간에서 매치 시작한 시간 나눔
                var nowTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                long elapsedSeconds = nowTimestamp - longestPlayerticket.match_start_time;
                long count = elapsedSeconds / mExpandingRatingRangeTimeCount;

                //카운트 만큼 범위를 넓혀줌
                double expanding = mExpandingRatingRange * count;
                double maxRating = Math.Min(longestPlayerRating + expanding, mMaxRating);
                double minRating = Math.Max(longestPlayerRating - expanding, mMinRating);

                //일단은 max값은 증가하지 않고 60이 최대로 변경한다
                //Rating범위 내의 X명의 상대 플레이어를 찾는다, 오래있었는지 여부에 따라 범위값이 증가한다
                //e.g.) rating=1700, deviation=200일 경우 플레이어의 능력이 구간[1500 ~ 1900]에 있다고 95% 확신 

                //내가 하고 싶은것이 일단 저 능력구간을 최대한 맞춰주고
                //진짜 안잡힌다면 deviation을 높여줌으로써 구간을 늘려주도록 하고 싶음
                //Console.WriteLine($"{player.uid}[{count}] : max:{maxRating}, min:{minRating}");

                //Capacity명의 비슷한 플레이어들 선별
                (error, var matchedPlayers) = await mMatchmakerService.FindMatchByRating(minRating, maxRating, mPlayerCapacity);
                if (error != WebErrorCode.None || matchedPlayers == null || matchedPlayers.Count != mPlayerCapacity)
                {
                    if(matchedPlayers != null)
                    {
                        foreach (var matchedPlayer in matchedPlayers)
                        {
                            await mMatchmakerService.RollbackTicket(KeyUtils.GetUID(matchedPlayer.Key));
                        }
                    }
                    continue;
                }

                //방 있는지 확인
                (error, var profile) = await mGameServerManagerService.FetchMatchInfo(matchedPlayers);
                if(error != WebErrorCode.None || profile == null)
                {
                    foreach (var matchedPlayer in matchedPlayers)
                    {
                        await mMatchmakerService.RollbackTicket(KeyUtils.GetUID(matchedPlayer.Key));
                    }
                    continue;
                }

                //방이 잡힌 상태
                error = await mMatchmakerService.MatchConfirmation(matchedPlayers);
                if (error != WebErrorCode.None)
                {
                    continue;
                }

                error = await mGameServerManagerService.DispatchMatchPlayers(matchedPlayers, profile);
                if (error != WebErrorCode.None)
                {
                    foreach (var matchedPlayer in matchedPlayers)
                    {
                        await mMatchmakerService.RollbackTicket(KeyUtils.GetUID(matchedPlayer.Key));
                    }
                    continue;
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
