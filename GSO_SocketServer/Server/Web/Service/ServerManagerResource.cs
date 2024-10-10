using Server.Docker;
using System;
using System.Threading.Tasks;
using WebCommonLibrary.DTO.GameServer;
using WebCommonLibrary.DTO.Matchmaker;

namespace Server.Web.Service
{
    public class ServerManagerResource : WebResource
    {
        public ServerManagerResource(WebServer owner, string name) : base(owner, name)
        { 
        
        }

        public async Task<RequestReadyMatchRes> PostRequestReady()
        {

            Console.WriteLine("[RequestReady GameServer]");

            RequestReadyMatchReq request = new RequestReadyMatchReq
            {
                container_id = DockerUtil.GetContainerId(),
            };

            return await Owner.PostAsync<RequestReadyMatchRes>(Host, "Session/RequestReady", request);
        }

        public async Task<ShutdownMatchRes> PostShutdown()
        {

            Console.WriteLine("[Shutdown GameServer]");

            ShutdownMatchReq request = new ShutdownMatchReq
            {
                container_id = DockerUtil.GetContainerId(),
            };

            return await Owner.PostAsync<ShutdownMatchRes>(Host, "Session/Shutdown", request);
        }

        public async Task<AllocateMatchRes> PostWaitForAllocateMatch()
        {

            //Console.WriteLine("[StartMatch GameServer]");

            AllocateMatchReq request = new AllocateMatchReq
            {
                container_id = DockerUtil.GetContainerId(),
            };

            return await Owner.PostAsync<AllocateMatchRes>(Host, "Session/AllocateMatch", request);
        }

        public async Task<StartMatchRes> PostStartMatch()
        {

            Console.WriteLine("[StartMatch GameServer]");

            StartMatchReq request = new StartMatchReq
            {
                container_id = DockerUtil.GetContainerId(),
            };

            return await Owner.PostAsync<StartMatchRes>(Host, "Session/StartMatch", request);
        }


    }
}
