using CloudStructures.Structures;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;

namespace Matchmaker.Repository.Interface
{
    public interface IMatchQueue : IDisposable
    {
        /// <summary>
        /// 매칭 레이팅에 유저의 점수 추가
        /// </summary>
        public Task<WebErrorCode> AddMatchRating(Int32 uid, Double rating);

        /// <summary>
        /// 매칭 레이팅에서 유저 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchRating(Int32 uid);

        /// <summary>
        /// 매칭 레이팅에서 유저 모두 조회
        /// </summary>
        public Task<RedisSortedSetEntry<string>[]?> GetAllMatchRating();

        /// <summary>
        /// 매칭 레이팅에서 범위에 따라 uid 리턴
        /// </summary>
        public Task<string[]?> SearchPlayerByRange(Double min, Double max);

        /// <summary>
        /// 매칭 티켓에 유저의 점수 추가
        /// </summary>
        public Task<WebErrorCode> AddMatchTicket(Int32 uid, String clientId);

        /// <summary>
        /// 매칭 티켓에서 유저 완전 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchTicket(Int32 uid);

        /// <summary>
        /// 매칭 티켓에서 유저 모두 조회
        /// </summary>
        public Task<Dictionary<string, TicketInfo>?> GetAllMatchTicket();

        /// <summary>
        /// 유저 티켓
        /// </summary>
        public Task<TicketInfo?> GetPlayerTicket(Int32 uid);

        /// <summary>
        /// 일부 유저 티켓
        /// </summary>
        public Task<Dictionary<string, TicketInfo>?> GetPlayerTickets(string[] keys);

        public Task<bool> UpdateTicket(Int32 uid, TicketInfo ticket);

    }
}
