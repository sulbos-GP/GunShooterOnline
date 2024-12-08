using Docker.DotNet;
using Docker.DotNet.Models;
using GameServerManager.Repository;
using GameServerManager.Repository.Interfaces;
using GameServerManager.Servicies.Interfaces;
using GSO_WebServerLibrary.Utils;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;

namespace GameServerManager.Servicies
{
    public class SessionService : ISessionService
    {
        private readonly IDockerService mDockerService;
        private readonly ISessionMemory mSessionMemory;
        private readonly IHttpClientFactory mHttpClientFactory;

        public SessionService(IDockerService dockerService, ISessionMemory sessionMemory, IHttpClientFactory httpClientFactory)
        {
            mDockerService = dockerService;
            mSessionMemory = sessionMemory;
            mHttpClientFactory = httpClientFactory;
        }

        public void Dispose()
        {

        }

        public async Task<WebErrorCode> InitMatch(long limit)
        {
            try
            {

                var containers = await mDockerService.GetAllContainerList();
                foreach (var container in containers)
                {
                    
                    await mSessionMemory.RemoveMatchStatus(container.ID);

                    if(container.State != "Dead")
                    {
                        await mDockerService.StopContainer(container.ID);
                    }
                    await mDockerService.RemoveContainer(container.ID);
                }

                await mDockerService.InitDocker();

                Console.WriteLine("Init Match");
                Console.WriteLine("{");
                Console.WriteLine($"\tDocker container create : {limit}");
                for (int index = 0; index < limit; ++index)
                {
                    var (error, match) = await CreateMatch();
                    if (error != WebErrorCode.None || match == null)
                    {
                        continue;
                    }

                    Console.WriteLine($"\tNew container id : {match.ID}\n");
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

        public async Task<(WebErrorCode, MatchProfile?)> FetchMatch()
        {
            try
            {
                var matchProfile = new MatchProfile();

                var matchStatus = await mSessionMemory.GetAllMatchStatus();
                if (matchStatus == null)
                {
                    return (WebErrorCode.EmptySession, null);
                }

                var match = matchStatus.FirstOrDefault(status => status.Value.state == EMatchState.Ready);
                if(match.Key == null)
                {
                    return (WebErrorCode.NoPendingSession, null);
                }
                string key = match.Key;
                MatchStatus status = match.Value;

                status.state = EMatchState.Allocated;
                await mSessionMemory.UpdateMatchStatus(key, status);

                matchProfile.container_id = key;
                matchProfile.world = status.world;
                matchProfile.host_ip = status.host_ip;
                matchProfile.container_port = status.container_port;
                matchProfile.host_port = status.host_port;
                return (WebErrorCode.None, matchProfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FetchMatch execption : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> StartMatch(string containerId, MatchStatus matchStatus)
        {
            try
            {
                var matchProfile = new MatchProfile();
                matchProfile.container_id = containerId;
                matchProfile.world = matchStatus.world;
                matchProfile.host_ip = matchStatus.host_ip;
                matchProfile.container_port = matchStatus.container_port;
                matchProfile.host_port = matchStatus.host_port;


                var MatchmakerClient = mHttpClientFactory.CreateClient("Matchmaker");
                if (MatchmakerClient == null)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }

                NotifyStartMatchReq packet = new NotifyStartMatchReq
                {
                    players = matchStatus.players,
                    match_profile = matchProfile,
                };
                var content = new StringContent(JsonSerializer.Serialize(packet), Encoding.UTF8, "application/json");

                var response = await MatchmakerClient.PostAsync("api/Session/StartMatch", content);
                response.EnsureSuccessStatusCode();

                var responseResult = await response.Content.ReadFromJsonAsync<NotifyStartMatchRes>();
                if(responseResult == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return responseResult.error_code;
            }
            catch
            {
                return (WebErrorCode.TEMP_Exception);
            }
        }

        public async Task<(WebErrorCode, MatchStatus?)> GetMatchStatus(string container_id)
        {
            try
            {
                var matchStatus = await mSessionMemory.GetMatchStatus(container_id);
                if (matchStatus == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }
                return (WebErrorCode.None, matchStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMatchStatus execption : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> DispatchMatchPlayers(string container_id, List<int> players)
        {
            try
            {
                var matchStatus = await mSessionMemory.GetMatchStatus(container_id);
                if (matchStatus == null)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }
                matchStatus.players = players;

                await mSessionMemory.UpdateMatchStatus(container_id, matchStatus);

                return WebErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DispatchMatchPlayers execption : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception);
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
                status.host_port = Convert.ToInt32(newContainerInfo.NetworkSettings.Ports.ElementAt(0).Value.ElementAt(0).HostPort);
                status.container_port = Convert.ToInt32(newContainerInfo.NetworkSettings.Ports.ElementAt(0).Value.ElementAt(0).HostPort);
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

                await mSessionMemory.UpdateMatchStatus(containerId, matchStatus);

                if(false == await mDockerService.StopContainer(containerId))
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                await mDockerService.RemoveContainer(containerId);

                await mSessionMemory.RemoveMatchStatus(containerId);

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
