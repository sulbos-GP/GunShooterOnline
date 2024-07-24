using GsoWebServer.Reposiotry.Interfaces;
using GSO_WebServerLibrary;
using GsoWebServer.Models.GameDB;
using SqlKata.Execution;

namespace GsoWebServer.Reposiotry.RDB.Game
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