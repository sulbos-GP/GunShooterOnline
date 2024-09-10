using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;

namespace Matchmaker.Service.Interfaces
{
    public interface IMatchmakerService : IDisposable
    {
        /// <summary>
        /// 플레이어 티켓 대기열에 추가
        /// </summary>
        public Task<WebErrorCode> AddMatchTicket(Int32 uid, String clientId);

        /// <summary>
        /// 플레이어 티켓 대기열에서 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchTicket(String uid);

        /// <summary>
        /// 플레이어 레이팅 대기열에 추가 및 정보 업데이트
        /// </summary>
        public Task<WebErrorCode> PushMatchQueue(Int32 uid, String world, String region);

        /// 플레이어 레이팅 대기열에서 삭제 및 정보 초기화
        /// </summary>
        public Task<WebErrorCode> PopMatchQueue(Int32 uid);

        /// <summary>
        /// 플레이어 레이팅 및 티겟 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchQueue(Int32 uid);

        /// <summary>
        /// 플레이어 레이팅 및 티겟 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchQueue(String[] keys);

        /// <summary>
        /// 모든 플레이어 조회
        /// </summary>
        public Task<(WebErrorCode, Dictionary<int, PlayerInfo>?)> ScanPlayers();

        /// <summary>
        /// 플레이어 잠금
        /// </summary>
        public Task<WebErrorCode> LockPlayers(String id);

        /// <summary>
        /// 플레이어 잠금 해제
        /// </summary>
        public Task<WebErrorCode> UnLockPlayers(String id);

        /// <summary>
        /// 플레이어 레이팅에 따른 매칭 찾아주고 key 리턴
        /// </summary>
        public Task<(WebErrorCode, Dictionary<string, TicketInfo>?)> FindMatchByRating(double min, double max, int capacity);

        /// <summary>
        /// 플레이어의 레이턴시 업데이트
        /// </summary>
        public Task<WebErrorCode> UpdateLatency(Int32 uid, Int64 latency);

        /// <summary>
        /// 매칭이 성사된 유저들에게 매칭이 성공했음을 알린다
        /// </summary>
        public Task<WebErrorCode> NotifyMatchSuccess(TicketInfo[] tickets, MatchProfile profile);
    }
}
