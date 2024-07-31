using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ContainerGameClient
{

    internal class Program

    {

        static async Task Main(string[] args)

        {

            UdpClient udpClient = new UdpClient();
            try
            {
                udpClient.Connect("127.0.0.1", 7777);
                Console.WriteLine("Connect to the server");

                string? message;
                while ((message = Console.ReadLine()) != "exit")
                {
                    if (message == null)
                    {
                        continue;
                    }

                    byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                    await udpClient.SendAsync(sendBytes, sendBytes.Length);

                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    string receivedMessage = Encoding.ASCII.GetString(result.Buffer);

                    if (receivedMessage == "Quit")
                    {
                        break;
                    }

                    Console.WriteLine("Received from server: " + receivedMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client Error : {ex.ToString()}");
            }
            finally
            {
                Console.WriteLine("Client shut down");
                udpClient.Close();
            }
        }

    }
}