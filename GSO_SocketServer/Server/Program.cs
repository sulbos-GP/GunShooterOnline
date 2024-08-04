using System;
using System.Net;
using System.Threading;
using QuadTree;
using Server.Game;

using ServerCore;

namespace Server
{
	class Program
	{

        Thread t;


		public static ServerNetworkService mNetworkService = new ServerNetworkService();

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

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);

            //IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 7777);

            Func<Session> session = () => { return new ClientSession(); };

            mNetworkService.Init(endPoint, session, "SomeConnectionKey", 100, 100);
            
            BattleGameRoom room = new BattleGameRoom();
            
            
           // mNetworkService.Init(endPoint, "SomeConnectionKey", 100, 100);
            //mNetworkService.SetChannel(endPoint, "SomeConnectionKey", 100, 100);
            mNetworkService.Start();
            mNetworkService.SetChannel(true, room,0);



            Console.WriteLine("q: Quit Server.");
            while (true)
            {
                string input = Console.ReadLine();
                if(input.Equals("q"))
                {
                    mNetworkService.Stop();
                    break;
                }
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
