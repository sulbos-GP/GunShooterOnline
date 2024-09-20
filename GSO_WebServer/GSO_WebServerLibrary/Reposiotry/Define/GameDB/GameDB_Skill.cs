using GSO_WebServerLibrary;
using WebCommonLibrary.Models.GameDB;
using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using System.Data;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<UserSkillInfo?> GetUserSkillByUid(Int32 uid, IDbTransaction? transaction)
        {
            return await mQueryFactory.Query("user_skill")
                .Where("uid", uid).
                FirstOrDefaultAsync<UserSkillInfo>(transaction);
        }

        public async Task<int> UpdateUserSkill(Int32 uid, UserSkillInfo skill, IDbTransaction? transaction)
        {
            return await mQueryFactory.Query("user_skill")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    rating      = skill.rating,
                    deviation   = skill.deviation,
                    volatility  = skill.volatility,
                }, transaction);
        }

    }
}