using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using Matchmaker.Repository.Interface;
using GSO_WebServerLibrary.Utils;
using StackExchange.Redis;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.Config;
using WebCommonLibrary.Error;
using System.Net.Sockets;
using Humanizer;
using static Humanizer.On;

namespace Matchmaker.Repository
{
    public partial class MatchQueue : IMatchQueue
    {
        public async Task ClearClient()
        {
            Dictionary<string, string> clients = await mMatchClient.GetAllAsync();
            if (clients == null || clients.Count == 0)
            {
                return;
            }

            foreach (var cleint in clients)
            {
                bool reuslt = await mMatchClient.DeleteAsync(cleint.Key);
                if (reuslt == false)
                {
                    return;
                }
            }
        }

        public async Task<string> GetClientId(int uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);
            var result = await mMatchClient.GetAsync(key);
            if (false == result.HasValue)
            {
                return string.Empty;
            }

            return result.Value;
        }

        public async Task<bool> AddClient(int uid, string connectionId)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);
            return await mMatchClient.SetAsync(key, connectionId);
        }

        public async Task<bool> RemoveClient(int uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.MATCH, uid);
            return await mMatchClient.DeleteAsync(key);
        }

        
    }
}
