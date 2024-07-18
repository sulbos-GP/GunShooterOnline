using GSO_WebServerLibrary;
using GsoWebServer.DTO.DataLoad;
using static Google.Apis.Requests.RequestError;

namespace GsoWebServer.Servicies.Interfaces
{
    public interface IDataLoadService
    {
        public Task<(WebErrorCode, DataLoadUserInfo?)> LoadUserData(int uid);
    }
}
