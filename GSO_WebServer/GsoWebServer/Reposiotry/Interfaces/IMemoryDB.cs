using GSO_WebServerLibrary;
using static Google.Apis.Requests.RequestError;

namespace GsoWebServer.Reposiotry.Interfaces
{
    public interface IMemoryDB : IDisposable
    {
        public Task<WebErrorCode> RegisterToken(Int32 uid, Int64 expires, String accessToken, String refreshToken);
        //public Task<WebErrorCode> CheckUserAuthAsync(String uid, String accessToken);
    }
}
