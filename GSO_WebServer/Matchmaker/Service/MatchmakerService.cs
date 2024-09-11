using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GSO_WebServerLibrary.Utils;
using Matchmaker.Hubs;
using WebCommonLibrary.Models.Match;
using Matchmaker.Repository.Interface;
using Matchmaker.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebCommonLibrary.Models.MasterDB;
using Matchmaker.Repository;
using WebCommonLibrary.Enum;
using Google.Apis.Games.v1.Data;

namespace Matchmaker.Service
{
    public class MatchmakerService : IMatchmakerService
    {
        private readonly IMatchQueue mMatchQueue;
        private readonly IGameDB mGameDB;
        private readonly IHubContext<MatchmakerHub> mMatchmakerHub;

        public MatchmakerService(IMatchQueue matchQueue, IGameDB gameDB, IHubContext<MatchmakerHub> matchmakerHubContext)
        {
            mMatchQueue = matchQueue;
            mGameDB = gameDB;
            mMatchmakerHub = matchmakerHubContext;
        }

        public void Dispose()
        {
        }

        public async Task ClearMatch()
        {
            await mMatchQueue.ClearTicket();
            await mMatchQueue.ClearRating();
        }

        public async Task<WebErrorCode> AddMatchTicket(Int32 uid, String clientId)
        {

            try
            {
                //mMatchQueue.GetRedisEventLock().TryLock();

                //이미 티켓이 존재할 경우
                var (error, ticket) = await mMatchQueue.GetTicketWithUid(uid);
                if (ticket != null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                error = await mMatchQueue.AddTicket(uid, clientId);
                if (error != WebErrorCode.None)
                {
                    return error;
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
            finally
            {
                //mMatchQueue.GetRedisEventLock().ReleaseLock();
            }
            
        }

        public async Task<WebErrorCode> RemoveMatchTicket(String clientId)
        {
            var (error, uid) = await mMatchQueue.GetUidWithClientId(clientId);
            if (uid == 0)
            {
                return error;
            }

            error = await mMatchQueue.RemoveTicket(uid);
            if (error != WebErrorCode.None)
            {
                return error;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> PushMatchQueue(Int32 uid, String world, String region)
        {
            (WebErrorCode error, Ticket? ticket) = await mMatchQueue.GetTicketWithUid(uid);
            if (ticket == null)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            UserSkillInfo? skill = await mGameDB.GetUserSkillByUid(uid);
            if(skill == null)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            bool valid = await mMatchQueue.IsValidRatingWithUid(uid);
            if(valid == true)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            error = await mMatchQueue.AddRating(uid, skill.rating);
            if (error != WebErrorCode.None)
            {
                return error;
            }

            {
                ticket.world = world;
                ticket.region = region;
                ticket.match_start_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                ticket.state = ETicketState.Join;
            }

            bool result = await mMatchQueue.SetTicket(uid, ticket);
            if (result == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> PopMatchQueue(Int32 uid)
        {

            (WebErrorCode error, Ticket? ticket) = await mMatchQueue.GetTicketWithUid(uid);
            if (ticket == null)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            if(ticket.state != ETicketState.InQueue || ticket.state != ETicketState.Join)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            error = await mMatchQueue.RemoveRating(uid);
            if (error != WebErrorCode.None)
            {
                return error;
            }

            {
                ticket.world = string.Empty;
                ticket.region = string.Empty;
                ticket.match_start_time = 0;
                ticket.state = ETicketState.NotInQueue;
            }

            bool result = await mMatchQueue.SetTicket(uid, ticket);
            if (result == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> RemoveMatchQueue(string key)
        {

            int uid = KeyUtils.GetUID(key);

            var error = await mMatchQueue.RemoveTicket(uid);
            if (error != WebErrorCode.None)
            {
                return error;
            }

            error = await mMatchQueue.RemoveRating(uid);
            if (error != WebErrorCode.None)
            {
                return error;
            }

            return WebErrorCode.None;
        }

        public async Task<(WebErrorCode, PlayerInfo?)> GetLongestWaitingPlayer()
        {
            var (error, tickets) = await mMatchQueue.GetAllTicket();
            if(error != WebErrorCode.None || tickets == null)
            {
                return (error, null);
            }

            var longestWaitingPlayer = tickets.OrderByDescending(p => p.Value.match_start_time).FirstOrDefault();

            string  key     = longestWaitingPlayer.Key;
            int     uid     = KeyUtils.GetUID(key);
            Ticket  ticket  = longestWaitingPlayer.Value;
            ticket.state    = ETicketState.WaitingForMatch;

            bool result = await mMatchQueue.SetTicket(uid, ticket);
            if (result == false)
            {
                return (WebErrorCode.TEMP_ERROR, null);
            }

            double? rating = await mMatchQueue.GetRatingWithKey(key);
            if (rating == null)
            {
                return (WebErrorCode.TEMP_ERROR, null);
            }

            var player = new PlayerInfo
            {
                rating = rating.Value,
                ticket = ticket
            };

            return (WebErrorCode.None, player);
        }

        public async Task<(WebErrorCode, Dictionary<string, Ticket>?)> FindMatchByRating(double min, double max, int capacity)
        {
            try
            {
                string[]? keys = await mMatchQueue.SearchPlayerByRange(min, max);
                if(keys == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                var (error, tickets) = await mMatchQueue.GetTicketsWithKeys(keys);
                if (tickets == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                if(tickets.Count < capacity)
                {
                    //매칭 인원수가 적다면
                    return (WebErrorCode.TEMP_ERROR, null);
                }
                else if (tickets.Count > capacity)
                {
                    //매치 시작한지 오래된 플레이어를 우선적으로 선별
                    tickets.OrderBy(ticket => ticket.Value.match_start_time).Take(capacity).ToDictionary();
                }

                foreach (var ticket in tickets)
                {
                    ticket.Value.state = ETicketState.WaitingForMatch;
                    await mMatchQueue.SetTicket(KeyUtils.GetUID(ticket.Key), ticket.Value);
                }

                return (WebErrorCode.None, tickets);
            }
            catch
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> UpdateLatency(Int32 uid, Int64 latency)
        {
            try
            {

                var (error, ticket) = await mMatchQueue.GetTicketWithUid(uid);
                if (error != WebErrorCode.None || ticket == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                {
                    ticket.latency = latency;
                }

                var result = await mMatchQueue.SetTicket(uid, ticket);
                if (result == false)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task NotifyMatchSuccess(Ticket ticket, MatchProfile profile)
        {
            Console.WriteLine($"\t\tClientId : {ticket.client_id}");
            await mMatchmakerHub.Clients.Client(ticket.client_id).SendAsync("S2C_MatchComplete", ticket.client_id, profile);

        }

    }
}
