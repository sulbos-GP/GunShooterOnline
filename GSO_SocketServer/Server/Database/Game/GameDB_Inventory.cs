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
        public async Task<IEnumerable<DB_StorageUnit>> LoadInventory(int storage_id)
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

            var inventory = result.Select(row => new DB_StorageUnit
            {
                grid_x = row.grid_x,
                grid_y = row.grid_y,
                rotation = row.rotation,
                unit_attributes_id = row.unit_attributes_id,
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

        public async Task<int> InsertItem(int storage_id, DB_StorageUnit unit, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            int unit_attributes_id = await query.Query("unit_attributes").
                InsertGetIdAsync<int>(new
                {
                    item_id = unit.attributes.item_id,
                    durability = unit.attributes.durability,
                    unit_storage_id = unit.attributes.unit_storage_id,
                    amount = unit.attributes.amount
                }, transaction);

            return await query.Query("storage_unit").
                InsertAsync(new
                {
                    storage_id = storage_id,
                    grid_x = unit.grid_x,
                    grid_y = unit.grid_y,
                    rotation = unit.rotation,
                    unit_attributes_id = unit_attributes_id,
                }, transaction);
        }

        public async Task<int> DeleteItem(int storage_id, DB_StorageUnit unit, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            int result = await query.Query("unit_attributes").
                Where("unit_attributes_id", unit.unit_attributes_id).
                DeleteAsync(transaction);

            var values = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "grid_x"      , unit.grid_x },
                { "grid_y"      , unit.grid_y },
                { "rotation"    , unit.rotation }
            };

            return await query.Query("storage_unit").
                Where(values).
                DeleteAsync(transaction);
        }

        public async Task<int> UpdateItem(int storage_id, DB_StorageUnit oldUnit, DB_StorageUnit curUnit, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            var oldValues = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "grid_x"      , oldUnit.grid_x },
                { "grid_y"      , oldUnit.grid_y },
                { "rotation"    , oldUnit.rotation }
            };

            var newValues = new Dictionary<string, object>()
            {
                { "storage_id"  , storage_id },
                { "grid_x"      , curUnit.grid_x },
                { "grid_y"      , curUnit.grid_y },
                { "rotation"    , curUnit.rotation }
            };

            return await query.Query("storage_unit").
                Where(oldValues).
                UpdateAsync(newValues, transaction);
        }

        public async Task<int> UpdateItemAttributes(int unit_attributes_id, DB_UnitAttributes attributes,  IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            var newValues = new Dictionary<string, object>()
            {
                { "item_id"     , attributes.item_id },
                { "durability"  , attributes.durability },
                { "storage_id"  , attributes.unit_storage_id },
                { "amount"      , attributes.amount },
            };

            return await query.Query("unit_attributes").
                Where("unit_attributes_id", unit_attributes_id).
                UpdateAsync(newValues, transaction);
        }

    }
}
