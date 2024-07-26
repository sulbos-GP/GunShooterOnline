using System.Net;
using System.Net.Sockets;

namespace ContainerGameClient
{

    internal class Program

    {

        static void Main(string[] args)

        {

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 7000);
            Console.WriteLine(ep);

            client.Connect(ep);
            Console.WriteLine("Connected");

            byte[] buffer = new byte[1024];
            client.Send(new byte[] { 1, 2, 3, 4 });

            int recvBytes = client.Receive(buffer);

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();

            client.Close();

        }

    }
}