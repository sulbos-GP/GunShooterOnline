using GSO_WebServerLibrary.Reposiotry.Define.MasterDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GsoWebServer.Servicies.Interfaces;

namespace GsoWebServer.Servicies.Game
{
    public partial class GameService : IGameService
    {
        private readonly IMasterDB mMasterDB;
        private readonly IGameDB mGameDB;

        public GameService(IMasterDB masterDB, IGameDB gameDB)
        {
            mMasterDB = masterDB;
            mGameDB = gameDB;
        }

        public void Dispose()
        {

        }
    }
}
