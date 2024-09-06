
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace WebClientCore
{
    public class WebClientService
    {

        private HttpClient mHttpClient;
        private Dictionary<string, Uri?> mHttpClientUris;

        public WebClientService() 
        { 
            mHttpClient = new HttpClient();
            mHttpClientUris = new Dictionary<string, Uri?>();
        }

        /// <summary>
        /// 연결할 웹 서버 Uri 추가
        /// e.g.) AddHttpClientUri("GameServerManager", "http://localhost:7000/")
        /// </summary>
        public void AddHttpClientUri(string name, string uri)
        {
            Console.WriteLine($"[AddHttpClientUri] {name} : {uri}");
            mHttpClientUris.Add(name, new Uri(uri));
        }

        /// <summary>
        /// 웹 서버에게 POST
        /// e.g.) PostAsync<RequesteMatchRes>(GameServerManager, RequestMatch, new RequesteMatchReq())
        /// </summary>
        public async Task<TResponse?> PostAsync<TResponse>(string name, string endPoint, object request) where TResponse : class
        {

            try
            {
                if (!mHttpClientUris.TryGetValue(name, out Uri? uri))
                {
                    return null;
                }

                if(uri == null)
                {
                    return null;
                }

                StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var responseMessage = await mHttpClient.PostAsync($"{uri.ToString()}api/{endPoint}", content);

                responseMessage.EnsureSuccessStatusCode();

                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                if (jsonResponse == null)
                {
                    return null;
                }

                TResponse? message = JsonSerializer.Deserialize<TResponse>(jsonResponse);
                if (message == null)
                {
                    return null;
                }

                return message;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[WebClient::PostAsync] : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 웹 서버에게 GET
        /// e.g.) PostAsync<FetchMatchRes>(GameServerManager, FetchMatch)
        /// </summary>
        public async Task<TResponse?> GetAsync<TResponse>(string name, string endPoint) where TResponse : class
        {
            try
            {
                if (!mHttpClientUris.TryGetValue(name, out Uri? uri))
                {
                    return null;
                }

                if (uri == null)
                {
                    return null;
                }

                var responseMessage = await mHttpClient.GetAsync($"{uri.ToString()}/api/{endPoint}");

                responseMessage.EnsureSuccessStatusCode();

                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                if (jsonResponse == null)
                {
                    return null;
                }

                TResponse? message = JsonSerializer.Deserialize<TResponse>(jsonResponse);
                if (message == null)
                {
                    return null;
                }

                return message;

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[WebClient::GetAsync] : {ex.Message}");
                return null;
            }
        }
    }
}
