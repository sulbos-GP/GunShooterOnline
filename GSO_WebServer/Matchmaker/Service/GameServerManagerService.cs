using GSO_WebServerLibrary.DTO.Match;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Models.Match;
using Matchmaker.Models;
using Matchmaker.Service.Interfaces;
using System.Net.Http;

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

                var response = await gameServerManagerClient.GetAsync("FetchMatch");
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
