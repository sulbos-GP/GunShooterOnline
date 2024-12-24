using Server.Database.Game;
using Server.Database.Master;
using Server.Docker;
using Server.Web;
using System;
using System.Collections.Generic;
using WebCommonLibrary.Reposiotry.MasterDatabase;

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
        private static Dictionary<EDatabase, string> databases = new Dictionary<EDatabase, string>(10);
        private static MasterDatabaseContext context;

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

        public static MasterDatabaseContext Context 
        { 
            get 
            {
                return context;
            }
        }
        #endregion

        public DatabaseHandler()
        {

        }

        public void AddMySQL<TMySQL>(EDatabase type, string server, string password, string database) where TMySQL : MySQL, new()
        {
            string connection = $"Server={server};user=root;Password={password};Database={database};Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True";
            databases[type] = connection;
        }

        public void initializeAndLoadData()
        {

#if DOCKER

            string server = DockerUtil.GetHostIP();
            string password = "!Q2w3e4r";

            AddMySQL<GameDB>(EDatabase.Game, server, password, "game_database");
            AddMySQL<MasterDB>(EDatabase.Master, server, password, "master_database");

#elif RELEASE
            string server = "gamepli-db.cfk4ckioaflf.ap-northeast-2.rds.amazonaws.com";
            string password = "GAMEpli412!";

            AddMySQL<GameDB>(EDatabase.Game, server, password, "game_database");
            AddMySQL<MasterDB>(EDatabase.Master, server, password, "master_database");
#else

            string server = "127.0.0.1";
            string password = "!Q2w3e4r";

            AddMySQL<GameDB>(EDatabase.Game, server, password, "game_database");
            AddMySQL<MasterDB>(EDatabase.Master, server, password, "master_database");
#endif

            context = new MasterDatabaseContext(databases[EDatabase.Master]);

            bool ret = context.IsValidContext();
            if(ret == false)
            {
                Console.WriteLine("마스터 데이터베이스 초기화 실패");
            }
        }

        public void RemoveMySQL()
        {

        }
    }
}
