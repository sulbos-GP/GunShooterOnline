using GsoWebServer.Reposiotry.Interfaces;
using GSO_WebServerLibrary;
using SqlKata.Execution;
using System.Data;

namespace GsoWebServer.Reposiotry.RDB.Game
{
    public partial class GameDB : IGameDB
    {
        /// <summary>
        /// 유저 신규 회원가입
        /// </summary>
        public async Task<int> SingUp(String playerId, String service, String refresh_token, IDbTransaction transaction)
        {
            return await mQueryFactory.Query("user")
                .InsertGetIdAsync<int>(new
                {
                    player_id = playerId,
                    service = service,
                    refresh_token = refresh_token
                }, transaction);
        }

        /// <summary>
        /// 유저 메타데이터 초기화
        /// </summary>
        public async Task<int> InitUserMatadata(int uid, IDbTransaction transaction)
        {
            return await mQueryFactory.Query("user_metadata").
                InsertAsync(new
                {
                    uid = uid
                }, transaction);
        }

        /// <summary>
        /// 유저 스킬(레이팅) 초기화
        /// </summary>
        public async Task<int> InitUserSkill(int uid, IDbTransaction transaction)
        {
            return await mQueryFactory.Query("user_skill").
                InsertAsync(new
                {
                    uid = uid
                }, transaction);
        }
    }
}
