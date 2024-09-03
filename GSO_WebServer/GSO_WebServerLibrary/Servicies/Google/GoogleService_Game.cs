using Google.Apis.Auth.OAuth2;
using Google.Apis.Games.v1.Data;
using Google.Apis.Games.v1;
using Google.Apis.Services;
using WebCommonLibrary.Error;
using GSO_WebServerLibrary.Servicies.Interfaces;

namespace GSO_WebServerLibrary.Servicies.Google
{
    public partial class GoogleService : IGoogleService
    {
        public async Task<(WebErrorCode, Player?)> GetMyPlayer(String userId, String accessToken)
        {
            try
            {
                var credential = GoogleCredential.FromAccessToken(accessToken);

                var service = new GamesService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = mGoogleConfig.Value.ApplicationName
                });

                PlayersResource.GetRequest request = service.Players.Get(userId);
                Player response = await request.ExecuteAsync();

                //if (response.GamePlayerId != userId)
                //{
                //    return (WebErrorCode.MyPlayerIdMismatch, null);
                //}

                return (WebErrorCode.None, response);
            }
            catch /*(Exception ex)*/
            {
                return (WebErrorCode.MyPlayerFailException, null);
            }
        }
    }
}