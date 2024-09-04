using GsoWebServer.Servicies.Interfaces;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

namespace GsoWebServer.Servicies.Game
{
    public partial class GameService : IGameService
    {
        public async Task<(WebErrorCode, IEnumerable<DB_ItemUnit>?)> LoadInventory(int storage_id)
        {
            try
            {
                return (WebErrorCode.None, await mGameDB.LoadInventory(storage_id));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadInventory] Error : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception, null);
            }
        }

    }
}
