using WebCommonLibrary.Models.GameDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using SqlKata.Execution;
using System.Data;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<IEnumerable<DB_ItemUnit>> LoadInventory(int storage_id)
        {
            var result = await mQueryFactory.Query("storage_unit")
            .Select
            (
                    "storage_unit.grid_x",
                    "storage_unit.grid_y",
                    "storage_unit.rotation",
                    "storage_unit.unit_attributes_id",

                    "unit_attributes.item_id",
                    "unit_attributes.durability",
                    "unit_attributes.unit_storage_id",
                    "unit_attributes.amount"
            )
            .LeftJoin("unit_attributes", "storage_unit.unit_attributes_id", "unit_attributes.unit_attributes_id")
            .Where("storage_unit.storage_id", storage_id)
            .GetAsync();

            var inventory = result.Select(row => new DB_ItemUnit
            {
                storage = new DB_StorageUnit
                {
                    grid_x = row.grid_x,
                    grid_y = row.grid_y,
                    rotation = row.rotation,
                    unit_attributes_id = row.unit_attributes_id,
                },

                attributes = new DB_UnitAttributes
                {
                    item_id = row.item_id,
                    durability = row.durability,
                    unit_storage_id = row.unit_storage_id as int?,
                    amount = row.amount
                }
            }).ToList();

            return inventory;
        }

        public async Task<int> DeleteInventoryItem(int storage_id, DB_ItemUnit unit, IDbTransaction? transaction = null)
        {
            var query = this.mQueryFactory;

            if(unit.storage == null)
            {
                return 0;
            }

            var values = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "grid_x"      , unit.storage.grid_x },
                { "grid_y"      , unit.storage.grid_y },
                { "rotation"    , unit.storage.rotation }
            };

            int result = await query.Query("storage_unit").
                Where(values).
                DeleteAsync(transaction);

            if (result == 0)
            {
                return 0;
            }

            return await query.Query("unit_attributes").
                Where("unit_attributes_id", unit.storage.unit_attributes_id).
                DeleteAsync(transaction);


        }

        public async Task<int> InsertInventoryItem(int storage_id, DB_ItemUnit unit, IDbTransaction? transaction = null)
        {
            var query = this.mQueryFactory;

            if (unit.storage == null || unit.attributes == null)
            {
                return 0;
            }

            int unit_attributes_id = unit.storage.unit_attributes_id;
            if (unit_attributes_id == 0)
            {
                unit_attributes_id = await query.Query("unit_attributes").
                InsertGetIdAsync<int>(new
                {
                    item_id = unit.attributes.item_id,
                    durability = unit.attributes.durability,
                    loaded_ammo = unit.attributes.loaded_ammo,
                    unit_storage_id = unit.attributes.unit_storage_id,
                    amount = unit.attributes.amount
                }, transaction);
            }
            unit.attributes.unit_storage_id = unit_attributes_id;

            return await query.Query("storage_unit").
                InsertAsync(new
                {
                    storage_id = storage_id,
                    grid_x = unit.storage.grid_x,
                    grid_y = unit.storage.grid_y,
                    rotation = unit.storage.rotation,
                    unit_attributes_id = unit_attributes_id,
                }, transaction);
        }
    }
}
