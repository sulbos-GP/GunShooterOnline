using Google.Apis.Games.v1;
using GsoWebServer.Servicies.Interfaces;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;
using static Google.Apis.Requests.BatchRequest;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<bool> ClearInventory(int storage_id)
        {
            try
            {
                var (error, items) = await LoadInventory(storage_id);
                if (error != WebErrorCode.None || items == null)
                {
                    return false;
                }

                foreach (var item in items)
                {
                    int isDelete = await mGameDB.DeleteInventoryItem(storage_id, item);
                    if(isDelete == 0)
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

        public async Task<WebErrorCode> InsertInventory(int storage_id, DB_ItemUnit unit)
        {
            try
            {
                return (await mGameDB.InsertInventoryItem(storage_id, unit) == 0 ? WebErrorCode.TEMP_ERROR : WebErrorCode.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InsertInventory] Error : {ex.ToString()}");
                return (WebErrorCode.TEMP_Exception);
            }
        }

    }
}
