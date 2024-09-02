using System;
using System.Net;
using WebClientCore;
using ServerCore;
using Server.Database.Handler;

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
            Console.WriteLine("Start Server.");
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

            var webManager = WebManager.Instance;
            webManager.ConfigureServices();

#else


            iPAddress = IPAddress.Loopback;
            port = 7777;
            acceptKey = "SomeConnectionKey";
            register = 100;
            backLog = 100;
#endif

            var database = DatabaseHandler.Instance;
            database.InitMySQL();

            Func<Session> session = () => { return new ClientSession(); };

            IPEndPoint endPoint = new IPEndPoint(iPAddress, port);
            mNetworkService.Init(endPoint, session, acceptKey, register, backLog);
            
            BattleGameRoom room = new BattleGameRoom();

            //mNetworkService.Init(endPoint, "SomeConnectionKey", 100, 100);
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


            while (true)
            {

            }

            ////FlushRoom();
            //JobTimer.Instance.Push(FlushRoom);

            //while (true)
            //{
            //	JobTimer.Instance.Flush();
            //}
        }
    }
}
