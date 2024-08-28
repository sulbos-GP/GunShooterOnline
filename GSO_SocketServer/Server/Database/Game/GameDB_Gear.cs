using Server.Database.Data;
using Server.Database.Interface;
using Server.Game.Object.Gear;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Game
{
    public partial class GameDB : MySQL, IGameDatabase
    {
        public async Task<IEnumerable<DB_Gear>> LoadGear(int uid)
        {
            var query = this.GetQueryFactory();

            var result = await query.Query("gear")
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

            var gears = result.Select(row => new DB_Gear
            {
                part = row.part as string,
                unit_attributes_id = row.unit_attributes_id,
                attributes = new DB_UnitAttributes
                {
                    item_id         = row.item_id,
                    durability      = row.durability,
                    unit_storage_id = row.unit_storage_id as int?,
                    amount          = row.amount
                }
            }).ToList();

            return gears;
        }

        public async Task<int> GetGearOfPart(int uid, EGearPart part)
        {
            var query = this.GetQueryFactory();

            var values = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "part" , part.GetType().GetField(part.ToString()) },
            };

            return await query.Query("gear")
                .Select("item_id")
                .Where(values)
                .FirstOrDefaultAsync<int>();
        }
    }
}
