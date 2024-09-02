using Docker.DotNet.Models;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;

namespace GameServerManager.Servicies.Interfaces
{
    public interface ISessionService : IDisposable
    {
        /// <summary>
        /// 도커 컨테이너 중 매치가 가능한 방이 있는지 확인
        /// </summary>
        public Task<(WebErrorCode, MatchProfile?)> FetchMatch();

        /// <summary>
        /// 도커 컨테이너 초기화 및 매치 생성
        /// </summary>
        public Task<WebErrorCode> InitMatch(long limit);

        /// <summary>
        /// 매치 생성
        /// </summary>
        public Task<(WebErrorCode, CreateContainerResponse?)> CreateMatch();

        /// <summary>
        /// 매치 준비 완료 요청 처리
        /// </summary>
        public Task<WebErrorCode> RequsetReadyMatch(String containerId);

        /// <summary>
        /// 매치 게임 종료 요청 처리
        /// </summary>
        public Task<WebErrorCode> ShutdownMatch(String containerId);
    }
}
