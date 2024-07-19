using Google.Apis.Games.v1.Data;
using GSO_WebServerLibrary;
using GsoWebServer.Models.GameDB;
using GsoWebServer.Servicies.Interfaces;
using static Google.Apis.Requests.RequestError;

namespace GsoWebServer.Servicies.Game
{
    public partial class GameService : IGameService
    {


        public async Task<(WebErrorCode, int)> SingUpWithNewUserGameData(String userId, String service)
        {
            var transaction = mGameDB.GetConnection().BeginTransaction();
            try
            {
                var uid = await mGameDB.SingUp(userId, service, transaction);
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

        public async Task<(WebErrorCode, MetadataInfo?)> GetMetadataInfo(int uid)
        {
            try
            {
                return (WebErrorCode.None, await mGameDB.GetUserMetaDataByUid(uid));
            }
            catch /*(Exception e)*/
            {
                return (WebErrorCode.AuthTokenFailSetNx, null);
            }
        }

        public async Task<(WebErrorCode, SkillInfo?)> GetSkillInfo(int uid)
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

    }
}
