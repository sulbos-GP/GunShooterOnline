using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ContainerGameServer
{


    internal class Program

    {

        static async Task Main(string[] args)
        {
            const int port = 7777;
            UdpClient server = new UdpClient(port);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, port);

            DateTime start = DateTime.Now;
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));
            const int shutdown = 60;
            int shutdownCount = 0;

            SortedSet<IPEndPoint> endPoints = new SortedSet<IPEndPoint>();

            try
            {

                Console.WriteLine($"UDP Server is listening on port {port}");

                //0.1초 마다 동작
                while (await timer.WaitForNextTickAsync(CancellationToken.None))
                {
                    shutdownCount += 1;
                    Console.WriteLine("Shutting down the server after {0} seconds.", shutdown - shutdownCount);

                    var receiveTask = server.ReceiveAsync();
                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(1000));   //0.1초까지 Task감시

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


                    if (completedTask == receiveTask)
                    {
                        UdpReceiveResult receiveResult = await receiveTask;
                        byte[] receiveBytes = receiveResult.Buffer;
                        IPEndPoint clientEndPoint = receiveResult.RemoteEndPoint;

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
                    
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server Error : {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Server shut down after {0} seconds.", shutdown);
                server.Close();
            }
        }

    }
}