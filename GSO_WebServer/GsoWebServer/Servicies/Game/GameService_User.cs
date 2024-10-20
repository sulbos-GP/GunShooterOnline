using Google.Apis.Games.v1.Data;
using GsoWebServer.Servicies.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Transactions;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;

namespace GsoWebServer.Servicies.Game
{
    public partial class GameService : IGameService
    {


        public async Task<(WebErrorCode, int)> SingUpWithNewUserGameData(String userId, String service, String refresh_token)
        {
            var transaction = mGameDB.GetConnection().BeginTransaction();
            try
            {
                var uid = await mGameDB.SingUp(userId, service, refresh_token, transaction);
                if(uid == 0)
                {
                    return (WebErrorCode.AuthTokenFailSetNx, 0);
                }

                var rowCount = await mGameDB.InitUserMatadata(uid, transaction);
                if (rowCount != 1)
                {
                    transaction.Rollback();
                    return (WebErrorCode.AuthTokenFailSetNx, 0);
                }

                rowCount = await mGameDB.InitUserSkill(uid, transaction);
                if (rowCount != 1)
                {
                    transaction.Rollback();
                    return (WebErrorCode.AuthTokenFailSetNx, 0);
                }

                transaction.Commit();
                return (WebErrorCode.None, uid);
            }
            catch /*(Exception e)*/
            {
                transaction.Rollback();
                return (WebErrorCode.CheckAuthFailException, 0);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public async Task<(WebErrorCode, string)> UpdateNickname(Int32 uid, String newNickname)
        {
            try
            {
                // 닉네임이 존재할 경우 리턴
                var user = await mGameDB.GetUserByNickname(newNickname);
                if (user != null)
                {
                    return (WebErrorCode.AuthTokenFailSetNx, string.Empty);
                }

                //닉네임을 변경 못했을 경우 리턴
                var result = await mGameDB.UpdateNickname(uid, newNickname);
                if (result == 0)
                {
                    return (WebErrorCode.AuthTokenFailSetNx, string.Empty);
                }

                return (WebErrorCode.None, newNickname);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[UpdateNickname] : {e.Message}");
                return (WebErrorCode.CheckAuthFailException, string.Empty);
            }
        }

        public async Task<(WebErrorCode, FUser?)> GetUserInfo(int uid)
        {
            try
            {
                return (WebErrorCode.None, await mGameDB.GetUserByUid(uid));
            }
            catch /*(Exception e)*/
            {
                return (WebErrorCode.AuthTokenFailSetNx, null);
            }
        }

        public async Task<(WebErrorCode, FUserMetadata?)> GetMetadataInfo(int uid)
        {
            try
            {
                return (WebErrorCode.None, await mGameDB.GetUserMetadataByUid(uid));
            }
            catch /*(Exception e)*/
            {
                return (WebErrorCode.AuthTokenFailSetNx, null);
            }
        }

        public async Task<WebErrorCode> UpdateUserMetadata(Int32 uid, FUserMetadata matadata)
        {
            try
            {
                var rowCount = await mGameDB.UpdateUserMetadata(uid, matadata);
                if (rowCount != 1)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }

                return (WebErrorCode.None);
            }
            catch /*(Exception e)*/
            {
                return (WebErrorCode.TEMP_Exception);
            }
        }

        public async Task<(WebErrorCode, FUserSkill?)> GetSkillInfo(int uid)
        {
            try
            {
                return (WebErrorCode.None, await mGameDB.GetUserSkillByUid(uid));
            }
            catch /*(Exception e)*/
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<(WebErrorCode, List<FUserRegisterQuest>?)> GetDailyQuest(int uid)
        {
            try
            {
                return (WebErrorCode.None, await mGameDB.GetUserDailyQuestByUid(uid));
            }
            catch /*(Exception e)*/
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<(WebErrorCode, List<FUserLevelReward>?)> GetUserLevelReward(int uid, bool? received, int? reward_level_id)
        {
            try
            {
                IEnumerable<FUserLevelReward>? list = await mGameDB.GetUserLevelRewardByUid(uid, received, reward_level_id);
                if (list == null)
                {
                    return (WebErrorCode.TEMP_ERROR, null);
                }

                return (WebErrorCode.None, list.ToList());
            }
            catch /*(Exception e)*/
            {
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<WebErrorCode> UpdateUserSkill(Int32 uid, FUserSkill skill)
        {
            try
            {
                var rowCount = await mGameDB.UpdateUserSkill(uid, skill);
                if (rowCount != 1)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }

                return (WebErrorCode.None);
            }
            catch /*(Exception e)*/
            {
                return (WebErrorCode.TEMP_Exception);
            }
        }

        public async Task<WebErrorCode> UpdateLevel(Int32 uid, Int32 experience)
        {
            var transaction = mGameDB.GetConnection().BeginTransaction();
            try
            {
                var user =  await mGameDB.GetUserByUid(uid, transaction);
                if (user == null)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }

                int oldLevel = (user.experience < 100) ? 1 : (user.experience / 100);
                int curLevel = (user.experience < 100) ? 1 : (user.experience + experience) / 100;

                if(0 == await mGameDB.UpdateLevel(uid, user.experience + experience, transaction))
                {
                    throw new Exception("레벨이 업데이트 되지 못했습니다.");
                }

                for(int level = oldLevel + 1; level <= curLevel; ++level)
                {
                    if(0 == await mGameDB.InsertLevelReward(uid, level, transaction))
                    {
                        throw new Exception("레벨업은 하였지만 보상을 추가하지는 못했습니다.");
                    }
                }

                transaction.Commit();
                return (WebErrorCode.None);
            }
            catch /*(Exception e)*/
            {
                transaction.Rollback();
                return (WebErrorCode.TEMP_Exception);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        /// <summary>
        /// 유저 레벨 보상 받기
        /// </summary>
        public async Task<WebErrorCode> RecvLevelReward(Int32 uid, Int32 level)
        {
            var transaction = mGameDB.GetConnection().BeginTransaction();
            try
            {

                var user = await mGameDB.GetUserByUid(uid, transaction);
                if(user == null)
                {
                    throw new Exception("유저가 존재하지 않습니다.");
                }

                if (0 == await mGameDB.RecivedLevelReward(uid, level, true, transaction))
                {
                    throw new Exception("레벨이 보상을 받지 못했습니다.");
                }

                int reward_id = mMasterDB.Context.MasterRewardLevel.Where(reward => reward.Value.level == level).FirstOrDefault().Key;
                if(reward_id == 0)
                {
                    throw new Exception($"{level}레벨에 대한 아이디가 존재하지 않습니다.");
                }

                FMasterRewardBase reward = mMasterDB.Context.MasterRewardBase.Where(reward => reward.Key == reward_id).FirstOrDefault().Value;
                if (reward == null)
                {
                    throw new Exception($"{level}레벨에 대한 보상이 존재하지 않습니다.");
                }

                //경험치 보상이 포함되어 있을 경우
                if (reward.experience != 0)
                {
                    user.experience += reward.experience;
                    if(0 == await mGameDB.UpdateLevel(uid, user.experience, transaction))
                    {
                        throw new Exception("보상에 있는 경험치를 업데이트 할 수 없습니다.");
                    }
                }

                //통화에 대한 보상이 있을 경우
                if(reward.money != 0 || user.ticket != 0 || user.gacha != 0)
                {
                    user.money += reward.money;
                    user.ticket += reward.ticket;
                    user.gacha += reward.gacha;

                    if (0 == await mGameDB.UpdateCurrency(uid, user.money, user.ticket, user.gacha, transaction))
                    {
                        throw new Exception("보상에 있는 경험치를 업데이트 할 수 없습니다.");
                    }
                }

                //박스로된 보상이 포함되어 있을 경우
                if(reward.reward_box_id != null)
                {

                }

                transaction.Commit();
                return (WebErrorCode.None);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[UpdateLevelReward] : {e.Message}");
                transaction.Rollback();
                return (WebErrorCode.TEMP_Exception);
            }
            finally
            {
                transaction.Dispose(); 
            }
        }

        public async Task<WebErrorCode> UpdateTicketCount(Int32 uid)
        {
            try
            {
                var user = await mGameDB.GetUserByUid(uid);
                if (user == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                if(user.ticket < GameDefine.MAX_TICKET)
                {
                    DateTime now = DateTime.UtcNow;
                    DateTime recent = user.recent_ticket_dt;

                    TimeSpan timeDiff = now - recent;
                    int timeTicket = (int)timeDiff.TotalMinutes / GameDefine.WAIT_TICKET_MINUTE;
                    if (timeTicket == 0)
                    {
                        return WebErrorCode.TicketRemainingTime;
                    }

                    int possibleTicketCount = timeTicket + user.ticket;
                    possibleTicketCount = Math.Clamp(possibleTicketCount, 0, GameDefine.MAX_TICKET);

                    int updateRes = await mGameDB.UpdateTicket(uid, possibleTicketCount);
                    if (updateRes == 0)
                    {
                        return WebErrorCode.TEMP_ERROR;
                    }

                    updateRes = await mGameDB.UpdateLastTicketTime(uid);
                    if (updateRes == 0)
                    {
                        return WebErrorCode.TEMP_ERROR;
                    }
                }

                return WebErrorCode.None;
            }
            catch
            {
                return WebErrorCode.TEMP_Exception;
            }
        }

        public async Task<WebErrorCode> UpdateLastTicketTime(int uid)
        {
            var rowCount = await mGameDB.UpdateLastTicketTime(uid);
            if (rowCount != 1)
            {
                return WebErrorCode.AccountIdMismatch;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> UpdateDailyTask(int uid)
        {
            FUser? user = await mGameDB.GetUserByUid(uid);
            if (user == null)
            {
                return (WebErrorCode.TEMP_ERROR);
            }

            DateTime currentTime = DateTime.UtcNow;
            DateTime recentLoginTime = user.recent_login_dt.AddDays(1);

            if (currentTime < recentLoginTime)
            {
                return WebErrorCode.DailyTaskIsAllocate;
            }

            var error = await UpdateDailyQuset(uid);
            if(error != WebErrorCode.None)
            {
                return error;
            }

            return WebErrorCode.None;
        }

        public async Task<WebErrorCode> UpdateDailyQuset(int uid)
        {
            var transaction = mGameDB.GetConnection().BeginTransaction();
            try
            {
                var oldDailyQuestList = await mGameDB.GetUserDailyQuestByUid(uid, transaction);
                if(oldDailyQuestList == null)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }

                var deleteCount = await mGameDB.DeleteUserDailyQuestByUid(uid, transaction);
                if(oldDailyQuestList.Count != deleteCount)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }

                var masterDailyQusetList = mMasterDB.Context.MasterQuestBase
                    .Select(quest => quest.Value)
                    .Where(quest => quest.type == "day")
                    .ToList();

                List<string> categorys = new List<string>
                {
                    "전투",
                    "보급",
                    "플레이"
                };

                foreach(var category in categorys)
                {
                    int quset_id = GetRandomDailyQuestWithCategory(masterDailyQusetList, category);
                    if (quset_id == -1)
                    {
                        return (WebErrorCode.TEMP_ERROR);
                    }

                    int result = await mGameDB.InsertUserDailyQuestByUid(uid, quset_id, transaction);
                    if(result == 0)
                    {
                        return (WebErrorCode.TEMP_ERROR);
                    }
                }

                transaction.Commit();
                return (WebErrorCode.None);
            }
            catch /*(Exception e)*/
            {
                transaction.Rollback();
                return (WebErrorCode.TEMP_Exception);
            }
        }

        public int GetRandomDailyQuestWithCategory(List<FMasterQuestBase> dailyQuestList, string category)
        {
            var catecoryList = dailyQuestList.Where(quest => quest.category == category).ToList();
            if (catecoryList.Count == 0)
            {
                return -1;
            }

            Random random = new Random();
            int index = random.Next(catecoryList.Count);
            return catecoryList[index].quest_id;
        }

        public async Task<WebErrorCode> CompleteDailyQuset(int uid, int quest_id)
        {
            var transaction = mGameDB.GetConnection().BeginTransaction();
            try
            {
                var oldDailyQuestList = await mGameDB.GetUserDailyQuestByUid(uid, transaction);
                if (oldDailyQuestList == null)
                {
                    return (WebErrorCode.DailyQuestInvalidList);
                }

                var completeQuest = oldDailyQuestList.FirstOrDefault(quest => quest.quest_id == quest_id);
                if(completeQuest == null)
                {
                    return (WebErrorCode.DailyQuestNotMatch);
                }

                var completeQuestData = mMasterDB.Context.MasterQuestBase.FirstOrDefault(quest => quest.Value.quest_id == quest_id).Value;
                if (completeQuestData == null)
                {
                    return (WebErrorCode.DailyQuestNotMatch);
                }

                if (completeQuest.progress != completeQuestData.target)
                {
                    return (WebErrorCode.DailyQuestNotEnough);
                }

                if (completeQuest.completed == true)
                {
                    return (WebErrorCode.DailyQuestAlreadyComplelte);
                }
                completeQuest.completed = true;

                int result = await mGameDB.UpdateUserDailyQuestByUid(uid, completeQuest, transaction);
                if (result == 0)
                {
                    return (WebErrorCode.TEMP_ERROR);
                }

                var error = await ReceiveReward(uid, completeQuestData.reward_id, transaction);
                if (error != WebErrorCode.None)
                {
                    return error;
                }

                transaction.Commit();
                return (WebErrorCode.None);
            }
            catch /*(Exception e)*/
            {
                transaction.Rollback();
                return (WebErrorCode.TEMP_Exception);
            }
        }

        public async Task<WebErrorCode> ReceiveReward(int uid, int reward_id, IDbTransaction transaction)
        {
            var user = await mGameDB.GetUserByUid(uid, transaction);
            if (user == null)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            FMasterRewardBase reward = mMasterDB.Context.MasterRewardBase.Where(reward => reward.Key == reward_id).FirstOrDefault().Value;
            if (reward == null)
            {
                return WebErrorCode.TEMP_ERROR;
            }

            //경험치 보상이 포함되어 있을 경우
            if (reward.experience != 0)
            {
                user.experience += reward.experience;
                if (0 == await mGameDB.UpdateLevel(uid, user.experience, transaction))
                {
                    return WebErrorCode.TEMP_ERROR;
                }
            }

            //통화에 대한 보상이 있을 경우
            if (reward.money != 0 || user.ticket != 0 || user.gacha != 0)
            {
                user.money += reward.money;
                user.ticket += reward.ticket;
                user.gacha += reward.gacha;

                if (0 == await mGameDB.UpdateCurrency(uid, user.money, user.ticket, user.gacha, transaction))
                {
                    return WebErrorCode.TEMP_ERROR;
                }
            }

            //박스로된 보상이 포함되어 있을 경우
            if (reward.reward_box_id != null)
            {

            }

            return WebErrorCode.None;
        }

    }
}
