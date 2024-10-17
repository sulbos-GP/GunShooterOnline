using GsoWebServer.Servicies.Interfaces;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Error;

namespace GsoWebServer.Servicies.DataLoad
{
    public class DataLoadService : IDataLoadService
    {
        readonly IGameService mGameService;

        public DataLoadService(IGameService gameService)
        {
            mGameService = gameService;
        }

        public void Dispose()
        {

        }

        public async Task<(WebErrorCode, DataLoadUserInfo?)> LoadUserData(int uid)
        {
            DataLoadUserInfo loadData = new DataLoadUserInfo();

            (var errorCode, loadData.UserInfo) = await mGameService.GetUserInfo(uid);
            if (errorCode != WebErrorCode.None)
            {
                return (errorCode, null);
            }

            (errorCode, loadData.MetadataInfo) = await mGameService.GetMetadataInfo(uid);
            if (errorCode != WebErrorCode.None)
            {
                return (errorCode, null);
            }

            (errorCode, loadData.SkillInfo) = await mGameService.GetSkillInfo(uid);
            if (errorCode != WebErrorCode.None)
            {
                return (errorCode, null);
            }

            (errorCode, loadData.LevelReward) = await mGameService.GetUserLevelReward(uid, null, null);
            if (errorCode != WebErrorCode.None)
            {
                return (errorCode, null);
            }

            return (WebErrorCode.None, loadData);
        }

        public async Task<(WebErrorCode, DailyLoadInfo?)> DailyLoadData(int uid)
        {
            DailyLoadInfo loadData = new DailyLoadInfo();

            (var errorCode, loadData.DailyQuset) = await mGameService.GetDailyQuest(uid);
            if (errorCode != WebErrorCode.None)
            {
                return (errorCode, null);
            }

            return (WebErrorCode.None, loadData);
        }

    }
}
