using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using WebCommonLibrary.Config;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.Models.MasterDB;
using WebCommonLibrary.Error;

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

        public async Task<bool> LoadMasterDatabase()
        {
            if(false == await LoadMasterTables())
            {
                return false;
            }

            if (WebErrorCode.None != await LoadUserRating())
            {
                return false;
            }

            return true;
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
