using WebCommonLibrary.Models.GameDB;
using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.Models.GameDatabase;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<IEnumerable<FUserLevelReward>?> GetUserLevelRewardByUid(int uid, bool? received, int? reward_level_id)
        {
            var where = new Dictionary<string, object>()
            {
                { "uid"  , uid },
            };

            if (received != null) { where.Add("received", received); }

            if (reward_level_id != null) { where.Add("reward_level_id", reward_level_id); }

            return await mQueryFactory.Query("user_level_reward")
                .Where(where).
                GetAsync<FUserLevelReward>();
        }
    }
}