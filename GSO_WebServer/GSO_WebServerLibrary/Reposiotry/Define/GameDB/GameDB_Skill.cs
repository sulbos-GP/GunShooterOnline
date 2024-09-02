using GSO_WebServerLibrary;
using GSO_WebServerLibrary.Models.GameDB;
using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<UserSkillInfo?> GetUserSkillByUid(Int32 uid)
        {
            return await mQueryFactory.Query("user_skill")
                .Where("uid", uid).
                FirstOrDefaultAsync<UserSkillInfo>();
        }

        public async Task<int> UpdateUserSkill(Int32 uid, UserSkillInfo skill)
        {
            return await mQueryFactory.Query("user_skill")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    rating      = skill.rating,
                    deviation   = skill.deviation,
                    volatility  = skill.volatility,
                });
        }

    }
}