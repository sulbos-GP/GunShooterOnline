using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using Matchmaker.Repository.Interface;
using GSO_WebServerLibrary.Utils;
using StackExchange.Redis;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.Config;
using WebCommonLibrary.Error;
using Microsoft.ML.Probabilistic.Collections;

namespace Matchmaker.Repository
{
    public partial class MatchQueue : IMatchQueue
    {
        public async Task ClearRating()
        {
            var keys = await mMatchRating.SortAsync();
            if(keys == null)
            {
                return;
            }

            foreach (var key in keys)
            {
                await RemoveRating(KeyUtils.GetUID(key));
            }
        }

        public async Task<bool> IsValidRatingWithUid(Int32 uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.Rating, uid);
            var ratings = await mMatchRating.RangeByRankAsync();
            return ratings.FindIndex(r => r == key) == -1 ? false : true;
        }

        public async Task<double?> GetRatingWithKey(string key)
        {
            return await mMatchRating.ScoreAsync(key);
        }

        public async Task<WebErrorCode> AddRating(Int32 uid, Double rating)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.Rating, uid);

            var reuslt = await mMatchRating.AddAsync(key, rating, null, When.NotExists);
            if (reuslt == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> RemoveRating(Int32 uid)
        {
            string key = KeyUtils.MakeKey(KeyUtils.EKey.Rating, uid);

            bool result = await mMatchRating.RemoveAsync(key);
            if (result == false)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            return WebErrorCode.None;
        }
    }
}
