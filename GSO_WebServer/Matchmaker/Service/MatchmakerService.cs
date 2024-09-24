using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GSO_WebServerLibrary.Utils;
using Matchmaker.Hubs;
using WebCommonLibrary.Models.Match;
using Matchmaker.Repository.Interface;
using Matchmaker.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Matchmaker.Repository;
using WebCommonLibrary.Enum;
using Google.Apis.Games.v1.Data;
using System.Threading;
using System.Net.Sockets;

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

        public async Task<WebErrorCode> AddMatchTicket(Int32 uid, string clientId)
        {

            try
            {
                await mMatchQueue.TryTakeLock(uid);

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
            catch (Exception e)
            {
                Console.WriteLine($"[MatchmakerService.AddMatchTicket] : {e.Message}");
                return WebErrorCode.TEMP_Exception;
            }
            finally
            {
                await mMatchQueue.ReleaseLock(uid);
            }
            
        }

        public async Task<WebErrorCode> RemoveMatchTicket(string clientId)
        {
            var (error, uid) = await mMatchQueue.GetUidWithClientId(clientId);
            if (uid == 0)
            {
                return error;
            }

            try
            {
                await mMatchQueue.TryTakeLock(uid);

                error = await mMatchQueue.RemoveTicket(uid);
                if (error != WebErrorCode.None)
                {
                    return error;
                }

                return WebErrorCode.None;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[MatchmakerService.RemoveMatchTicket] : {e.Message}");
                return WebErrorCode.TEMP_Exception;
            }
            finally
            {
                await mMatchQueue.ReleaseLock(uid);
            }
        }

        public async Task<WebErrorCode> PushMatchQueue(Int32 uid, String world, String region)
        {
            Console.WriteLine("[MatchmakerService.PushMatchQueue]");
            try
            {
                await mMatchQueue.TryTakeLock(uid);

                var user = await mGameDB.GetUserByUid(uid);
                if (user == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                if(user.ticket <= 0)
                {
                    //return WebErrorCode.PushPlayerNoTicket;
                }

                (WebErrorCode error, Ticket? ticket) = await mMatchQueue.GetTicketWithUid(uid);
                if (ticket == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                if (ticket.state != ETicketState.NotInQueue)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                UserSkillInfo? skill = await mGameDB.GetUserSkillByUid(uid);
                if (skill == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                bool valid = await mMatchQueue.IsValidRatingWithUid(uid);
                if (valid == true)
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
                    ticket.state = ETicketState.InQueue;
                    ticket.isExit = false;
                }

                bool result = await mMatchQueue.SetTicket(uid, ticket);
                //if (result == false)
                //{
                //    return WebErrorCode.TEMP_ERROR;
                //}

                return WebErrorCode.None;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[MatchmakerService.PushMatchQueue] : {e.Message}");
                return WebErrorCode.TEMP_Exception;
            }
            finally
            {
                await mMatchQueue.ReleaseLock(uid);
            }
        }

        public async Task<WebErrorCode> PopMatchQueue(Int32 uid)
        {
            Console.WriteLine("[MatchmakerService.PopMatchQueue]");

            try
            {
                await mMatchQueue.TryTakeLock(uid);
                (WebErrorCode error, Ticket? ticket) = await mMatchQueue.GetTicketWithUid(uid);
                if (ticket == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                {
                    ticket.isExit = true;
                }
             
                bool result = await mMatchQueue.SetTicket(uid, ticket);
                //if (result == false)
                //{
                //    return WebErrorCode.TEMP_ERROR;
                //}

                return WebErrorCode.PopPlayersExitRequested;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[MatchmakerService.PopMatchQueue] : {e.Message}");
                return WebErrorCode.TEMP_Exception;
            }
            finally
            {
                await mMatchQueue.ReleaseLock(uid);
            }
        }

        public async Task<WebErrorCode> RemoveMatchQueue(string key)
        {
            Console.WriteLine("[MatchmakerService.RemoveMatchQueue]");
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

            await mMatchQueue.ReleaseLock(uid);

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> DisconnectMatch(string clientId)
        {
            var (error, uid) = await mMatchQueue.GetUidWithClientId(clientId);
            if (uid == 0)
            {
                return error;
            }

            try
            {
                await mMatchQueue.TryTakeLock(uid);

                (error, Ticket? ticket) = await mMatchQueue.GetTicketWithUid(uid);
                if (ticket == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                {
                    ticket.isExit = true;
                }

                bool result = await mMatchQueue.SetTicket(uid, ticket);

                error = await mMatchQueue.RemoveTicket(uid);
                if (error != WebErrorCode.None)
                {
                    return error;
                }

                return WebErrorCode.None;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[MatchmakerService.RemoveMatchTicket] : {e.Message}");
                return WebErrorCode.TEMP_Exception;
            }
            finally
            {
                await mMatchQueue.ReleaseLock(uid);
            }
        }

        public async Task<WebErrorCode> LeavingMatchQueue(string key)
        {
            Console.WriteLine("[MatchmakerService.LeavingMatchQueue]");
            int uid = KeyUtils.GetUID(key);

            var (error , ticket) = await mMatchQueue.GetTicketWithUid(uid);
            if(error != WebErrorCode.None || ticket == null)
            {
                return error;
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
                ticket.isExit = false;
            }

            bool result = await mMatchQueue.SetTicket(uid, ticket);
            //if (result == false)
            //{
            //    return WebErrorCode.TEMP_ERROR;
            //}

            return WebErrorCode.None;
        }

        public async Task<Dictionary<string, Ticket>?> CheckPlayersLeavingQueue()
        {
            var (error, tickets) = await mMatchQueue.GetAllTicket();
            if (error != WebErrorCode.None || tickets == null || tickets.Count == 0)
            {
                return null;
            }

            return tickets.Where(kvp => kvp.Value.isExit == true).ToDictionary();
        }

        public async Task<(WebErrorCode, PlayerInfo?)> GetLongestWaitingPlayer()
        {
            while (true)
            {
                var (error, tickets) = await mMatchQueue.GetAllTicket();
                if (error != WebErrorCode.None || tickets == null || tickets.Count == 0)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                var longestWaitingPlayer = tickets.
                                            Where(kvp => kvp.Value.state == ETicketState.InQueue && kvp.Value.isExit != true).
                                            OrderByDescending(p => p.Value.match_start_time).
                                            FirstOrDefault();

                if(longestWaitingPlayer.Value == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                string  key = longestWaitingPlayer.Key;
                int     uid = KeyUtils.GetUID(key);

                if (false == await mMatchQueue.TryTakeLock(uid, false))
                {
                    continue;
                }

                Ticket ticket = longestWaitingPlayer.Value;
                ticket.state = ETicketState.WaitingForMatch;

                bool result = await mMatchQueue.SetTicket(uid, ticket);
                //if (result == false)
                //{
                //    return (WebErrorCode.TEMP_ERROR, null);
                //}

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

                await mMatchQueue.ReleaseLock(uid);

                return (WebErrorCode.None, player);
            }
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
                    return (WebErrorCode.TEMP_ERROR, tickets);
                }
                else if (tickets.Count > capacity)
                {
                    //매치 시작한지 오래된 플레이어를 우선적으로 선별
                    var matchPlayers = tickets.
                        Where(kvp => kvp.Value.state == ETicketState.InQueue && kvp.Value.isExit != true).
                        OrderBy(ticket => ticket.Value.match_start_time).
                        Take(capacity).
                        ToDictionary();
                }

                foreach (var ticket in tickets)
                {
                    int uid = KeyUtils.GetUID(ticket.Key);
                    await mMatchQueue.TryTakeLock(uid);

                    ticket.Value.state = ETicketState.WaitingForMatch;
                    await mMatchQueue.SetTicket(uid, ticket.Value);
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
                //if (result == false)
                //{
                //    return WebErrorCode.TEMP_ERROR;
                //}

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
            await mMatchmakerHub.Clients.Client(ticket.client_id).SendAsync("S2C_MatchSuccess", ticket.client_id, profile);

        }

        public async Task NotifyMatchFailed(Ticket ticket, WebErrorCode error)
        {
            await mMatchmakerHub.Clients.Client(ticket.client_id).SendAsync("S2C_MatchFailed", error);
        }

        public async Task RollbackTicket(Int32 uid, Ticket ticket)
        {
            Console.WriteLine("[MatchmakerService.RollbackTicket]");

            await mMatchQueue.ReleaseLock(uid);

            await mMatchQueue.SetTicket(uid, ticket);
        }

    }
}
