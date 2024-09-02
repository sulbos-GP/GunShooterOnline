using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Error;

namespace GsoWebServer.Servicies.Interfaces
{
    public interface IDataLoadService : IDisposable
    {
        public Task<(WebErrorCode, DataLoadUserInfo?)> LoadUserData(int uid);
    }
}
