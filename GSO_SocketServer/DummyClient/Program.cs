using System;
using System.Net;
using System.Threading;
using ServerCore;

namespace DummyClient
{
	

	class Program
	{

        public static ClientNetworkService mNetworkService = new ClientNetworkService();

        static void Main(string[] args)
		{

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 9050);

            Func<Session> session = () => { return new ServerSession(); };

            mNetworkService.Init(endPoint, session, "SomeConnectionKey");
            mNetworkService.Start();

            Console.WriteLine("q: Quit Client.");
            while (true)
            {
                string input = Console.ReadLine();
                if (input.Equals("q"))
                {
                    mNetworkService.Stop();
                    break;
                }
            }

            //while (true)
            //{
            //	try
            //	{
            //		SessionManager.Instance.SendForEach();
            //	}
            //	catch (Exception e)
            //	{
            //		Console.WriteLine(e.ToString());
            //	}

            //	Thread.Sleep(250);
            //}
        }
	}
}
