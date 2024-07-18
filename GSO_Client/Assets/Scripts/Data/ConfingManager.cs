using System;
using System.IO;

namespace Server.Data
{
    [Serializable]
    public class ServerConfig
    {
        public string dataPath;
        public string connectionString;
    }

    internal class ConfingManager
    {
        public static ServerConfig config { get; private set; }

        public static void LoadConfig()
        {
            var text = File.ReadAllText("config.json"); //debug파일에 있음
            //config = JsonConvert.DeserializeObject<ServerConfig>(text);
        }
    }
}