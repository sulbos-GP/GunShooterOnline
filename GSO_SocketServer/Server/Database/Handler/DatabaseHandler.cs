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

        private static Dictionary<EDatabase, string> databases = new Dictionary<EDatabase, string>(10);
        #region Database
        public static GameDB GameDB 
        { 
            get
            {
                GameDB database = new GameDB();
                database.Open(databases[EDatabase.Game]);
                return database; 
            } 
        }

        public static MasterDB MasterDB 
        { 
            get 
            {
                MasterDB database = new MasterDB();
                database.Open(databases[EDatabase.Master]);
                return database;
            }
        }
        #endregion

        public DatabaseHandler()
        {

        }

        public void AddMySQL<TMySQL>(EDatabase type, string connectionString) where TMySQL : MySQL, new()
        {
            databases[type] = connectionString;
        }

        public void RemoveMySQL()
        {

        }
    }
}
