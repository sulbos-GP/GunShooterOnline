using MySqlConnector;
using Server.Database.Interface;
using SqlKata.Execution;

namespace Server.Database.Game
{
    public partial class GameDB : MySQL, IGameDatabase
    {

        public GameDB() : base()
        {
            
        }

    }

}
