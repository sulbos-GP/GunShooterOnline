using GSO_WebServerLibrary;
using WebCommonLibrary.Models.GameDB;
using Google.Apis.Games.v1.Data;
using SqlKata.Execution;
using static Google.Apis.Requests.RequestError;
using System.Transactions;
using System.Data;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.Models.MasterDB;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {

        public async Task<UserInfo?> GetUserByPlayerId(String playerId)
        {
            return await mQueryFactory.Query("user")
                .Where("player_id", playerId).
                FirstOrDefaultAsync<UserInfo>();
        }

        public async Task<UserInfo?> GetUserByUid(int uid, IDbTransaction? transaction = null)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid).
                FirstOrDefaultAsync<UserInfo>(transaction);
        }

        public async Task<UserInfo?> GetUserByNickname(String nickname)
        {
            return await mQueryFactory.Query("user")
                .Where("nickname", nickname).
                FirstOrDefaultAsync<UserInfo>();
        }
        
        public async Task<int> UpdateRecentLogin(int uid)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    recent_login_dt = DateTime.Now,
                });
        }

        public async Task<int> UpdateNickname(int uid, String nickname)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    nickname = nickname,
                });
        }

        public async Task<int> UpdateLevel(int uid, int experience, IDbTransaction? transaction = null)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    experience = experience,
                }, transaction);
        }

        public async Task<int> InsertLevelReward(int uid, int level, IDbTransaction? transaction = null)
        {

            Dictionary<int, DB_RewardLevel> rewards = mMasterDB.GetRewardLevelList();
            int reward_id = rewards.FirstOrDefault(r => r.Value.level == level).Key;
            if (reward_id == 0)
            {
                return 0;
            }

            return await mQueryFactory.Query("user_level_reward").
                InsertAsync(new
                {
                    uid = uid,
                    reward_id = reward_id,
                }, transaction);
        }

        public async Task<int> UpdateLevelReward(int uid, int level, bool received)
        {

            Dictionary<int, DB_RewardLevel> rewards = mMasterDB.GetRewardLevelList();
            int reward_id = rewards.FirstOrDefault(r => r.Value.level == level).Key;
            if (reward_id == 0)
            {
                return 0;
            }

            var where = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "reward_id"  , reward_id },
            };

            return await mQueryFactory.Query("user_level_reward")
                .Where(where)
                .UpdateAsync(new
                {
                    received = received,
                    received_dt = DateTime.Now,
                });
        }
    }
}
