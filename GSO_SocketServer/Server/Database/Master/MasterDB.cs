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

        public async Task<ItemInfo> GetItemInfo(int item_id)
        {
            var query = this.GetQueryFactory();

            return await query.Query("master_item_base")
                .Where("item_id", item_id)
                .FirstOrDefaultAsync<ItemInfo>();
        }
    }
}
