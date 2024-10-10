using Matchmaker.Service.Interfaces;
using System.Text.Json;
using System.Text;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.Match;
using WebCommonLibrary.DTO.GameServer;
using WebCommonLibrary.DTO.Matchmaker;
using GSO_WebServerLibrary.Utils;

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

        public async Task<(WebErrorCode, MatchProfile?)> FetchMatchInfo(Dictionary<string, Ticket> players)
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
                    playerCount = players.Count,
                };
                var content = new StringContent(JsonSerializer.Serialize(packet), Encoding.UTF8, "application/json");

                var response = await gameServerManagerClient.PostAsync("api/Session/FetchMatch", content);
                response.EnsureSuccessStatusCode();

                var fetchMatch = await response.Content.ReadFromJsonAsync<FetchMatchRes>();
                if(fetchMatch == null || fetchMatch.error_code != WebErrorCode.None)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                return (WebErrorCode.None, fetchMatch.match_profile);
            }
            catch
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> DispatchMatchPlayers(Dictionary<string, Ticket> players, MatchProfile matchProfile)
        {
            try
            {

                var gameServerManagerClient = GetGameServerManagerClient();
                if (gameServerManagerClient == null)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }


                List<int> playerList = new List<int>();
                foreach (var key in players.Keys.ToList())
                {
                    int uid = KeyUtils.GetUID(key);
                    playerList.Add(uid);
                }

                DispatchMatchPlayerReq packet = new DispatchMatchPlayerReq
                {
                    match_profile = matchProfile,
                    players = playerList
                };
                var content = new StringContent(JsonSerializer.Serialize(packet), Encoding.UTF8, "application/json");

                var response = await gameServerManagerClient.PostAsync("api/Session/DispatchMatchPlayers", content);
                response.EnsureSuccessStatusCode();

                var fetchMatch = await response.Content.ReadFromJsonAsync<FetchMatchRes>();
                if (fetchMatch == null || fetchMatch.error_code != WebErrorCode.None)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }

                return (WebErrorCode.None);
            }
            catch
            {
                return (WebErrorCode.TEMP_Exception);
            }
        }
    }
}
