﻿using WebCommonLibrary.Models.GameDB;
using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using System.Data;
using WebCommonLibrary.Models.GameDatabase;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<FUserMetadata?> GetUserMetadataByUid(Int32 uid, IDbTransaction? transaction)
        {
            return await mQueryFactory.Query("user_metadata")
                .Where("uid", uid).
                FirstOrDefaultAsync<FUserMetadata>(transaction);
        }

        public async Task<int> UpdateUserMetadata(Int32 uid, FUserMetadata matadata, IDbTransaction? transaction)
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
                }, transaction);
        }
    }
}