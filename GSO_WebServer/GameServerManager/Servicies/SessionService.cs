using Docker.DotNet;
using Docker.DotNet.Models;
using GameServerManager.Models;
using GameServerManager.Repository.Interfaces;
using GameServerManager.Servicies.Interfaces;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Models.Match;

namespace GameServerManager.Servicies
{
    public class SessionService : ISessionService
    {
        private readonly IDockerService mDockerService;
        private readonly ISessionMemory mSessionMemory;

        public SessionService(IDockerService dockerService, ISessionMemory sessionMemory)
        {
            mDockerService = dockerService;
            mSessionMemory = sessionMemory;
        }

        public void Dispose()
        {

        }

        public async Task<(WebErrorCode, MatchProfile?)> FetchMatch()
        {
            try
            {
                var matchProfile = new MatchProfile();

                var matchStatus = await mSessionMemory.GetAllMatchStatus();
                if (matchStatus == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                var readyMatch = matchStatus.FirstOrDefault(status => status.Value.state == Models.EMatchState.Ready);
                if(readyMatch.Key == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                matchProfile.container_id = readyMatch.Key;
                matchProfile.world = readyMatch.Value.world;
                matchProfile.host_ip = readyMatch.Value.host_ip;
                matchProfile.container_port = readyMatch.Value.container_port;
                matchProfile.host_port = readyMatch.Value.host_port;
                return (WebErrorCode.None, matchProfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FetchMatch execption : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> InitMatch(long limit)
        {
            try
            {

                await mDockerService.InitDocker();

                Console.WriteLine("Init Match");
                Console.WriteLine("{");
                Console.WriteLine($"\tDocker container create : {limit}");
                for (int index = 0; index < limit; ++index)
                {
                    var (error, match) = await CreateMatch();
                    if(error != WebErrorCode.None || match == null)
                    {
                        continue;
                    }

                    Console.WriteLine($"\tNew container id : {match.ID}");
                }
                Console.WriteLine("}");

                return WebErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InitMatch execption : {ex.ToString()}");
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<(WebErrorCode, CreateContainerResponse?)> CreateMatch()
        {
            try
            {
                var newContainer = await mDockerService.CreateContainer();
                if (newContainer == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                var newContainerInfo = await mDockerService.GetContainerInfo(newContainer.ID);
                if (newContainerInfo == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                if (newContainerInfo.State.Status != "created")
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                var error = await mSessionMemory.AddMatchStatus
                    (
                        newContainerInfo.ID,
                        newContainerInfo.Name,
                        newContainerInfo.Config.Image.Substring(newContainerInfo.Config.Image.IndexOf(":") + 1),
                        EMatchState.Creating,
                        "",
                        0,
                        0,
                        newContainerInfo.Created
                    );

                var status = await mSessionMemory.GetMatchStatus(newContainerInfo.ID);
                if (status == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                if (error != WebErrorCode.None)
                {
                    await mDockerService.KillContainer(newContainerInfo.ID);
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                await mDockerService.StartContainer(newContainerInfo.ID);

                status.state = EMatchState.Starting;
                await mSessionMemory.UpdateMatchStatus(newContainerInfo.ID, status);

                newContainerInfo = await mDockerService.GetContainerInfo(newContainer.ID);
                if (newContainerInfo == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                if (newContainerInfo.State.Status != "running")
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                status.host_ip = newContainerInfo.NetworkSettings.IPAddress;
                //status.host_port = Convert.ToInt32(newContainerInfo.NetworkSettings.Ports.ElementAt(0).Key.Substring(newContainerInfo.NetworkSettings.Ports.ElementAt(0).Key.IndexOf("/") + 1));
                //status.container_port = Convert.ToInt32(newContainerInfo.NetworkSettings.Ports.ElementAt(0).Value.ElementAt(0).HostPort);
                status.state = EMatchState.Scheduled;
                await mSessionMemory.UpdateMatchStatus(newContainerInfo.ID, status);

                return (WebErrorCode.None, newContainer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateMatch execption : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> RequsetReadyMatch(String containerId)
        {
            try
            {
                var containerInfo = await mDockerService.GetContainerInfo(containerId);
                var matchStatus = await mSessionMemory.GetMatchStatus(containerId);

                if (containerInfo == null || matchStatus == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }
                
                if(matchStatus.state != EMatchState.Scheduled)
                {
                    return WebErrorCode.TEMP_ERROR;
                }
                matchStatus.state = EMatchState.RequestReady;

                await mSessionMemory.UpdateMatchStatus(containerId, matchStatus);

                matchStatus.state = EMatchState.Ready;

                await mSessionMemory.UpdateMatchStatus(containerId, matchStatus);

                return WebErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RequsetReadyMatch execption : {ex.ToString()}");
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<WebErrorCode> ShutdownMatch(String containerId)
        {
            try
            {
                var containerInfo   = await mDockerService.GetContainerInfo(containerId);
                var matchStatus     = await mSessionMemory.GetMatchStatus(containerId);

                if(containerInfo == null || matchStatus == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }
                matchStatus.state = EMatchState.Shutdown;

                if(false == await mDockerService.StopContainer(containerId))
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ShutdownMatch execption : {ex.ToString()}");
                return WebErrorCode.TEMP_Exception;
            }
        }

    }
}
