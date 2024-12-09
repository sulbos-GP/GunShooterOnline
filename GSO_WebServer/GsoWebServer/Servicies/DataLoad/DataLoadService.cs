using GsoWebServer.Servicies.Interfaces;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

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

            (errorCode, loadData.gears) = await mGameService.LoadGear(uid);
            if (errorCode != WebErrorCode.None)
            {
                return (errorCode, null);
            }

            if(loadData.gears != null)
            {
                var backpack = loadData.gears.FirstOrDefault(gear => gear.gear.part == "backpack");
                if(backpack != null && backpack.attributes.unit_storage_id != null)
                {
                    (errorCode, loadData.items) = await mGameService.LoadInventory(backpack.attributes.unit_storage_id.Value);
                    if (errorCode != WebErrorCode.None)
                    {
                        return (errorCode, null);
                    }
                }
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
