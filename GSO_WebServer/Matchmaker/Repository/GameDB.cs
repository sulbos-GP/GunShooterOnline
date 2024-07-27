using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using Matchmaker.Models;
using GSO_WebServerLibrary;

namespace Matchmaker.Repository
{
    public class GameDB : IGameDB
    {
        private readonly IOptions<DbConfig> mDbConfig;
        private IDbConnection? mDbConn;
        private readonly SqlKata.Compilers.MySqlCompiler mCompiler;
        private readonly QueryFactory mQueryFactory;

        public GameDB(IOptions<DbConfig> dbConfig)
        {
            mDbConfig = dbConfig;

            Open();

            mCompiler = new SqlKata.Compilers.MySqlCompiler();
            mQueryFactory = new QueryFactory(mDbConn, mCompiler);
        }

        public void Dispose()
        {
            Close();
        }

        public async Task<Tuple<WebErrorCode, Int64>> GetUID(String userid)
        {
            try
            {
                var uid = await mQueryFactory.Query("users").Select("uid").Where("id", userid).FirstOrDefaultAsync<long>();

                if (uid == 0)
                {
                    return new Tuple<WebErrorCode, long>(WebErrorCode.LoginFailUserNotExist, 0);
                }

                return new Tuple<WebErrorCode, long>(WebErrorCode.None, uid);
            }
            catch /*(Exception e)*/
            {
                return new Tuple<WebErrorCode, long>(WebErrorCode.LoginFailException, 0);
            }
        }

        public async Task<Tuple<WebErrorCode, PlayerSkill?>> GetUserSkill(Int64 uid)
        {

            try
            {
                var userSkills = await mQueryFactory.Query("skills").Select("rating", "deviation", "volatility").Where("uid", uid).FirstOrDefaultAsync<PlayerSkill>();

                if (userSkills == null)
                {
                    return new Tuple<WebErrorCode, PlayerSkill?>(WebErrorCode.LoginFailUserNotExist, null);
                }

                return new Tuple<WebErrorCode, PlayerSkill?>(WebErrorCode.None, userSkills);
            }
            catch /*(Exception e)*/
            {
                return new Tuple<WebErrorCode, PlayerSkill?>(WebErrorCode.LoginFailException, null);
            }
        }

        private void Open()
        {
            mDbConn = new MySqlConnection(mDbConfig.Value.GameDB);

            mDbConn.Open();
        }

        private void Close()
        {
            mDbConn?.Close();
        }
    }
}
