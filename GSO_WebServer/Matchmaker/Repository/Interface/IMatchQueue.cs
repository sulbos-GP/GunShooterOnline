using CloudStructures;
using CloudStructures.Structures;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;

namespace Matchmaker.Repository.Interface
{
    public interface IMatchQueue : IDisposable
    {

        //////////////////////////////////////////////////////
        ///                                                ///
        ///                                                ///
        ///                     Lock                       ///
        ///                                                ///
        ///                                                ///
        //////////////////////////////////////////////////////

        public Task<bool> TryTakeLock(int uid, bool isWaiting = true);

        public Task ReleaseLock(int uid);

        //////////////////////////////////////////////////////
        ///                                                ///
        ///                                                ///
        ///                    TICKET                      ///
        ///                                                ///
        ///                                                ///
        //////////////////////////////////////////////////////

        public Task ClearTicket();

        /// <summary>
        /// 클라이언트 아이디로 UID 가져오기
        /// </summary>
        public Task<(WebErrorCode, int)> GetUidWithClientId(String clientId);

        /// <summary>
        /// 티켓 가져오기
        /// </summary>
        public Task<(WebErrorCode, Ticket?)> GetTicketWithUid(Int32 uid);
        public Task<(WebErrorCode, Dictionary<int, Ticket>?)> GetTicketsWithUids(Int32[] uids);
        public Task<(WebErrorCode, Ticket?)> GetTicketWithKey(string key);
        public Task<(WebErrorCode, Dictionary<string, Ticket>?)> GetTicketsWithKeys(string[] keys);
        public Task<(WebErrorCode, Ticket?)> GetTicketWithClientId(String clientId);

        /// <summary>
        /// 모든 티켓 가져오기
        /// </summary>
        public Task<(WebErrorCode, Dictionary<string, Ticket>?)> GetAllTicket();

        /// <summary>
        /// 매칭 티켓에 유저의 점수 추가
        /// </summary>
        public Task<WebErrorCode> AddTicket(Int32 uid, String clientId);

        /// <summary>
        /// 매칭 티켓에서 유저 완전 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveTicket(Int32 uid);

        /// <summary>
        /// 티켓 업데이트
        /// </summary>
        public Task<bool> SetTicket(Int32 uid, Ticket ticket);

        //////////////////////////////////////////////////////
        ///                                                ///
        ///                                                ///
        ///                    RATING                      ///
        ///                                                ///
        ///                                                ///
        //////////////////////////////////////////////////////

        public Task ClearRating();

        /// <summary>
        /// 레이팅 존재하는지
        /// </summary>
        public Task<bool> IsValidRatingWithUid(Int32 uid);

        /// <summary>
        /// 레이팅 가져오기
        /// </summary>
        public Task<double?> GetRatingWithKey(string uid);

        /// <summary>
        /// 레이팅 추가
        /// </summary>
        public Task<WebErrorCode> AddRating(Int32 uid, Double rating);

        /// <summary>
        /// 레이팅 제거
        /// </summary>
        public Task<WebErrorCode> RemoveRating(Int32 uid);

        //////////////////////////////////////////////////////
        ///                                                ///
        ///                                                ///
        ///                      MATCH                     ///
        ///                                                ///
        ///                                                ///
        //////////////////////////////////////////////////////

        /// <summary>
        /// 매칭 레이팅에서 범위에 따라 uid 리턴
        /// </summary>
        public Task<string[]?> SearchPlayerByRange(Double min, Double max);
    }
}
