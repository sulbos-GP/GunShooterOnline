using Server.Database.Interface;
using Server.Game;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

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

        public async Task<IEnumerable<DB_InventoryUnit>> LoadInventory(int storage_id)
        {
            var query = this.GetQueryFactory();

            return await query.Query("storage_unit")
                .Where("storage_id", storage_id)
                .GetAsync<DB_InventoryUnit>();
        }

        public async Task<int> InsertItem(int storage_id, DB_InventoryUnit unit)
        {
            var query = this.GetQueryFactory();

            return await query.Query("storage_unit").
                InsertAsync(new
                {
                    storage_id = storage_id,
                    item_id = unit.item_id,
                    grid_x = unit.grid_x,
                    grid_y = unit.grid_y,
                    rotation = unit.rotation,
                    stack_count = unit.stack_count
                }, null);
        }

        public async Task<int> DeleteItem(int storage_id, DB_InventoryUnit unit)
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

        public async Task<int> UpdateItem(int storage_id, DB_InventoryUnit oldUnit, DB_InventoryUnit newUnit)
        {
            var query = this.GetQueryFactory();

            var oldValues = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "item_id"     , oldUnit.item_id },
                { "grid_x"      , oldUnit.grid_x },
                { "grid_y"      , oldUnit.grid_y },
                { "rotation"    , oldUnit.rotation },
                { "stack_count" , oldUnit.stack_count },
            };

            var newValues = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "item_id"     , newUnit.item_id },
                { "grid_x"      , newUnit.grid_x },
                { "grid_y"      , newUnit.grid_y },
                { "rotation"    , newUnit.rotation },
                { "stack_count" , newUnit.stack_count },
            };

            return await query.Query("storage_unit").
                Where(oldValues).
                UpdateAsync(newValues);
        }

        public async Task<bool> MergeItem(int storage_id, DB_InventoryUnit oldMergeUnit, DB_InventoryUnit newMergeUnit, bool isDelete, DB_InventoryUnit oldCombinedUnit, DB_InventoryUnit newCombinedUnit)
        {
            var query = this.GetQueryFactory();

            using (var transaction = this.GetConnection().BeginTransaction())
            {
                try
                {
                    if(isDelete)
                    {
                        await DeleteItem(storage_id, oldCombinedUnit);
                    }
                    else
                    {
                        await UpdateItem(storage_id, oldCombinedUnit, newCombinedUnit);
                    }

                    await UpdateItem(storage_id, oldMergeUnit, newMergeUnit);

                    transaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine($"[MergeItem] : {e.Message.ToString()}");
                    return false;
                }
               
            }

        }

        public async Task<bool> DevideItem(int storage_id, DB_InventoryUnit oldTotalUnit, DB_InventoryUnit newTotalUnit, DB_InventoryUnit newUnit)
        {
            var query = this.GetQueryFactory();

            using (var transaction = this.GetConnection().BeginTransaction())
            {
                try
                {
                    await InsertItem(storage_id, newUnit);
                    await UpdateItem(storage_id, oldTotalUnit, newTotalUnit);

                    transaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine($"[DevideItem] : {e.Message.ToString()}");
                    return false;
                }

            }

        }
    }
}
