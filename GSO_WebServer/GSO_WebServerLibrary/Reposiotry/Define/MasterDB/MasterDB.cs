using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using WebCommonLibrary.Config;
using GSO_WebServerLibrary.Reposiotry.Interfaces;

namespace GSO_WebServerLibrary.Reposiotry.Define.MasterDB
{
    public partial class MasterDB : IMasterDB
    {
        private IDbConnection mDbConn;
        private readonly SqlKata.Compilers.MySqlCompiler mCompiler;
        private readonly QueryFactory mQueryFactory;

        public MasterDB(IOptions<DatabaseConfig> dbConfig)
        {

            mDbConn = new MySqlConnection(dbConfig.Value.MasterDB);
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

    }
}
