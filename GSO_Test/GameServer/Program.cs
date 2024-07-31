using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ContainerGameServer
{


    internal class Program
    {
        private static readonly int port = 7777;
        private static UdpClient server = new UdpClient(port);
        private static SortedSet<IPEndPoint> endPoints = new SortedSet<IPEndPoint>();

        static async Task Main(string[] args)
        {
            DateTime start = DateTime.Now;
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));
            const int shutdown = 20;



            try
            {

                Console.WriteLine($"UDP Server is listening on port {port}");

                CancellationTokenSource cancleToken = new CancellationTokenSource();
                var cancellationToken = cancleToken.Token;

                //Recive
                var receiveTask = Task.Run(() => ReceiveMessagesAsync(cancellationToken), cancellationToken);

                //1초 마다 동작
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {

                    Console.WriteLine("Shutting down the server after {0} seconds.", shutdown - (DateTime.Now - start).TotalSeconds);

                    if ((DateTime.Now - start).TotalSeconds > shutdown)
                    {
                        foreach (var endpoint in endPoints)
                        {
                            string responseData = "Quit";
                            byte[] sendBytes = Encoding.ASCII.GetBytes(responseData);
                            server.Send(sendBytes, sendBytes.Length, endpoint);
                        }
                        break;
                    }
                }

                cancleToken.Cancel();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server Error : {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Server shut down");
                server.Close();
            }
        }

        private static async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = await Task.Run(() => server.ReceiveAsync(), cancellationToken).ConfigureAwait(false);

                        byte[] receiveBytes = result.Buffer;
                        IPEndPoint clientEndPoint = result.RemoteEndPoint;

                        endPoints.Add(clientEndPoint);

                        string receiveData = Encoding.ASCII.GetString(receiveBytes);
                        Console.WriteLine("packet: {0} from {1}", receiveData, clientEndPoint.ToString());

                        //죽거나 탈출한 경우
                        if (receiveData == "Death" || receiveData == "Escape")
                        {

                            endPoints.Remove(clientEndPoint);

                            string responseData = "Quit";
                            byte[] sendBytes = Encoding.ASCII.GetBytes(responseData);
                            await server.SendAsync(sendBytes, sendBytes.Length, clientEndPoint);
                        }
                        else
                        {
                            string responseData = "Data received: " + receiveData;
                            byte[] sendBytes = Encoding.ASCII.GetBytes(responseData);
                            await server.SendAsync(sendBytes, sendBytes.Length, clientEndPoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server Reviced Error : {ex.Message}");
                    }
                }
            }

        }
    }
