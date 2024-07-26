using System.Net;
using System.Net.Sockets;


namespace ContainerGameServer
{


    internal class Program

    {

        static void Main(string[] args)
        {

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 7000);

            Console.WriteLine(ep);

            socket.Bind(ep);

            socket.Listen(10);

            new Thread(() =>
            {

                byte[] buffer = new byte[10];

                while (true)
                {
                    Socket client = socket.Accept();
                    Console.WriteLine("Client connected");

                    int recvBytes = client.Receive(buffer);

                    client.Send(new byte[] { 1, 2, 3, 4 });
                    client.Close();
                }

            }).Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();

            socket.Close();

        }

    }
}