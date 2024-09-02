using Server.Docker;
using System;
using System.Threading.Tasks;
using WebCommonLibrary.DTO.GameServer;

namespace Server.Web.Service
{
    public class ServerManagerResource : WebResource
    {
        public ServerManagerResource() : base("GameServerManager")
        { 
        
        }

        public async Task<RequestReadyMatchRes> PostRequestReady()
        {

            Console.WriteLine("[RequestReady GameServer]");

            ShutdownMatchReq request = new ShutdownMatchReq
            {
                container_id = DockerUtil.GetContainerId(),
            };

            return await WebManager.Instance.WebClient.PostAsync<RequestReadyMatchRes>(host, "Session/RequestReady", request);
        }

        public async Task<ShutdownMatchRes> PostShutdown()
        {

            Console.WriteLine("[Shutdown GameServer]");

            ShutdownMatchReq request = new ShutdownMatchReq
            {
                container_id = DockerUtil.GetContainerId(),
            };

            return await WebManager.Instance.WebClient.PostAsync<ShutdownMatchRes>(host, "Session/Shutdown", request);
        }
    }
}
