using MySqlConnector;
using SqlKata.Execution;
using System;
using System.Data;
using System.Data.Common;

namespace Server.Database
{

    public class MySQL : IDisposable
    {
        private IDbConnection connection;
        private SqlKata.Compilers.MySqlCompiler compiler;
        private QueryFactory queryFactory;

        public MySQL() 
        {

        }

        public void Dispose()
        {
            Close();
        }

        public bool isOpen()
        {
            return connection.State == ConnectionState.Open;
        }

        public void Open(string connectionString)
        {
            connection = new MySqlConnection(connectionString);

            connection.Open();

            compiler = new SqlKata.Compilers.MySqlCompiler();
            queryFactory = new QueryFactory(connection, compiler);
        }

        public void Close()
        {
            connection.Close();
        }

        protected IDbConnection GetConnection()
        {
            return connection;
        }

        protected SqlKata.Compilers.MySqlCompiler GetCompier()
        {
            return compiler;
        }

        protected QueryFactory GetQueryFactory()
        {
            return queryFactory;
        }

    }
}
