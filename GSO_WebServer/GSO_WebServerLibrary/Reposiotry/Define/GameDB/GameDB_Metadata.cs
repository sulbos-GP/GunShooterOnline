using WebCommonLibrary.Models.GameDB;
using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<UserMetadataInfo?> GetUserMetadataByUid(Int32 uid)
        {
            return await mQueryFactory.Query("user_metadata")
                .Where("uid", uid).
                FirstOrDefaultAsync<UserMetadataInfo>();
        }

        public async Task<int> UpdateUserMetadata(Int32 uid, UserMetadataInfo matadata)
        {
            return await mQueryFactory.Query("user_metadata")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    total_games     = matadata.total_games,
                    kills           = matadata.kills,
                    deaths          = matadata.deaths,
                    damage          = matadata.damage,
                    farming         = matadata.farming,
                    escape          = matadata.escape,
                    survival_time   = matadata.survival_time,
                });
        }
    }
}