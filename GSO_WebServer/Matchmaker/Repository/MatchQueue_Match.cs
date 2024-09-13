using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using Matchmaker.Repository.Interface;
using GSO_WebServerLibrary.Utils;
using StackExchange.Redis;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.Config;
using WebCommonLibrary.Error;

namespace Matchmaker.Repository
{
    public partial class MatchQueue : IMatchQueue
    {

        public async Task<string[]?> SearchPlayerByRange(Double min, Double max)
        {
            return await mMatchRating.RangeByScoreAsync(min, max);
        }
        
    }
}
