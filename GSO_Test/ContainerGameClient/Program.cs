using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ContainerGameClient
{

    internal class Program
    {
        static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public static void RecvAsync(IAsyncResult result)
        {

            int size = client.EndReceive(result);

            if (size > 0)
            {

                if (result.AsyncState == null)
                {
                    return;
                }

                byte[] recv = new byte[1024];
                recv = (byte[])result.AsyncState;

                string message = Encoding.UTF8.GetString(recv, 0, size);
                Console.WriteLine(message);

            }
        }

        static void Main(string[] args)
        {
            int port = 7002;

            IPEndPoint mRemoteIpEndPoint = new IPEndPoint(IPAddress.Loopback, port);

            client.Connect(mRemoteIpEndPoint);

            byte[] buffer = new byte[1024];

            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecvAsync), buffer);

            while (true)
            {
                Console.Write("message : ");
                string? message = Console.ReadLine();
                if (message == null)
                {
                    break;
                }
                byte[] send = Encoding.UTF8.GetBytes(message);

                client.SendTo(send, send.Length, SocketFlags.None, mRemoteIpEndPoint);

                if(message == "exit")
                {
                    break;
                }

            }

            client.Close();
        }

    }
}