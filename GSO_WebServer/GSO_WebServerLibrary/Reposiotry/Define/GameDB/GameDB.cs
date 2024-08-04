using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using GSO_WebServerLibrary;
using System.Data.Common;
using GSO_WebServerLibrary.Config;
using GSO_WebServerLibrary.Reposiotry.Interfaces;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        private IDbConnection mConnection;
        private readonly SqlKata.Compilers.MySqlCompiler mCompiler;
        private readonly QueryFactory mQueryFactory;

        public GameDB(IOptions<DatabaseConfig> dbConfig)
        {
            mConnection = new MySqlConnection(dbConfig.Value.GameDB);
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
