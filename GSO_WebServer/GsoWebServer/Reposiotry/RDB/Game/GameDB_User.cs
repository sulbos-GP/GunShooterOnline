using GsoWebServer.Reposiotry.Interfaces;
using GSO_WebServerLibrary;
using GsoWebServer.Models.GameDB;
using Google.Apis.Games.v1.Data;
using SqlKata.Execution;
using static Google.Apis.Requests.RequestError;
using System.Transactions;
using System.Data;

namespace GsoWebServer.Reposiotry.RDB.Game
{
    public partial class GameDB : IGameDB
    {

        public async Task<UserInfo?> GetUserByPlayerId(String playerId)
        {
            return await mQueryFactory.Query("user")
                .Where("player_id", playerId).
                FirstOrDefaultAsync<UserInfo>();
        }

        public async Task<UserInfo?> GetUserByUid(int uid)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid).
                FirstOrDefaultAsync<UserInfo>();
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
    
    }
}
