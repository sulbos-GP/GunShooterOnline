using WebCommonLibrary.Enum;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;

namespace GameServerManager.Repository.Interfaces
{
    public interface ISessionMemory : IDisposable
    {
        /// <summary>
        /// 매치 정보 추가
        /// </summary>
        public Task<WebErrorCode> AddMatchStatus(String containerId, String name, String world, EMatchState state, String hostIp, Int32 containerPort, Int32 hostPort, DateTime age);

        /// <summary>
        /// 매치 정보 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveMatchStatus(String containerId);

        /// <summary>
        /// 매치 정보 수정
        /// </summary>
        public Task<bool> UpdateMatchStatus(String containerId, MatchStatus status);

        /// <summary>
        /// 매치 정보 모두 가져오기
        /// </summary>
        public Task<Dictionary<String, MatchStatus>?> GetAllMatchStatus();

        /// <summary>
        /// 매치 정보 가져오기
        /// </summary>
        public Task<MatchStatus?> GetMatchStatus(String containerId);
    }
}
