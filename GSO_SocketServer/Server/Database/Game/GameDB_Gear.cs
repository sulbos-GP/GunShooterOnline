using Server.Database.Data;
using Server.Database.Interface;
using Server.Game;
using Server.Game.Object.Gear;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<int> InsertGear(int uid, ItemObject item, EGearPart part, IDbTransaction transaction = null)
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

            return await query.Query("gear").
                InsertAsync(new
                {
                    uid = uid,
                    part = part.GetType().GetField(part.ToString()),
                    unit_attributes_id = unit_attributes_id,
                }, transaction);
        }

        public async Task<int> DeleteGear(int uid, ItemObject item, EGearPart part, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            DB_Unit unit = item.Unit;
            int result = await query.Query("unit_attributes").
                Where("unit_attributes_id", unit.storage.unit_attributes_id).
                DeleteAsync(transaction);

            var values = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "part" , part.GetType().GetField(part.ToString()) },
            };

            return await query.Query("gear")
                .Where(values)
                .DeleteAsync(transaction);
        }

        public async Task<int> UpdateGear(int uid, ItemObject oldItem, ItemObject newItem, EGearPart part, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            var oldValues = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "part" , part.GetType().GetField(part.ToString()) },
                { "unit_attributes_id", oldItem.UnitAttributesId }
            };

            var newValues = new Dictionary<string, object>()
            {
                { "part" , part.GetType().GetField(part.ToString()) },
                { "unit_attributes_id", newItem.UnitAttributesId },
            };

            return await query.Query("gear")
                .Where(oldValues)
                .UpdateAsync(newValues, transaction);
        }
    }
}
