using WebCommonLibrary.Models.GameDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using SqlKata.Execution;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {
        public async Task<IEnumerable<DB_GearUnit>> LoadGear(int uid)
        {
            var result = await mQueryFactory.Query("gear")
                .Select
                (
                        "gear.part",
                        "gear.unit_attributes_id",

                        "unit_attributes.item_id",
                        "unit_attributes.durability",
                        "unit_attributes.unit_storage_id",
                        "unit_attributes.amount"
                )
                .LeftJoin("unit_attributes", "gear.unit_attributes_id", "unit_attributes.unit_attributes_id")
                .Where("gear.uid", uid)
                .GetAsync();

            var gears = result.Select(row => new DB_GearUnit
            {
                gear = new DB_Gear
                {
                    part = row.part,
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

            return gears;
        }

    }
}
