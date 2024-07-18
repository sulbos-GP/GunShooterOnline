using System;
using System.Net;
using ServerCore;

namespace Server
{
	class Program
	{

		public static ServerNetworkService mNetworkService = new ServerNetworkService();

		static void Main(string[] args)
		{
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 9050);

            //Func<Session> session = () => { return new ClientSession(); };
            
            //NetworkService.Init(endPoint, session, "SomeConnectionKey", 100, 100);
            mNetworkService.Init(endPoint, "SomeConnectionKey", 100, 100);
            mNetworkService.Start();

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
