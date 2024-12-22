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
using System.Threading;

namespace Server
{
    class Program
	{
		public static GameServer gameserver = new GameServer();
        public static DatabaseHandler database = new DatabaseHandler();
        public static WebServer web = new WebServer();


        public static ulong ServerTickCount
        {
            get
            {
                return LogicTimer.Tick;/// 10000.0; //50 => 1초
            }
          /*  set
            {
                LogicTimer._time = (ulong)(value * 10000.0);
                Console.WriteLine(LogicTimer._time);

            }*/
        }


        public static int mFramesPerSecond { get; internal set; } = LogicTimer.mFramesPerSecond; //50ms
        public static int ServerIntervalTick { get; internal set; } = 1000 / mFramesPerSecond;

        public static int minutes = 10;

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

                if(false == gameserver.Start())
                {
                    Console.WriteLine("게임 실행 중 에러 발생");
                    return;
                }

            }

            //게임 종료
#if DEBUG
            

              /*  while(true)
                {
                    Console.WriteLine("Shutting down the server after {0} minutes.", minutes);

                    //Task.Delay(minutes * 60 * 1000).Wait();
                    //Task.Delay(60000).Wait();

                    Console.WriteLine("Stop game server");
                    gameserver.ResetServer();
                    //gameserver.Stop();
                }*/


#endif
        }
    }
}
