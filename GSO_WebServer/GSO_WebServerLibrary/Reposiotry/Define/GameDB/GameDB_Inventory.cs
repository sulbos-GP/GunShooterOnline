using GSO_WebServerLibrary.Models.GameDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<IEnumerable<DB_StorageUnit>> LoadStorageUnits(int storage_id)
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

            var inventory = result.Select(row => new DB_StorageUnit
            {
                grid_x = row.grid_x,
                grid_y = row.grid_y,
                rotation = row.rotation,
                unit_attributes_id = row.unit_attributes_id as int?,
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
    }
}
