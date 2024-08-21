using Server.Database.Interface;
using Server.Game;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Database.Game
{
    ///////////////////////////////////////////////
    ///                                         ///
    ///               INVENTORY                 ///
    ///                                         ///
    ///////////////////////////////////////////////

    public partial class GameDB : MySQL, IGameDatabase
    {

        public async Task<int> GetStorageItemId(int storage_id)
        {
            var query = this.GetQueryFactory();

            return await query.Query("storage")
                .Select("storage_item_id")
                .Where("storage_id", storage_id)
                .FirstOrDefaultAsync<int>();
        }

        public async Task<IEnumerable<Gear>> LoadGear(int uid)
        {
            var query = this.GetQueryFactory();

            return await query.Query("gear")
                .Where("uid", uid)
                .GetAsync<Gear>();
        }

        public async Task<IEnumerable<InventoryUnit>> LoadInventory(int storage_id)
        {
            var query = this.GetQueryFactory();
            
            return await query.Query("storage_unit")
                .Where("storage_id", storage_id)
                .GetAsync<InventoryUnit>();
        }

        public async Task<int> InsertItem(int storage_id, InventoryUnit unit)
        {
            var query = this.GetQueryFactory();

            return await query.Query("storage_unit").
                InsertAsync(new
                {
                    storage_id = storage_id,
                    item_id = unit.item_id,
                    grid_x = unit.grid_x,
                    grid_y = unit.grid_y,
                    rotation= unit.rotation,
                    stack_count = unit.stack_count
                }, null);
        }

        public async Task<int> DeleteItem(int storage_id, InventoryUnit unit)
        {
            var query = this.GetQueryFactory();

            var values = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "item_id"     , unit.item_id },
                { "grid_x"      , unit.grid_x },
                { "grid_y"      , unit.grid_y },
                { "rotation"    , unit.rotation },
                { "stack_count" , unit.stack_count },
            };

            return await query.Query("storage_unit").
                Where(values).
                DeleteAsync();
        }

        public async Task<bool> MoveItem()
        {
            var query = this.GetQueryFactory();

        }
    }
}
