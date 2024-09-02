using GSO_WebServerLibrary.Error;
using GsoWebServer.DTO.DataLoad;
using static Google.Apis.Requests.RequestError;

namespace GsoWebServer.Servicies.Interfaces
{
    public interface IDataLoadService : IDisposable
    {
        public Task<(WebErrorCode, DataLoadUserInfo?)> LoadUserData(int uid);
    }
}
