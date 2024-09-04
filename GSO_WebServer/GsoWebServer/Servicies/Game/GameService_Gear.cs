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

    }
}
