using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;

namespace Matchmaker.Service.Interfaces
{
    public interface IMatchmakerService : IDisposable
    {

        public Task ClearMatch();

        /// <summary>
        /// 허브에 연결됨
        /// </summary>
        public Task<bool> ConnectedClient(int uid, string connectionId);

        /// <summary>
        /// 허브에 연결 끊김
        /// </summary>
        public Task<bool> DisconnectedClient(int uid, string connectionId);

        /// <summary>
        /// 플레이어 티켓 검색
        /// </summary>
        public Task<Ticket?> GetMatchTicket(Int32 uid);

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
        public Task<WebErrorCode> RemoveMatchQueue(string key);

        /// <summary>
        /// 플레이어들 중 나가고 싶은 사람이 있다면 처리
        /// </summary>
        public Task<Dictionary<string, Ticket>?> CheckPlayersLeavingQueue();

        /// <summary>
        /// 가장 오래 기달린 플레이어
        /// </summary>
        public Task<(WebErrorCode, PlayerInfo?)> GetLongestWaitingPlayer();

        /// <summary>
        /// 플레이어 레이팅에 따른 매칭 찾아주고 key 리턴
        /// </summary>
        public Task<(WebErrorCode, Dictionary<string, Ticket>?)> FindMatchByRating(double min, double max, int capacity);

        public Task<WebErrorCode> MatchConfirmation(Dictionary<string, Ticket> players);

        /// <summary>
        /// 매칭이 성사된 유저들에게 매칭이 성공했음을 알린다
        /// </summary>
        public Task NotifyMatchSuccess(Ticket ticket, MatchProfile profile);

        /// <summary>
        /// 매칭이 성사된 유저들에게 매칭이 성공했음을 알린다
        /// </summary>
        public Task NotifyMatchFailed(Ticket ticket, WebErrorCode error);


        /// <summary>
        /// 플레이어의 레이턴시 업데이트
        /// </summary>
        public Task<WebErrorCode> UpdateLatency(Int32 uid, Int64 latency);

        public Task RollbackTicket(Int32 uid);
    }
}
