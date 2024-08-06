using GsoWebServer.Reposiotry.Interfaces;
using GsoWebServer.Servicies.Interfaces;

namespace GsoWebServer.Servicies.Game
{
    public partial class GameService : IGameService
    {
        private readonly IGameDB mGameDB;
        public GameService(IGameDB gameDB)
        {
            mGameDB = gameDB;
        }

        public void Dispose()
        {

        }
    }
}
