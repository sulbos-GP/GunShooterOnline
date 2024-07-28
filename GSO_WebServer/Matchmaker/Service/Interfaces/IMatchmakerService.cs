using GSO_WebServerLibrary;
using Matchmaker.DTO.GameServerManager;
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
        public Task<WebErrorCode> AddMatchQueue(Int32 uid, Double rating, String world, String region);

        /// <summary>
        /// 플레이어 레이팅 및 정보 대기열에서 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchQueue(Int32 uid);

        /// <summary>
        /// 플레이어 레이팅 및 정보 대기열에서 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchQueue(String[] keys);

        /// <summary>
        /// 모든 플레이어 조회
        /// </summary>
        public Task<(WebErrorCode, Dictionary<int, PlayerInfo>?)> ScanPlayers();

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
        public Task<WebErrorCode> NotifyMatchSuccess(String[] keys, MatchInfo match);
    }
}
