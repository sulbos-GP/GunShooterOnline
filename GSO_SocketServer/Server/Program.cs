using System;
using System.Net;
using WebClientCore;
using ServerCore;
using Server.Database.Handler;
using Server.Docker;
using Server.Web;
using Server.Web.Service;
using System.Threading.Tasks;
using Server.Server;

namespace Server
{
    class Program
	{
		public static GameServer gameserver = new GameServer();
        public static DatabaseHandler database = new DatabaseHandler();
        public static WebServer web = new WebServer();

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
#else
            //iPAddress = IPAddress.Loopback;
            iPAddress = IPAddress.Any;

            port = 7777;
            acceptKey = "SomeConnectionKey";
            register = 100;
            backLog = 100;
#endif
            //게임 시작
            {
                Func<Session> session = () => { return new ClientSession(); };

                IPEndPoint endPoint = new IPEndPoint(iPAddress, port);
                gameserver.Init(endPoint, session, acceptKey, register, backLog);

                gameserver.Start();
            }

            //게임 종료
            {
                int minutes = 10;
                Console.WriteLine("Shutting down the server after {0} minutes.", minutes);
                Task.Delay(minutes * 60 * 1000).Wait();

                Console.WriteLine("Stop game server");

                gameserver.Stop();
            }
        }
    }
}
