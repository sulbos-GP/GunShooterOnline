using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Models.GameDB;
using GsoWebServer.Servicies.Interfaces;

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
                var user = await mGameDB.GetUserByNickname(newNickname);
                if (user is not null)
                {
                    return (WebErrorCode.AuthTokenFailSetNx, string.Empty);
                }

                var rowCount = await mGameDB.UpdateNickname(uid, newNickname);
                if (rowCount != 1)
                {
                    return (WebErrorCode.AuthTokenFailSetNx, string.Empty);
                }

                return (WebErrorCode.None, newNickname);
            }
            catch /*(Exception e)*/
            {
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
                return (WebErrorCode.AuthTokenFailSetNx, null);
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

    }
}
