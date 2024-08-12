using Docker.DotNet;
using Docker.DotNet.Models;
using GameServerManager.Servicies.Interfaces;
using GSO_WebServerLibrary.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading;

namespace GameServerManager.Servicies
{
    public class DockerService : IDockerService
    {
        private readonly IOptions<DockerConfig> mDockerConfig;

        private readonly DockerClient mDockerClient;
        private CancellationTokenSource mCancellationTokenSource;
        
        private int mContainerCount = 1;
        private PortManager mPortManager;

        public DockerService(IOptions<DockerConfig> config)
        {
            mDockerConfig = config;

            mDockerClient = new DockerClientConfiguration().CreateClient();
            //mDockerClient = new DockerClientConfiguration(new Uri(mDockerConfig.Value.EndPoint)).CreateClient();
            mCancellationTokenSource = new CancellationTokenSource();
            mPortManager = new PortManager(7001, 8000);
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
            //await mDockerClient.Images.DeleteImageAsync(
            //    mDockerConfig.Value.Image,
            //    new ImageDeleteParameters
            //    {

            //    });

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

            string port = mPortManager.PopPort().ToString();
            string ip = "host.docker.internal";
            string register = "10";
            string backLog = "10";

            string name = mDockerConfig.Value.Tag + mContainerCount++;
            string image = mDockerConfig.Value.Image + ":" + mDockerConfig.Value.Tag;
            return await mDockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Name = name,
                Image = image,
                Env = new List<string>
                {
                    $"HOST_PORT={port}",
                    $"HOST_IP={ip}",
                    $"REGISTER={register}",
                    $"BACKLOG={backLog}"
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { $"{port}/udp", new List<PortBinding> { new PortBinding { HostIP = "0.0.0.0" ,HostPort = $"{port}" } } }
                    }
                },
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { $"{port}/udp", new EmptyStruct() }
                },
                Healthcheck = new HealthConfig
                {
                    Interval = new TimeSpan(0, 0, 5),
                    Timeout = new TimeSpan(0, 0, 3)
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
                new ContainerStartParameters
                {

                }
                );
        }

        public async Task<bool> StopContainer(String containerId)
        {

            var info = await GetContainerInfo(containerId);
            mPortManager.PushPort(Convert.ToInt32(info.NetworkSettings.Ports.ElementAt(0).Value.ElementAt(0).HostPort));

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

            var info = await GetContainerInfo(containerId);
            mPortManager.PushPort(Convert.ToInt32(info.NetworkSettings.Ports.ElementAt(0).Value.ElementAt(0).HostPort));

            await mDockerClient.Containers.KillContainerAsync(
                containerId,
                new ContainerKillParameters
                {

                },
                mCancellationTokenSource.Token);
        }

        public async Task RemoveContainer(String containerId)
        {
            await mDockerClient.Containers.RemoveContainerAsync(
                containerId,
                new ContainerRemoveParameters
                {

                },
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

            if(info.State == null)
            {
                return false;
            }

            if(info.State.Health == null)
            {
                return false;
            }

            string health = info.State.Health.Status;
            if(health == "starting")
            {
                return true;
            }
            else if(health == "healthy")
            {
                return true;
            }
            else if(health == "unhealthy")
            {
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
