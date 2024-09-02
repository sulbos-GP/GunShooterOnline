using Server.Database.Game;
using Server.Database.Master;
using Server.Web;
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
        static DatabaseHandler instance;

        public static DatabaseHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseHandler();
                }
                return instance;
            }
        }
        #endregion

        private static Dictionary<EDatabase, string> databases = new Dictionary<EDatabase, string>(10);
        private static DatabaseContext context = new DatabaseContext();

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


        #region Context

        public static DatabaseContext Context
        {
            get
            {
                return context;
            }
        }
        #endregion

        public void AddMySQL<TMySQL>(EDatabase type, string connectionString) where TMySQL : MySQL, new()
        {
            databases[type] = connectionString;
        }

        public void InitMySQL()
        {
#if DOCKER
            instance.AddMySQL<GameDB>(EDatabase.Game, "Server=127.0.0.1;user=root;Password=!Q2w3e4r;Database=game_database;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;");
            instance.AddMySQL<MasterDB>(EDatabase.Master, "Server=127.0.0.1;user=root;Password=!Q2w3e4r;Database=master_database;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;");
#else
            instance.AddMySQL<GameDB>(EDatabase.Game, "Server=127.0.0.1;user=root;Password=!Q2w3e4r;Database=game_database;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;");
            instance.AddMySQL<MasterDB>(EDatabase.Master, "Server=127.0.0.1;user=root;Password=!Q2w3e4r;Database=master_database;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;");
#endif

            context.LoadDatabaseContext().Wait();
        }

        public void RemoveMySQL()
        {

        }
    }
}
