using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using GsoWebServer.Reposiotry.Interfaces;
using GsoWebServer.Models.Config;
using GSO_WebServerLibrary;

namespace GsoWebServer.Reposiotry.RDB.Master
{
    public partial class MasterDB : IMasterDB
    {
        private readonly IOptions<DbConfig> mDbConfig;

        private IDbConnection mDbConn;
        private readonly SqlKata.Compilers.MySqlCompiler mCompiler;
        private readonly QueryFactory mQueryFactory;

        public MasterDB(IOptions<DbConfig> dbConfig)
        {
            mDbConfig = dbConfig;

            mDbConn = new MySqlConnection(mDbConfig.Value.MasterDB);
            Open();

            mCompiler = new SqlKata.Compilers.MySqlCompiler();
            mQueryFactory = new QueryFactory(mDbConn, mCompiler);
        }

        public void Dispose()
        {
            Close();
        }

        private void Open()
        {
            mDbConn.Open();
        }

        private void Close()
        {
            mDbConn?.Close();
        }

        public async Task<bool> LoadMasterData()
        {
            return true;
        }
    }
}
