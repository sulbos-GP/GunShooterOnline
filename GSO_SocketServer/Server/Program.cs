using System;
using System.Net;
using WebClientCore;
using Server.Docker;

using ServerCore;
using Server.DTO;
using System.Threading.Tasks;

namespace Server
{

	class Program
	{
		public static ServerNetworkService mNetworkService = new ServerNetworkService();
        public static WebClientService mWebClientService = new WebClientService();

        public static ushort ServerTickCount { get; internal set; } = 0;
        public static int ServerIntervalTick { get; internal set; } = 250;



        public void OnLogicUpdate()
        {
            while (true)
            {
             
            }
        }

        static void Main(string[] args)
		{

            IPAddress iPAddress;
            int port = 0;
            string acceptKey = string.Empty;
            int register = 0;
            int backLog = 0;

#if DOCKER
            iPAddress   = IPAddress.Any;
            port        = DockerUtil.GetHostPort();
            acceptKey   = DockerUtil.GetContainerId();
            register    = DockerUtil.GetRegister();
            backLog     = DockerUtil.GetBacklog();

            InitWebClientService();
#else

            iPAddress = IPAddress.Loopback;
            port = 7777;
            acceptKey = "SomeConnectionKey";
            register = 100;
            backLog = 100;
#endif

            Func<Session> session = () => { return new ClientSession(); };

            IPEndPoint endPoint = new IPEndPoint(iPAddress, port);
            mNetworkService.Init(endPoint, session, acceptKey, register, backLog);
            
            BattleGameRoom room = new BattleGameRoom();
            
            
           // mNetworkService.Init(endPoint, "SomeConnectionKey", 100, 100);
            //mNetworkService.SetChannel(endPoint, "SomeConnectionKey", 100, 100);
            mNetworkService.Start();
            mNetworkService.SetChannel(true, room,0);

#if DOCKER

            RequestReady().Wait();

            int minutes = 20;
            Console.WriteLine("Shutting down the server after {0} minutes.", minutes);

            Task.Delay(minutes * 60 * 1000).Wait();

            mNetworkService.Stop();

            Shutdown().Wait();
#else
            Console.WriteLine("q: Quit Server.");
            while (true)
            {
                string input = Console.ReadLine();
                if (input.Equals("q"))
                {
                    mNetworkService.Stop();
                    break;
                }
            }
#endif




            ////FlushRoom();
            //JobTimer.Instance.Push(FlushRoom);

            //while (true)
            //{
            //	JobTimer.Instance.Flush();
            //}
        }

        static void InitWebClientService()
        {
            mWebClientService.AddHttpClientUri("GameServerManager", $"http://{DockerUtil.GetHostIP()}:7000");
        }

        //임시
        static async Task RequestReady()
        {
            /// <summary>
            /// 서버에서 유저를 받아줄 준비를 모두 마쳤다면 호출
            /// </summary>
            /// 
            Console.WriteLine("[RequestReady GameServer]");

            RequestReadyMatchReq request = new RequestReadyMatchReq
            {
                container_id = DockerUtil.GetContainerId(),
            };

            var response = await mWebClientService.PostAsync<RequestReadyMatchRes>("GameServerManager", "Session/RequestReady", request);
        }

        static async Task Shutdown()
        {
            /// <summary>
            /// 서버에서 유저를 받아줄 준비를 모두 마쳤다면 호출
            /// </summary>
            Console.WriteLine("[Shutdown GameServer]");

            ShutdownMatchReq request = new ShutdownMatchReq
            {
                container_id = DockerUtil.GetContainerId(),
            };

            var response = await mWebClientService.PostAsync<ShutdownMatchRes>("GameServerManager", "Session/Shutdown", request);
        }

    }
}
