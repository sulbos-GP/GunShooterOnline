using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Models.Match;
using Matchmaker.Models;

namespace Matchmaker.Service.Interfaces
{
    public interface IMatchmakerService : IDisposable
    {
        /// <summary>
        /// 플레이어 레이팅 및 정보 대기열에 초기화
        /// </summary>
        public Task<WebErrorCode> InitMatchQueue(Int32 uid, String clientId);

        /// <summary>
        /// 플레이어 레이팅 및 정보 대기열에 추가
        /// </summary>
        public Task<WebErrorCode> AddMatchQueue(Int32 uid, String world, String region);

        /// <summary>
        /// 플레이어 레이팅 및 정보 대기열에서 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchQueue(Int32 uid);

        /// <summary>
        /// 플레이어 레이팅 및 정보 대기열에서 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchQueue(String[] keys);

        /// <summary>
        /// 모든 플레이어 조회 및 잠금
        /// </summary>
        public Task<(WebErrorCode, Dictionary<int, PlayerInfo>?)> ScanAndLockPlayers();

        /// <summary>
        /// 모든 플레이어 잠금 해제
        /// </summary>
        public Task<WebErrorCode> UnLockPlayers(Dictionary<int, PlayerInfo> players);

        /// <summary>
        /// 플레이어 레이팅에 따른 매칭 찾아주고 key 리턴
        /// </summary>
        public Task<(WebErrorCode, string[]?)> FindMatchByRating(double min, double max, int capacity);

        /// <summary>
        /// 플레이어의 레이턴시 업데이트
        /// </summary>
        public Task<WebErrorCode> UpdateLatency(Int32 uid, Int64 latency);

        /// <summary>
        /// 매칭이 성사된 유저들에게 매칭이 성공했음을 알린다
        /// </summary>
        public Task<WebErrorCode> NotifyMatchSuccess(String[] keys, MatchProfile profile);
    }
}
