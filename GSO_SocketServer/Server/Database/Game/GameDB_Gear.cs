using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using Server.Game;
using Server.Game.Object.Gear;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;

namespace Server.Database.Game
{
    public partial class GameDB : MySQL
    {
        public async Task<IEnumerable<DB_GearUnit>> LoadGear(int uid)
        {
            var query = this.GetQueryFactory();

            var result = await query.Query("gear")
                .Select
                (
                        "gear.part",
                        "gear.unit_attributes_id",

                        "unit_attributes.item_id",
                        "unit_attributes.durability",
                        "unit_attributes.loaded_ammo",
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
                    part = row.part as string,
                    unit_attributes_id = row.unit_attributes_id,
                },

                attributes = new DB_UnitAttributes
                {
                    item_id         = row.item_id,
                    durability      = row.durability,
                    loaded_ammo     = row.loaded_ammo,
                    unit_storage_id = row.unit_storage_id as int?,
                    amount          = row.amount
                }

            }).ToList();

            return gears;
        }

        public async Task<DB_GearUnit> GetGearOfPart(int uid, string part, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            var values = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "part" , part },
            };

            var result = await query.Query("gear")
                .Select
                (
                        "gear.part",
                        "gear.unit_attributes_id",

                        "unit_attributes.item_id",
                        "unit_attributes.durability",
                        "unit_attributes.loaded_ammo",
                        "unit_attributes.unit_storage_id",
                        "unit_attributes.amount"
                )
                .LeftJoin("unit_attributes", "gear.unit_attributes_id", "unit_attributes.unit_attributes_id")
                .Where(values)
                .GetAsync(transaction);

            DB_GearUnit gear = result.Select(row => new DB_GearUnit
            {
                gear = new DB_Gear
                {
                    part = row.part as string,
                    unit_attributes_id = row.unit_attributes_id,
                },

                attributes = new DB_UnitAttributes
                {
                    item_id = row.item_id,
                    durability = row.durability,
                    loaded_ammo = row.loaded_ammo,
                    unit_storage_id = row.unit_storage_id as int?,
                    amount = row.amount
                }

            }).FirstOrDefault();

            return gear;
        }

        public async Task CreateDefaultBackpack(int uid, FMasterItemBase data, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            EGearPart part = EGearPart.Backpack;
            FieldInfo field = part.GetType().GetField(part.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            string description = attribute?.Description ?? part.ToString();

            int storge_id = await query.Query("storage").
                InsertGetIdAsync<int>(new
                {
                    storage_type = description
                }, transaction);


            int unit_attributes_id = await query.Query("unit_attributes").
            InsertGetIdAsync<int>(new
            {
                item_id = data.item_id,
                durability = 0,
                loaded_ammo = 0,
                unit_storage_id = storge_id,
                amount = 1
            }, transaction);
            


            int result = await query.Query("gear").
            InsertAsync(new
                {
                    uid = uid,
                    part = description,
                    unit_attributes_id = unit_attributes_id,
                }, transaction);
        }

        public async Task<int> InsertGear(int uid, ItemObject item, string part, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            DB_ItemUnit unit = item.Unit;
            int unit_attributes_id = unit.storage.unit_attributes_id;
            unit_attributes_id = await query.Query("unit_attributes").
            InsertGetIdAsync<int>(new
            {
                item_id = unit.attributes.item_id,
                durability = unit.attributes.durability,
                loaded_ammo = unit.attributes.loaded_ammo,
                unit_storage_id = unit.attributes.unit_storage_id,
                amount = unit.attributes.amount
            }, transaction);

            item.UnitAttributesId = unit_attributes_id;

            return await query.Query("gear").
                InsertAsync(new
                {
                    uid = uid,
                    part = part,
                    unit_attributes_id = unit_attributes_id,
                }, transaction);
        }

        public async Task<int> DeleteGear(int uid, ItemObject item, string part, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            var values = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "part" , part },
            };

            int result = await query.Query("gear")
                .Where(values)
                .DeleteAsync(transaction);

            if(result == 0)
            {
                return 0;
            }

            DB_ItemUnit unit = item.Unit;
            return await query.Query("unit_attributes").
                Where("unit_attributes_id", unit.storage.unit_attributes_id).
                DeleteAsync(transaction);

        }

        public async Task<int> UpdateGear(int uid, ItemObject oldItem, ItemObject newItem, string part, IDbTransaction transaction = null)
        {
            var query = this.GetQueryFactory();

            var oldValues = new Dictionary<string, object>()
            {
                { "uid"  , uid },
                { "part" , part },
                { "unit_attributes_id", oldItem.UnitAttributesId }
            };

            var newValues = new Dictionary<string, object>()
            {
                { "part" , part },
                { "unit_attributes_id", newItem.UnitAttributesId },
            };

            return await query.Query("gear")
                .Where(oldValues)
                .UpdateAsync(newValues, transaction);
        }
    }
}
