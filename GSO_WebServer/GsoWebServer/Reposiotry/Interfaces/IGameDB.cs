using GSO_WebServerLibrary;
using GsoWebServer.Models.GameDB;
using System.Data;

namespace GsoWebServer.Reposiotry.Interfaces
{
    public interface IGameDB : IDisposable
    {
        //GameDB
        public IDbConnection GetConnection();

        //GameDB_Init
        public Task<int> SingUp(String playerId, String service, IDbTransaction transaction);
        public Task<int> InitUserMatadata(int uid, IDbTransaction transaction);
        public Task<int> InitUserSkill(int uid, IDbTransaction transaction);

        //GameDB_Game

        //GameDB_Metadata

        //GameDB_Skill

        //GameDB_User

        /// <summary>
        /// 유저의 플레이어 아이디로 user정보 불러오기
        /// </summary>
        public Task<UserInfo?> GetUserByPlayerId(String playerId);

        /// <summary>
        /// 유저의 유니크 아이디로 user정보 불러오기
        /// </summary>
        public Task<UserInfo?> GetUserByUid(int uid);

        /// <summary>
        /// 유저의 닉네임으로 user정보 불러오기
        /// </summary>
        public Task<UserInfo?> GetUserByNickname(String nickname);

        /// <summary>
        /// 유저의 최근 로그인 시간 업데이트
        /// </summary>
        public Task<int> UpdateRecentLogin(int uid);

        /// <summary>
        /// 유저의 닉네임 변경
        /// </summary>
        public Task<int> UpdateNickname(int uid, String nickname);

        /// <summary>
        /// 유저의 유니크 아이디로 user 메타데이터 불러오기
        /// </summary>
        public Task<MetadataInfo?> GetUserMetaDataByUid(int uid);

        /// <summary>
        /// 유저의 유니크 아이디로 user 스킬 불러오기
        /// </summary>
        public Task<SkillInfo?> GetUserSkillByUid(int uid);
    }
}
