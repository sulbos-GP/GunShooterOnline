using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;


namespace ContainerGameServer
{
    public class ErrorCodeDTO
    {
        public ushort error_code { get; set; } = 0;

        public string error_description { get; set; } = string.Empty;
    }

    public class RequestReadyMatchReq
    {
        public string container_id { get; set; } = string.Empty;
    }

    public class RequestReadyMatchRes : ErrorCodeDTO
    {
    }

    public class ShutdownMatchReq
    {
        public string container_id { get; set; } = string.Empty;
    }

    public class ShutdownMatchRes : ErrorCodeDTO
    {
    }

    public class Program
    {
        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static SortedSet<IPEndPoint> mEndPoints = new SortedSet<IPEndPoint>();

        public static void RecvAsync(IAsyncResult result)
        {

            int size = server.EndReceive(result);

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

            byte[] buffer = new byte[1024];
            server.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecvAsync), buffer);
        }
        public static async Task<bool> ServerAsync(CancellationToken cancellationToken)
        {

            long shutdownTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 60;
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                long elapsedSeconds = shutdownTimeStamp - now;
                Console.WriteLine("Shutting down the server after {0} seconds.", elapsedSeconds);

                if (elapsedSeconds <= 0)
                {
                    break;
                }

            }

            return true;
        }
        public static int GetEnvPort()
        {
            string? portEnv = Environment.GetEnvironmentVariable("PORT");
            if (portEnv == null)
            {
                return 0;
            }
            return int.Parse(portEnv);
        }

        public static string GetEnvIp()
        {
            string? ip = Environment.GetEnvironmentVariable("HOST_IP");
            if (ip == null)
            {
                return string.Empty;
            }
            return ip;
        }

        public static string GetContainerId()
        {
            string[] cgroupLines = File.ReadAllLines("/proc/self/cgroup");
            string? cgroupLine = cgroupLines.FirstOrDefault(line => line.Contains("cpu:"));

            if (cgroupLine != null)
            {
                return cgroupLine.Split('/').Last();
            }

            return string.Empty;
        }

        public static async Task<HttpResponseMessage?> SendGameServerManager(object? request, string endPoint)
        {
            string host = GetEnvIp();
            if (host == string.Empty)
            {
                return null;
            }

            HttpClient client = new HttpClient();
            StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            return await client.PostAsync($"http://{host}:7000/api/Session/{endPoint}", content);

        }

        static async Task Main(string[] args)
        {

            //게임 서버 열기
            try
            {

                int port = GetEnvPort();
                if (port == 0)
                {
                    return;
                }

                IPEndPoint mLocalIpEndPoint = new IPEndPoint(IPAddress.Any, port);
                server.Bind(mLocalIpEndPoint);

                byte[] buffer = new byte[1024];

                server.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecvAsync), buffer);

                Console.WriteLine($"UDP Server is listening on port {port}");


                //게임 준비상태 완료 요청
                {
                    RequestReadyMatchReq request = new RequestReadyMatchReq
                    {
                        container_id = GetContainerId()
                    };

                    var response = await SendGameServerManager(request, "RequestReady");
                    if (response == null)
                    {
                        return;
                    }

                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (jsonResponse == null)
                    {
                        return;
                    }

                    var message = JsonSerializer.Deserialize<RequestReadyMatchRes>(jsonResponse);
                    if (message == null)
                    {
                        return;
                    }

                    string result = message.error_code == 0 ? "successful" : "failure";
                    Console.WriteLine($"RequestReady response : {result}\n");
                }

                var cts = new CancellationTokenSource();
                await ServerAsync(cts.Token);
                cts.Cancel();


                //셧다운 요청
                {
                    ShutdownMatchReq request = new ShutdownMatchReq
                    {
                        container_id = GetContainerId()
                    };

                    var response = await SendGameServerManager(request, "Shutdown");
                    if (response == null)
                    {
                        return;
                    }

                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (jsonResponse == null)
                    {
                        return;
                    }

                    var message = JsonSerializer.Deserialize<RequestReadyMatchRes>(jsonResponse);
                    if (message == null)
                    {
                        return;
                    }

                    string result = message.error_code == 0 ? "successful" : "failure";
                    Console.WriteLine($"Shutdown response : {result}\n");
                }

            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("The requested resource was not found.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
            finally
            {
                if (server != null)
                {
                    server.Close();
                }
            }
        }

    }
}