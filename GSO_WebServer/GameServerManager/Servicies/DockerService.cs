using Docker.DotNet;
using Docker.DotNet.Models;
using GameServerManager.Servicies.Interfaces;
using GSO_WebServerLibrary.Config;
using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace GameServerManager.Servicies
{
    public class DockerService : IDockerService
    {
        private readonly IOptions<DockerConfig> mDockerConfig;

        private readonly DockerClient mDockerClient;
        private CancellationTokenSource mCancellationTokenSource;

        private int mContainerCount = 1;
        private readonly long mContainerCapacity = 1;

        public DockerService(IOptions<DockerConfig> config)
        {
            mDockerConfig = config;

            mDockerClient = new DockerClientConfiguration().CreateClient();
            //mDockerClient = new DockerClientConfiguration(new Uri(mDockerConfig.Value.EndPoint)).CreateClient();
            mCancellationTokenSource = new CancellationTokenSource();

        }

        public async void Dispose()
        {
            await DockerDispose();
        }

        public async Task<bool> InitDocker()
        {
            try
            {
                VersionResponse version = await mDockerClient.System.GetVersionAsync();

                Console.WriteLine("Init Docker");
                Console.WriteLine("{");

                Console.WriteLine($"\tDocker os : {version.Os}");
                Console.WriteLine($"\tDocker version : {version.Version}");
                Console.WriteLine($"\tDocker engine uri : {mDockerClient.Configuration.EndpointBaseUri}");

                Console.WriteLine($"\tDocker pull image : {mDockerConfig.Value.Image}");
                await CreateImageFromRegistry();
                Console.WriteLine("}");

                return true;

            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"ArgumentNullException : {ex}");
                return false;
            }
            catch (DockerApiException ex)
            {
                Console.WriteLine($"DockerApiException : {ex}");
                return false;
            }

        }

        public async Task DockerDispose()
        {
            try
            {
                Console.WriteLine("Docker dispose");
                Console.WriteLine("{");

                mCancellationTokenSource.Cancel();

                Console.Write($"    Docker container all list : ");
                IList<ContainerListResponse> list = await GetAllContainerList();
                Console.WriteLine(list.Count);

                Console.WriteLine($"    Docker container stop");
                foreach (var container in list)
                {
                    await StopContainer(container.ID);
                }

                mCancellationTokenSource.Dispose();
                mDockerClient.Dispose();

                Console.WriteLine("}");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"ArgumentNullException : {ex}");
            }
            catch (DockerApiException ex)
            {
                Console.WriteLine($"DockerApiException : {ex}");
            }

        }

        /// <summary>
        /// 도커 저장소에서 이미지 가져오기
        /// </summary>
        private void OnProgressCreateImage(JSONMessage message)
        {
            Console.WriteLine($"        status:{message.Status}, id:{message.ID}, from:{message.From}, time:{message.Time}");
        }

        private async Task CreateImageFromRegistry()
        {
            await mDockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = mDockerConfig.Value.Image,
                    Tag = mDockerConfig.Value.Tag,
                },
                new AuthConfig
                {
                    Email = mDockerConfig.Value.Email,
                    Username = mDockerConfig.Value.UserName,
                    Password = mDockerConfig.Value.Password
                },
                new Progress<JSONMessage>(this.OnProgressCreateImage));
        }

        /// <summary>
        /// 도커 컨테이너 생성
        /// </summary>
        public async Task<CreateContainerResponse> CreateContainer()
        {
            string name = mDockerConfig.Value.Tag + mContainerCount++;
            string image = mDockerConfig.Value.Image + ":" + mDockerConfig.Value.Tag;
            return await mDockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Name = name,
                Image = image,
                HostConfig = new HostConfig()
                {
                    DNS = new[] { "8.8.8.8", "8.8.4.4" }
                }
            });
        }

        /// <summary>
        /// 도커 컨테이너 시작
        /// </summary>
        public async Task StartContainer(String containerId)
        {
            await mDockerClient.Containers.StartContainerAsync(
                containerId,
                new ContainerStartParameters()
                );
        }

        public async Task<bool> StopContainer(String containerId)
        {
            return await mDockerClient.Containers.StopContainerAsync(
                containerId,
                new ContainerStopParameters
                {
                    WaitBeforeKillSeconds = 30
                },
                mCancellationTokenSource.Token);
        }


        public async Task KillContainer(String containerId)
        {
            await mDockerClient.Containers.KillContainerAsync(
                containerId,
                new ContainerKillParameters(),
                mCancellationTokenSource.Token);
        }

        public async Task RestartContainer(String containerId)
        {
            await mDockerClient.Containers.RestartContainerAsync(
                containerId,
                new ContainerRestartParameters
                {
                    WaitBeforeKillSeconds = 30
                },
                mCancellationTokenSource.Token);
        }

        /// <summary>
        /// 도커 컨테이너 모니터링 (나중에 사용 예정)
        /// </summary>
        private void OnProgressMonitor(Message message)
        {
            Console.WriteLine($"        status:{message.Status}, id:{message.ID}, from:{message.From}, time:{message.Time}");
        }

        private async Task SetMonitorEvents()
        {
            ContainerEventsParameters parameters = new ContainerEventsParameters();
            await mDockerClient.System.MonitorEventsAsync(parameters, new Progress<Message>(OnProgressMonitor));
        }

        public async Task<IList<ContainerListResponse>> GetContainerList(long limit)
        {
            IList<ContainerListResponse> containers = await mDockerClient.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    Limit = limit,
                });

            return containers;
        }

        public async Task<IList<ContainerListResponse>> GetAllContainerList()
        {
            IList<ContainerListResponse> containers = await mDockerClient.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    All = true,
                });

            return containers;
        }

        public async Task<ContainerInspectResponse> GetContainerInfo(String containerId)
        {
            return await mDockerClient.Containers.InspectContainerAsync(containerId);
        }

        public async Task<bool> CheckContainerHealth(String containerId)
        {
            var info = await GetContainerInfo(containerId);
            if(info == null)
            {
                return false;
            }

            return info.State.Health.Status == "healthy";
        }
    }
}
