using CloudStructures;
using Microsoft.Extensions.Options;
using GSO_WebServerLibrary.Config;
using GSO_WebServerLibrary.Reposiotry.Interfaces;

namespace GSO_WebServerLibrary.Reposiotry.Define.MemoryDB
{
    public partial class MemoryDB : IMemoryDB
    {
        public RedisConnection mRedisConn;
        public MemoryDB(IOptions<DatabaseConfig> dbConfig)
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
