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

namespace Matchmaker.Service
{
    public class MatchmakerService : IMatchmakerService
    {
        private readonly IMatchQueue mMatchQueueMDB;
        private readonly IGameDB mGameDB;
        private readonly IHubContext<MatchmakerHub> mMatchmakerHub;

        public MatchmakerService(IMatchQueue matchQueue, IGameDB gameDB, IHubContext<MatchmakerHub> matchmakerHubContext)
        {
            mMatchQueueMDB = matchQueue;
            mGameDB = gameDB;
            mMatchmakerHub = matchmakerHubContext;
        }

        public void Dispose()
        {
        }

        public async Task<WebErrorCode> AddMatchTicket(Int32 uid, String clientId)
        {
            try
            {
                WebErrorCode error = await mMatchQueueMDB.AddMatchTicket(uid, clientId);
                if (error != WebErrorCode.None)
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

        public async Task<WebErrorCode> RemoveMatchTicket(String clientId)
        {
            try
            {
                var matchTickets = await mMatchQueueMDB.GetAllMatchTicket();
                if(matchTickets == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }
                string uidStr = matchTickets.FirstOrDefault(r => r.Value.client_id == clientId).Key;
                if (uidStr == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                WebErrorCode error = await mMatchQueueMDB.RemoveMatchTicket(KeyUtils.GetUID(uidStr));
                if (error != WebErrorCode.None)
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

        public async Task<WebErrorCode> PushMatchQueue(Int32 uid, String world, String region)
        {
            try
            {

                UserSkillInfo? skill = await mGameDB.GetUserSkillByUid(uid);
                if(skill == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                WebErrorCode error = await mMatchQueueMDB.AddMatchRating(uid, skill.rating);
                if (error != WebErrorCode.None)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                var ticket = await mMatchQueueMDB.GetPlayerTicket(uid);
                if (ticket == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                {
                    ticket.world = world;
                    ticket.region = region;
                    ticket.match_start_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                }

                bool result = await mMatchQueueMDB.UpdateTicket(uid, ticket);
                if (result == true)
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

        public async Task<WebErrorCode> PopMatchQueue(Int32 uid)
        {
            try
            {
                WebErrorCode error = await mMatchQueueMDB.RemoveMatchRating(uid);
                if (error != WebErrorCode.None)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                var ticket = await mMatchQueueMDB.GetPlayerTicket(uid);
                if (ticket == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                {
                    ticket.world = string.Empty;
                    ticket.region = string.Empty;
                    ticket.match_start_time = 0;
                }

                bool result = await mMatchQueueMDB.UpdateTicket(uid, ticket);
                if (result == true)
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


        public async Task<WebErrorCode> RemoveMatchQueue(Int32 uid)
        {
            try
            {
                WebErrorCode error = await mMatchQueueMDB.RemoveMatchRating(uid);
                if (error != WebErrorCode.None)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                error = await mMatchQueueMDB.RemoveMatchTicket(uid);
                if (error != WebErrorCode.None)
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

        public async Task<WebErrorCode> RemoveMatchQueue(String[] keys)
        {
            try
            {
                foreach (String key in keys)
                {

                    int uid = KeyUtils.GetUID(key);

                    WebErrorCode error = await mMatchQueueMDB.RemoveMatchRating(uid);
                    if (error != WebErrorCode.None)
                    {
                        return WebErrorCode.TEMP_ERROR;
                    }

                    error = await mMatchQueueMDB.RemoveMatchTicket(uid);
                    if (error != WebErrorCode.None)
                    {
                        return WebErrorCode.TEMP_ERROR;
                    }
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<(WebErrorCode, Dictionary<int, PlayerInfo>?)> ScanPlayers()
        {
            try
            {
                var players = await mMatchQueueMDB.GetAllMatchRating();
                var tickets = await mMatchQueueMDB.GetAllMatchTicket();

                if (players == null || tickets == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                var playerInfos = new Dictionary<int, PlayerInfo>();
                foreach (var player in players)
                {

                    string key = player.Value;
                    double rating = player.Score;

                    tickets.TryGetValue(key, out var ticket);
                    if (ticket == null)
                    {
                        //티켓이 없는 경우
                        continue;
                    }


                    int uid = KeyUtils.GetUID(key);
                    string lockKey = KeyUtils.MakeKey(KeyUtils.EKey.MATCHLock, uid);

                    PlayerInfo playerInfo = new PlayerInfo();
                    playerInfo.rating = rating;
                    playerInfo.ticket = ticket;

                    playerInfos.Add(uid, playerInfo);

                }

                return (WebErrorCode.None, playerInfos);

            }
            catch
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> LockPlayers(String clientId)
        {
            try
            {

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<WebErrorCode> UnLockPlayers(String clientId)
        {
            try
            {

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<(WebErrorCode, Dictionary<string, TicketInfo>?)> FindMatchByRating(double min, double max, int capacity)
        {
            try
            {
                string[]? keys = await mMatchQueueMDB.SearchPlayerByRange(min, max);
                if(keys == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                var tickets = await mMatchQueueMDB.GetPlayerTickets(keys);
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

                var ticket = await mMatchQueueMDB.GetPlayerTicket(uid);
                if (ticket == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                {
                    ticket.latency = latency;
                }

                var result = await mMatchQueueMDB.UpdateTicket(uid, ticket);
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

        public async Task<WebErrorCode> NotifyMatchSuccess(TicketInfo[] tickets, MatchProfile profile)
        {
            try
            {
                Console.WriteLine("\tMatchPlayers");
                Console.WriteLine("\t{");
                foreach (var ticket in tickets)
                {
                    Console.WriteLine($"\t\tClientId : {ticket.client_id}");
                    await mMatchmakerHub.Clients.Client(ticket.client_id).SendAsync("S2C_MatchComplete", ticket.client_id, profile);
                }
                Console.WriteLine("\t}");

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

    }
}
