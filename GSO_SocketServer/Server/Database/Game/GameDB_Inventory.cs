using Google.Protobuf.Protocol;
using Server.Database.Data;
using Server.Database.Interface;
using Server.Game;
using Server.Game.Object.Gear;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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
        public async Task<IEnumerable<DB_Unit>> LoadInventory(int storage_id)
        {
            var query = this.GetQueryFactory();

            var result = await query.Query("storage_unit")
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

            var inventory = result.Select(row => new DB_Unit
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

        public async Task<int> InsertItem(int storage_id, ItemObject item, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            DB_Unit unit = item.Unit;
            int unit_attributes_id = unit.storage.unit_attributes_id;
            if (unit_attributes_id == 0)
            {
                unit_attributes_id = await query.Query("unit_attributes").
                InsertGetIdAsync<int>(new
                {
                    item_id = unit.attributes.item_id,
                    durability = unit.attributes.durability,
                    unit_storage_id = unit.attributes.unit_storage_id,
                    amount = unit.attributes.amount
                }, transaction);
            }
            item.UnitAttributesId = unit_attributes_id;

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

        public async Task<int> InsertGetStorageId(IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            return await query.Query("storage").
                InsertGetIdAsync<int>(new
                {
                    storage_type = "backpack"
                }, transaction);
        }

        public async Task<int> DeleteStorage(int? storage_id, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            return await query.Query("storage").
                Where("storage_id", storage_id).
                DeleteAsync(transaction);
        }

        public async Task<int> DeleteItem(int storage_id, DB_Unit unit, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            return await query.Query("unit_attributes").
                Where("unit_attributes_id", unit.storage.unit_attributes_id).
                DeleteAsync(transaction);

            //var values = new Dictionary<string, object>()
            //{
            //    { "storage_id"  , storage_id },
            //    { "grid_x"      , unit.storage.grid_x },
            //    { "grid_y"      , unit.storage.grid_y },
            //    { "rotation"    , unit.storage.rotation }
            //};

            //return await query.Query("storage_unit").
            //    Where(values).
            //    DeleteAsync(transaction);
        }

        public async Task<int> UpdateItem(int storage_id, ItemObject oldItem, ItemObject newItem, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            DB_Unit oldUnit = oldItem.Unit;
            var oldValues = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "grid_x"      , oldUnit.storage.grid_x },
                { "grid_y"      , oldUnit.storage.grid_y },
                { "rotation"    , oldUnit.storage.rotation }
            };

            DB_Unit newUnit = newItem.Unit;
            var newValues = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "grid_x"      , newUnit.storage.grid_x },
                { "grid_y"      , newUnit.storage.grid_y },
                { "rotation"    , newUnit.storage.rotation }
            };

            return await query.Query("storage_unit").
                Where(oldValues).
                UpdateAsync(newValues, transaction);
        }

        public async Task<int> UpdateItemAttributes(ItemObject item,  IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            DB_Unit unit = item.Unit;
            var newValues = new Dictionary<string, object>()
            {
                { "item_id"     , unit.attributes.item_id },
                { "durability"  , unit.attributes.durability },
                { "unit_storage_id"  , unit.attributes.unit_storage_id },
                { "amount"      , unit.attributes.amount },
            };

            return await query.Query("unit_attributes").
                Where("unit_attributes_id", unit.storage.unit_attributes_id).
                UpdateAsync(newValues, transaction);
        }

    }
}
