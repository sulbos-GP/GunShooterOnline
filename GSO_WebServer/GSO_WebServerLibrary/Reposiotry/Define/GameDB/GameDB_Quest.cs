using GSO_WebServerLibrary;
using WebCommonLibrary.Models.GameDB;
using SqlKata.Execution;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using System.Data;
using WebCommonLibrary.Models.GameDatabase;
using SqlKata;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<List<FUserRegisterQuest>?> GetUserDailyQuestByUid(Int32 uid, IDbTransaction? transaction)
        {
            var result = await mQueryFactory.Query("user_register_quest")
                .Where("uid", uid)
                .GetAsync<FUserRegisterQuest>(transaction);

            var registerQuestList = result.ToList();

            var dailyQuestList = registerQuestList
                .Where(quest => mMasterDB.Context.MasterQuestBase.Find(quest.quest_id).type == "day")
                .ToList();

            return dailyQuestList;
        }

        public async Task<int> DeleteUserDailyQuestByUid(int uid, IDbTransaction? transaction = null)
        {
            var dailyQuest = await GetUserDailyQuestByUid(uid, transaction);
            if (dailyQuest == null)
            {
                return 0;
            }

            int deleteCount = 0;
            foreach(var quest in dailyQuest)
            {
                var where = new Dictionary<string, object>()
                {
                    { "uid"  , uid },
                    { "quest_id"  , quest.quest_id },
                };

                var result = await mQueryFactory.Query("user_register_quest")
                    .Where(where)
                    .DeleteAsync(transaction);

                deleteCount += result;
            }
 
            return deleteCount;
        }

        public async Task<int> InsertUserDailyQuestByUid(int uid, int quest_id, IDbTransaction? transaction = null)
        {
            return await mQueryFactory.Query("user_register_quest").
                InsertAsync(new
                {
                    uid = uid,
                    quest_id = quest_id,
                    completed = false,
                    register_dt = DateTime.UtcNow,
                }, transaction);
        }

    }
}