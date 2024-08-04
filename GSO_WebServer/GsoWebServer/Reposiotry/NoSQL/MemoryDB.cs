using CloudStructures;
using Microsoft.Extensions.Options;
using GsoWebServer.Reposiotry.Interfaces;
using GsoWebServer.Models.Config;

namespace GsoWebServer.Reposiotry.NoSQL
{
    public partial class MemoryDB : IMemoryDB
    {
        public RedisConnection mRedisConn;
        public MemoryDB(IOptions<DbConfig> dbConfig)
        {
            RedisConfig config = new("default", dbConfig.Value.Redis);
            mRedisConn = new RedisConnection(config);
        }

        public void Dispose()
        {
            if (mRedisConn != null)
            {
                mRedisConn.GetConnection().Close();
            }
        }
    }
}
