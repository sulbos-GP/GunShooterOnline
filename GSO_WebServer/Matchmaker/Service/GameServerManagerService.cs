using Matchmaker.Service.Interfaces;
using System.Text.Json;
using System.Text;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.DTO.Matchmaker;

namespace Matchmaker.Service
{
    public class GameServerManagerService : IGameServerManagerService
    {

        private readonly IHttpClientFactory mHttpClientFactory;

        public GameServerManagerService(IHttpClientFactory httpClientFactory)
        {
            mHttpClientFactory = httpClientFactory;
        }

        public void Dispose()
        {
        }

        private HttpClient? GetGameServerManagerClient()
        {
            return mHttpClientFactory.CreateClient("GameServerManager");
        }

        public async Task<(WebErrorCode, MatchProfile?)> FetchMatchInfo()
        {
            try
            {

                var gameServerManagerClient = GetGameServerManagerClient();
                if (gameServerManagerClient == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                FetchMatchReq packet = new FetchMatchReq
                {

                };
                var content = new StringContent(JsonSerializer.Serialize(packet), Encoding.UTF8, "application/json");

                var response = await gameServerManagerClient.PostAsync("api/Session/FetchMatch", content);
                response.EnsureSuccessStatusCode();

                var fetchMatch = await response.Content.ReadFromJsonAsync<FetchMatchRes>();
                if(fetchMatch == null || fetchMatch.match_profile == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                if (fetchMatch.error_code != WebErrorCode.None)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }
                var profile = fetchMatch.match_profile;

                return (WebErrorCode.None, profile);
            }
            catch
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }
    }
}
