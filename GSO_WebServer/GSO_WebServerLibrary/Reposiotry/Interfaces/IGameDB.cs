using GSO_WebServerLibrary.Models.GameDB;
using System.Data;

namespace GSO_WebServerLibrary.Reposiotry.Interfaces
{
    public interface IGameDB : IDisposable
    {
        //GameDB
        public IDbConnection GetConnection();

        //GameDB_Init
        public Task<int> SingUp(string playerId, string service, string refresh_token, IDbTransaction transaction);
        public Task<int> InitUserMatadata(int uid, IDbTransaction transaction);
        public Task<int> InitUserSkill(int uid, IDbTransaction transaction);

        //GameDB_Game

        //GameDB_Metadata

        //GameDB_Skill

        //GameDB_User

        /// <summary>
        /// 유저의 플레이어 아이디로 user정보 불러오기
        /// </summary>
        public Task<UserInfo?> GetUserByPlayerId(string playerId);

        /// <summary>
        /// 유저의 유니크 아이디로 user정보 불러오기
        /// </summary>
        public Task<UserInfo?> GetUserByUid(int uid);

        /// <summary>
        /// 유저의 닉네임으로 user정보 불러오기
        /// </summary>
        public Task<UserInfo?> GetUserByNickname(string nickname);

        /// <summary>
        /// 유저의 최근 로그인 시간 업데이트
        /// </summary>
        public Task<int> UpdateRecentLogin(int uid);

        /// <summary>
        /// 유저의 닉네임 변경
        /// </summary>
        public Task<int> UpdateNickname(int uid, string nickname);

        /// <summary>
        /// 유저의 유니크 아이디로 user 메타데이터 불러오기
        /// </summary>
        public Task<UserMetadataInfo?> GetUserMetadataByUid(int uid);

        /// <summary>
        /// 유저의 유니크 아이디로 user 메타데이터 업데이트
        /// </summary>
        public Task<int> UpdateUserMetadata(int uid, UserMetadataInfo matadata);

        /// <summary>
        /// 유저의 유니크 아이디로 user 스킬 불러오기
        /// </summary>
        public Task<UserSkillInfo?> GetUserSkillByUid(int uid);

        /// <summary>
        /// 유저의 유니크 아이디로 user 스킬 업데이트
        /// </summary>
        public Task<int> UpdateUserSkill(int uid, UserSkillInfo skill);
    }
}
