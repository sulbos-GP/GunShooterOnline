using GSO_WebServerLibrary;
using WebCommonLibrary.Models.GameDB;
using SqlKata.Execution;
using System.Data;
using GSO_WebServerLibrary.Reposiotry.Interfaces;

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
                    recent_login_dt = DateTime.UtcNow,
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

            var reward_id = mMasterDB.Context.MasterRewardLevel.FirstOrDefault(r => r.Value.level == level).Key;
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

        public async Task<int> RecivedLevelReward(int uid, int level, bool received, IDbTransaction? transaction)
        {

            var reward_id = mMasterDB.Context.MasterRewardLevel.FirstOrDefault(r => r.Value.level == level).Key;
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
                }, transaction);
        }
    }
}
