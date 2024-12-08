using GsoWebServer.Servicies.Interfaces;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

namespace GsoWebServer.Servicies.Game
{
    public partial class GameService : IGameService
    {
        public async Task<(WebErrorCode, IEnumerable<DB_GearUnit>?)> LoadGear(int uid)
        {
            try
            {
                return (WebErrorCode.None, await mGameDB.LoadGear(uid));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadInventory] Error : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

        public async Task<bool> ClearGear(int uid)
        {
            try
            {
                var (error, gears) = await LoadGear(uid);
                if (error != WebErrorCode.None || gears == null)
                {
                    return false;
                }

                foreach (var gear in gears)
                {

                    if (gear == null)
                    {
                        continue;
                    }

                    int isDelete = await mGameDB.DeleteGearItem(uid, gear, gear.gear.part);
                    if (isDelete == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ClearInventory] Error : {ex.ToString()}");
                return false;
            }
        }

        public async Task<WebErrorCode> InsertGear(int uid, DB_GearUnit unit)
        {
            try
            {
                return (await mGameDB.InsertGearItem(uid, unit, unit.gear.part) == 0 ? WebErrorCode.TEMP_ERROR : WebErrorCode.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadInventory] Error : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception);
            }
        }

        public async Task<int> InsertGearBackpackItem(int uid, DB_GearUnit unit)
        {
            try
            {
                return (await mGameDB.InsertGearBackpackItem(uid, unit));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadInventory] Error : {ex.ToString()}");
                return (0);
            }
        }

    }
}
