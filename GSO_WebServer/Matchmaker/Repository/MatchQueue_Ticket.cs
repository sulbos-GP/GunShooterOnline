using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using Matchmaker.Repository.Interface;
using GSO_WebServerLibrary.Utils;
using StackExchange.Redis;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.Config;
using WebCommonLibrary.Error;
using WebCommonLibrary.Enum;

namespace Matchmaker.Repository
{
    public partial class MatchQueue : IMatchQueue
    {
        public async Task ClearTicket()
        {
            var (error, tickets) = await GetAllTicket();
            if(error != WebErrorCode.None || tickets == null)
            {
                return;
            }

            foreach (var ticket in tickets)
            {
                await RemoveTicket(KeyUtils.GetUID(ticket.Key));
            }
        }

        public async Task<(WebErrorCode, int)> GetUidWithClientId(string clientId)
        {
            var (error, tickets) = await GetAllTicket();
            if (error != WebErrorCode.None || tickets == null)
            {
                return (error, 0);
            }

            string uidStr = tickets.FirstOrDefault(r => r.Value.client_id == clientId).Key;
            if (uidStr == null)
            {
                return (WebErrorCode.TEMP_ERROR, 0);
            }

            return (WebErrorCode.None, KeyUtils.GetUID(uidStr));
        }

        public async Task<(WebErrorCode, Ticket?)> GetTicketWithUid(Int32 uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);

            var (error, ticket) = await GetTicketWithKey(key);
            if (error != WebErrorCode.None || ticket == null)
            {
                return (WebErrorCode.TEMP_ERROR, null);
            }

            return (WebErrorCode.None, ticket);
        }

        public async Task<(WebErrorCode, Ticket?)> GetTicketWithKey(string key)
        {
            var result = await mMatchTickets.GetAsync(key);
            if (false == result.HasValue)
            {
                return (WebErrorCode.TEMP_ERROR, null);
            }

            return (WebErrorCode.None, result.Value);
        }

        public async Task<(WebErrorCode, Dictionary<int, Ticket>?)> GetTicketsWithUids(Int32[] uids)
        {
            Dictionary<int, Ticket> tickets = new Dictionary<int, Ticket>();
            foreach (int uid in uids)
            {
                var(error, ticket) = await GetTicketWithUid(uid);
                if (error != WebErrorCode.None || ticket == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }
                tickets.Add(uid, ticket);
            }
            return (WebErrorCode.None, tickets);
        }

        public async Task<(WebErrorCode, Dictionary<string, Ticket>?)> GetTicketsWithKeys(string[] keys)
        {
            Dictionary<string, Ticket> tickets = new Dictionary<string, Ticket>();
            foreach (string key in keys)
            {
                var (error, ticket) = await GetTicketWithKey(key);
                if (error != WebErrorCode.None || ticket == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }
                tickets.Add(key, ticket);
            }
            return (WebErrorCode.None, tickets);
        }

        public async Task<(WebErrorCode, Ticket?)> GetTicketWithClientId(string clientId)
        {
            var (error, tickets) = await GetAllTicket();
            if(error != WebErrorCode.None || tickets == null)
            {
                return (error, null);
            }

            string uidStr = tickets.FirstOrDefault(r => r.Value.client_id == clientId).Key;
            if (uidStr == null)
            {
                return (WebErrorCode.TEMP_ERROR, null);
            }

            (error, var ticket) = await GetTicketWithUid(KeyUtils.GetUID(uidStr));
            if (error != WebErrorCode.None || ticket == null)
            {
                return (error, null);
            }

            return (WebErrorCode.None, ticket);
        }

        public async Task<(WebErrorCode, Dictionary<string, Ticket>?)> GetAllTicket()
        {
            var tickets = await mMatchTickets.GetAllAsync();
            if(tickets == null)
            {
                return (WebErrorCode.TEMP_ERROR, null);
            }

            return (WebErrorCode.None, tickets);
        }

        public async Task<WebErrorCode> AddTicket(Int32 uid, string clientId)
        {
            Ticket ticket = new Ticket
            {
                client_id = clientId,
                state = ETicketState.NotInQueue,
            };

            bool reuslt = await SetTicket(uid, ticket);
            if (reuslt == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> RemoveTicket(Int32 uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);

            bool reuslt = await mMatchTickets.DeleteAsync(key);
            if (reuslt == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<bool> SetTicket(Int32 uid, Ticket ticket)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);
            return await mMatchTickets.SetAsync(key, ticket);
        }

    }
}
