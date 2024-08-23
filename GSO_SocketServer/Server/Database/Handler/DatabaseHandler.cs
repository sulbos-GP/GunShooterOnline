using Server.Database.Game;
using Server.Database.Master;
using System;
using System.Collections.Generic;

namespace Server.Database.Handler
{
    public enum EDatabase
    {
        None,
        Game,
        Master,
    }

    public class DatabaseHandler
    {
        #region Singleton
        static DatabaseHandler _instance = new DatabaseHandler();
        public static DatabaseHandler Instance { get { return _instance; } }
        #endregion

        private static Dictionary<EDatabase, MySQL> databases = new Dictionary<EDatabase, MySQL>(10);
        #region Database
        public static GameDB GameDB { get { return (GameDB)databases[EDatabase.Game]; } }
        public static MasterDB MasterDB { get { return (MasterDB)databases[EDatabase.Master]; } }
        #endregion

        public DatabaseHandler()
        {

        }

        public void AddMySQL<TMySQL>(EDatabase type, string connectionString) where TMySQL : MySQL, new()
        {
            try
            {
                MySQL database = new TMySQL();

                database.Open(connectionString);

                if (database.isOpen())
                {
                    databases.TryAdd(type, database);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AddDatabaseConnectionPool] : {ex.Message.ToString()}");
            }

        }

        public void RemoveMySQL()
        {
            try
            {
                foreach (var (type, database) in databases)
                {
                    if (database.isOpen())
                    {
                        database.Close();
                    }

                    databases.Remove(type);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CloseDatabases] : {ex.Message.ToString()}");
            }
        }
    }
}
