using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace GsoWebServer.Servicies.Interfaces
{
    public interface IGameService : IDisposable
    {
        /// <summary>
        /// 유저의 회원가입 및 게임 데이터 생성
        /// </summary>
        public Task<(WebErrorCode, int)> SingUpWithNewUserGameData(String userId, String service, String refresh_token);

        /// <summary>
        /// 유저의 닉네임 변경
        /// </summary>
        public Task<(WebErrorCode, string)> UpdateNickname(Int32 uid, String newNickname);

        /// <summary>
        /// 유저 정보 가져오기
        /// </summary>
        public Task<(WebErrorCode, FUser?)> GetUserInfo(int uid);

        /// <summary>
        /// 유저 메타데이터 정보 가져오기
        /// </summary>
        public Task<(WebErrorCode, FUserMetadata?)> GetMetadataInfo(int uid);

        /// <summary>
        /// 유저 스킬(레이팅) 정보 가져오기
        /// </summary>
        public Task<(WebErrorCode, FUserSkill?)> GetSkillInfo(int uid);

        /// <summary>
        /// 유저 등록된 일일 퀘스트 정보 가져오기
        /// </summary>
        public Task<(WebErrorCode, List<FUserRegisterQuest>?)> GetDailyQuest(int uid);

        /// <summary>
        /// 유저 레벨 보상 정보 가져오기
        /// </summary>
        public Task<(WebErrorCode, List<FUserLevelReward>?)> GetUserLevelReward(int uid, bool? received, int? reward_level_id);

        /// <summary>
        /// 유저 메타데이터 정보 업데이트
        /// </summary>
        public Task<WebErrorCode> UpdateUserMetadata(Int32 uid, FUserMetadata matadata);

        /// <summary>
        /// 유저 스킬(레이팅) 정보 업데이트
        /// </summary>
        public Task<WebErrorCode> UpdateUserSkill(Int32 uid, FUserSkill skill);

        /// <summary>
        /// 저장소의 아이템 로드
        /// </summary>
        public Task<(WebErrorCode, IEnumerable<DB_ItemUnit>?)> LoadInventory(int storage_id);

        /// <summary>
        /// 장비의 아이템 로드
        /// </summary>
        public Task<(WebErrorCode, IEnumerable<DB_GearUnit>?)> LoadGear(int uid);

        /// <summary>
        /// 유저 레벨업
        /// </summary>
        public Task<WebErrorCode> UpdateLevel(Int32 uid, Int32 experience);

        /// <summary>
        /// 유저 레벨 보상 받기
        /// </summary>
        public Task<WebErrorCode> RecvLevelReward(Int32 uid, Int32 level);

        /// <summary>
        /// 티켓 수량 업데이트
        /// </summary>
        public Task<WebErrorCode> UpdateTicketCount(Int32 uid);

        public Task<WebErrorCode> UpdateLastTicketTime(int uid);

        public Task<WebErrorCode> UpdateDailyTask(int uid);

        public Task<WebErrorCode> UpdateDailyQuset(int uid);
    }
}
