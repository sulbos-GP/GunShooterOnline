using WebCommonLibrary.Models.GameDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using SqlKata.Execution;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models.MasterDatabase;

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

        public async Task<int> DeleteGearItem(int uid, DB_GearUnit unit, string part, IDbTransaction? transaction = null)
        {
            var query = this.mQueryFactory;

            var values = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "part" , part },
            };

            int result = await query.Query("gear")
                .Where(values)
                .DeleteAsync(transaction);

            if (result == 0)
            {
                return 0;
            }

            return await query.Query("unit_attributes").
                Where("unit_attributes_id", unit.gear.unit_attributes_id).
                DeleteAsync(transaction);
        }

        public async Task<int> InsertGearItem(int uid, DB_GearUnit unit, string part, IDbTransaction? transaction = null)
        {
            var query = this.mQueryFactory;

            int unit_attributes_id = unit.gear.unit_attributes_id;
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
            unit.gear.unit_attributes_id = unit_attributes_id;

            return await query.Query("gear").
                InsertAsync(new
                {
                    uid = uid,
                    part = part,
                    unit_attributes_id = unit_attributes_id,
                }, transaction);
        }

        public async Task<int> InsertGearBackpackItem(int uid, DB_GearUnit unit, IDbTransaction? transaction = null)
        {
            var query = this.mQueryFactory;

            int storge_id = await query.Query("storage").
                InsertGetIdAsync<int>(new
                {
                    storage_type = "backpack"
                }, transaction);


            int unit_attributes_id = await query.Query("unit_attributes").
            InsertGetIdAsync<int>(new
            {
                item_id = unit.attributes.item_id,
                durability = unit.attributes.durability,
                loaded_ammo = unit.attributes.loaded_ammo,
                unit_storage_id = storge_id,
                amount = unit.attributes.amount
            }, transaction);



            int result = await query.Query("gear").
            InsertAsync(new
            {
                uid = uid,
                part = "backpack",
                unit_attributes_id = unit_attributes_id,
            }, transaction);

            return storge_id;
        }
    }
}
