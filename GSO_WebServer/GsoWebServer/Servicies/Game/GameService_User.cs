using GsoWebServer.Servicies.Interfaces;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDB;

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

        public async Task<(WebErrorCode, UserInfo?)> GetUserInfo(int uid)
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

        public async Task<(WebErrorCode, UserMetadataInfo?)> GetMetadataInfo(int uid)
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

        public async Task<WebErrorCode> UpdateUserMetadata(Int32 uid, UserMetadataInfo matadata)
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

        public async Task<(WebErrorCode, UserSkillInfo?)> GetSkillInfo(int uid)
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

        public async Task<(WebErrorCode, List<UserLevelReward>?)> GetUserLevelReward(int uid, bool? received, int? reward_level_id)
        {
            try
            {
                IEnumerable<UserLevelReward>? list = await mGameDB.GetUserLevelRewardByUid(uid, received, reward_level_id);
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

        public async Task<WebErrorCode> UpdateUserSkill(Int32 uid, UserSkillInfo skill)
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

                for(int level = oldLevel; level < curLevel; ++level)
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

                int reward_id = mMasterDB.GetRewardLevelList().Where(reward => reward.Value.level == level).FirstOrDefault().Key;
                if(reward_id == 0)
                {
                    throw new Exception($"{level}레벨에 대한 아이디가 존재하지 않습니다.");
                }

                DB_RewardBase reward = mMasterDB.GetRewardBaseList().Where(reward => reward.Key == reward_id).FirstOrDefault().Value;
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

    }
}
