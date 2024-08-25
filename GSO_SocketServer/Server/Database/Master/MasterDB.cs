using Server.Database.Data;
using Server.Database.Interface;
using Server.Game;
using SqlKata.Execution;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Master
{
    public partial class MasterDB : MySQL, IMasterDatabase
    {

        public MasterDB() : base()
        {
        }

        public async Task<IEnumerable<T>> LoadTable<T>(string table)
        {
            var query = this.GetQueryFactory();

            return await query.Query(table)
                .GetAsync<T>();
        }

        public async Task<DB_BackpackData> GetBackpackData(string code)
        {
            var query = this.GetQueryFactory();

            return await query.Query("master_item_backpack")
                .Where("code", code)
                .FirstOrDefaultAsync<DB_BackpackData>();
        }

    }
}
