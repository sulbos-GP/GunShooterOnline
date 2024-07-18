using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using GsoWebServer.Reposiotry.Interfaces;
using GsoWebServer.Models.Config;
using GSO_WebServerLibrary;
using System.Data.Common;

namespace GsoWebServer.Reposiotry.RDB.Game
{
    public partial class GameDB : IGameDB
    {
        private readonly IOptions<DbConfig> mDbConfig;

        private IDbConnection mConnection;
        private readonly SqlKata.Compilers.MySqlCompiler mCompiler;
        private readonly QueryFactory mQueryFactory;

        public GameDB(IOptions<DbConfig> dbConfig)
        {
            mDbConfig = dbConfig;

            mConnection = new MySqlConnection(mDbConfig.Value.GameDB);
            Open();

            mCompiler = new SqlKata.Compilers.MySqlCompiler();
            mQueryFactory = new QueryFactory(mConnection, mCompiler);
        }

        public void Dispose()
        {
            Close();
        }

        private void Open()
        {
            mConnection.Open();
        }

        private void Close()
        {
            mConnection.Close();
        }

        public IDbConnection GetConnection()
        {
            return mConnection;
        }
    }
}
