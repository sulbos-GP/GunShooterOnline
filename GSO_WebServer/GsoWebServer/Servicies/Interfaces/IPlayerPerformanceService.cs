using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

namespace GsoWebServer.Servicies.Interfaces
{
    public interface IPlayerPerformanceService : IDisposable
    {
        /// <summary>
        /// 플레이어의 게임 결과 통계 반영
        /// </summary>
        public Task<WebErrorCode> UpdatePlayerStats(int uid, MatchOutcome outcome);

        /// <summary>
        /// 플레이어의 게임 결과 레이팅 반영
        /// </summary>
        public Task<WebErrorCode> UpdatePlayerRating(Dictionary<int, MatchOutcome> outcomes);
    }
}
