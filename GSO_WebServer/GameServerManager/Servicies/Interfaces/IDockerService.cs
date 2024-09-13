using Docker.DotNet.Models;

namespace GameServerManager.Servicies.Interfaces
{
    public interface IDockerService : IDisposable
    {
        /// <summary>
        /// 도커 초기화
        /// </summary>
        public Task<bool> InitDocker();

        public Task ClearContainer();

        /// <summary>
        /// 도커 컨테이너 생성
        /// </summary>
        public Task<CreateContainerResponse> CreateContainer();

        /// <summary>
        /// 도커 컨테이너 시작
        /// </summary>
        public Task StartContainer(String containerId);

        /// <summary>
        /// 도커 컨테이너 종료
        /// </summary>
        public Task<bool> StopContainer(String containerId);

        /// <summary>
        /// 도커 컨테이너 강제 종료
        /// </summary>
        public Task KillContainer(String containerId);

        /// <summary>
        /// 도커 컨테이너 제거
        /// </summary>
        public Task RemoveContainer(String containerId);

        /// <summary>
        /// 도커 컨테이너 재시작
        /// </summary>
        public Task RestartContainer(String containerId);

        /// <summary>
        /// 도커 컨테이너 일정 리스트 불러오기
        /// </summary>
        public Task<IList<ContainerListResponse>> GetContainerList(long limit);

        /// <summary>
        /// 도커 컨테이너 모든 리스트 불러오기
        /// </summary>
        public Task<IList<ContainerListResponse>> GetAllContainerList();

        /// <summary>
        /// 도커 컨테이너 health 체크
        /// </summary>
        public Task<ContainerInspectResponse> GetContainerInfo(String containerId);

        /// <summary>
        /// 도커 컨테이너 health 체크
        /// </summary>
        public Task<bool> CheckContainerHealth(String containerId);
    }
}
