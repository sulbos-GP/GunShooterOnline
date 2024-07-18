using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using AuthenticationServer.Models;
using System.Reflection;
using Google.Apis.Oauth2.v2.Data;

namespace AuthenticationServer.Repository
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

        public async Task<Tuple<ErrorCode, long>> SignIn(String id, String service)
        {
            try
            {

                var userInfo = await mQueryFactory.Query("users").Where("id", id).FirstOrDefaultAsync<GameUser>();

                if (userInfo == null)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.LoginFailUserNotExist, 0);
                }

                if (userInfo.id != id)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.LoginFailServiceNotMatch, 0);
                }

                if (userInfo.service != service)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.LoginFailServiceNotMatch, 0);
                }

                return new Tuple<ErrorCode, long>(ErrorCode.None, userInfo.uid);
            }
            catch /*(Exception e)*/
            {
                return new Tuple<ErrorCode, long>(ErrorCode.LoginFailException, 0);
            }
        }

        public async Task<Tuple<ErrorCode, long>> SignUp(String id, String service)
        {
            try
            {

                var result = await mQueryFactory.Query("users").InsertAsync(new NewGameUser
                {
                    id = id,
                    service = service
                });

                if (result != 1)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.LoginFailUserNotExist, 0);
                }

                var userInfo = await mQueryFactory.Query("users").Select("uid").Where("id", id).FirstAsync<GameUser>();

                if (userInfo == null)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.LoginFailUserNotExist, 0);
                }

                return new Tuple<ErrorCode, long>(ErrorCode.None, userInfo.uid);
            }
            catch /*(Exception e)*/
            {
                return new Tuple<ErrorCode, long>(ErrorCode.LoginFailException, 0);
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
