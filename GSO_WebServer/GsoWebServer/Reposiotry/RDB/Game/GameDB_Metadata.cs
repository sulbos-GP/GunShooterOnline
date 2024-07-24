using GsoWebServer.Reposiotry.Interfaces;
using GSO_WebServerLibrary;
using GsoWebServer.Models.GameDB;
using SqlKata.Execution;
using static System.Net.Mime.MediaTypeNames;

namespace GsoWebServer.Reposiotry.RDB.Game
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