using Server.Game;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace Server.Database.Game
{
    public partial class GameDB : MySQL
    {
        public async Task<IEnumerable<FUserRegisterQuest>> LoadRegisterQuest(int uid, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            return await query.Query("user_register_quest")
                .Where("uid", uid)
                .GetAsync<FUserRegisterQuest>();
        }

        public async Task<int> UpdateRegisterQuest(int uid, FUserRegisterQuest quest, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();


            var where = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "quest_id" , quest.quest_id },
            };

            var values = new Dictionary<string, object>()
            {
                { "progress" , quest.progress },
                { "completed", quest.completed },
            };

            return await query.Query("user_register_quest")
            .Where(where)
            .UpdateAsync(values, transaction);
            
        }
    }
}
