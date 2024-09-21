using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using WebCommonLibrary.Config;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.Models.MasterDB;
using WebCommonLibrary.Error;
using WebCommonLibrary.Reposiotry.MasterDatabase;

namespace GSO_WebServerLibrary.Reposiotry.Define.MasterDB
{
    public partial class MasterDB : IMasterDB
    {
        private IDbConnection mDbConn;
        private readonly SqlKata.Compilers.MySqlCompiler mCompiler;
        private readonly QueryFactory mQueryFactory;

        public MasterDatabaseContext Context { get; }

        public MasterDB(IOptions<DatabaseConfig> dbConfig)
        {
            Context = new MasterDatabaseContext(dbConfig.Value.MasterDB);
            mDbConn = new MySqlConnection(dbConfig.Value.MasterDB);
            Open();

            mCompiler = new SqlKata.Compilers.MySqlCompiler();
            mQueryFactory = new QueryFactory(mDbConn, mCompiler);
        }

        public async Task<bool> LoadMasterDatabase()
        {

            if(false == Context.IsValidContext())
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
