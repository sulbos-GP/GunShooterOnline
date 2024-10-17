using WebCommonLibrary.Models.GameDB;
using System.Data;
using WebCommonLibrary.Models.GameDatabase;

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

        //GameDB_Storage

        /// <summary>
        /// 유저의 인벤토리 로드
        /// </summary>
        public Task<IEnumerable<DB_ItemUnit>> LoadInventory(int storage_id);

        /// <summary>
        /// 유저의 장비 로드
        /// </summary>
        public Task<IEnumerable<DB_GearUnit>> LoadGear(int uid);

        //GameDB_User

        /// <summary>
        /// 유저의 플레이어 아이디로 user정보 불러오기
        /// </summary>
        public Task<FUser?> GetUserByPlayerId(string playerId);

        /// <summary>
        /// 유저의 유니크 아이디로 user정보 불러오기
        /// </summary>
        public Task<FUser?> GetUserByUid(int uid, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 닉네임으로 user정보 불러오기
        /// </summary>
        public Task<FUser?> GetUserByNickname(string nickname);

        /// <summary>
        /// 유저의 최근 로그인 시간 업데이트
        /// </summary>
        public Task<int> UpdateRecentLogin(int uid);

        public Task<int> UpdateLastTicketTime(int uid);

        /// <summary>
        /// 유저의 닉네임 변경
        /// </summary>
        public Task<int> UpdateNickname(int uid, string nickname);

        /// <summary>
        /// 유저의 유니크 아이디로 user 메타데이터 불러오기
        /// </summary>
        public Task<FUserMetadata?> GetUserMetadataByUid(int uid, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 user 메타데이터 업데이트
        /// </summary>
        public Task<int> UpdateUserMetadata(int uid, FUserMetadata matadata, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 user 스킬 불러오기
        /// </summary>
        public Task<FUserSkill?> GetUserSkillByUid(int uid, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 user 일일 퀘스트 불러오기
        /// </summary>
        public Task<List<FUserRegisterQuest>?> GetUserDailyQuestByUid(int uid, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 user 일일 퀘스트 삭제
        /// </summary>
        public Task<int> DeleteUserDailyQuestByUid(int uid, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 user 일일 퀘스트 삭제
        /// </summary>
        public Task<int> InsertUserDailyQuestByUid(int uid, int quest_id, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 user 스킬 업데이트
        /// </summary>
        public Task<int> UpdateUserSkill(int uid, FUserSkill skill, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 user 레벨 보상 불러오기
        /// </summary>
        public Task<IEnumerable<FUserLevelReward>?> GetUserLevelRewardByUid(int uid, bool? received, int? reward_level_id);

        /// <summary>
        /// 유저의 유니크 아이디로 level 업데이트
        /// </summary>
        public Task<int> UpdateLevel(int uid, int experience, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 level 보상 추가
        /// </summary>
        public Task<int> InsertLevelReward(int uid, int level, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 유니크 아이디로 level 보상 추가
        /// </summary>
        public Task<int> RecivedLevelReward(int uid, int level, bool received, IDbTransaction? transaction = null);

        ///////////////////////////////////////
        ///                                 ///
        ///           currency              ///
        ///                                 ///
        ///////////////////////////////////////

        /// <summary>
        /// 유저의 통화 업데이트
        /// </summary>
        public Task<int> UpdateCurrency(int uid, int money, int ticket, int gacha, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 돈 업데이트
        /// </summary>
        public Task<int> UpdateMoney(int uid, int money, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 돈 업데이트
        /// </summary>
        public Task<int> UpdateTicket(int uid, int ticket, IDbTransaction? transaction = null);

        /// <summary>
        /// 유저의 돈 업데이트
        /// </summary>
        public Task<int> UpdateGacha(int uid, int gacha, IDbTransaction? transaction = null);
    }
}
