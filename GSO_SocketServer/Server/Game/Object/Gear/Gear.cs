using Google.Protobuf.WellKnownTypes;
using Server.Database.Data;
using Server.Database.Handler;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Transactions;

namespace Server.Game.Object.Gear
{
    public class Gear : GameObject
    {
        private Dictionary<string, Storage> parts = new Dictionary<string, Storage>();
        private Player owner;

        public Gear(Player owner) 
        {
            this.owner = owner;
            InitGear();
            LoadGear().Wait();
        }

        public string ConvertPartToString(EGearPart part)
        {
            switch (part)
            {
                case EGearPart.MainWeapon:      return "main_weapon";
                case EGearPart.SubWeapon:       return "sub_weapon";
                case EGearPart.Armor:           return "armor";
                case EGearPart.Backpack:        return "backpack";
                case EGearPart.PocketFirst:     return "pocket_first";
                case EGearPart.PocketSecond:    return "pocket_second";
                case EGearPart.PocketThird:     return "pocket_third";
            }
            return "";
        }

        public Storage GetPart(EGearPart part)
        {
            return parts[ConvertPartToString(part)];
        }

        public ItemObject GetPartItem(EGearPart part)
        {
            Storage storage = parts[ConvertPartToString(part)];
            if(storage.ItemCount == 0)
            {
                return null;
            }
            return storage.GetItem();
        }

        public void InitGear()
        {
            EGearPart[] statuses = (EGearPart[])System.Enum.GetValues(typeof(EGearPart));
            for (int i = 1; i < statuses.Length; i++)
            {
                Storage storage = new Storage();
                storage.Init(10, 10, 100);
                parts.TryAdd(ConvertPartToString(statuses[i]), storage);
            }
        }

        public async Task LoadGear()
        {
            try
            {
                IEnumerable<DB_Gear> gears = await DatabaseHandler.GameDB.LoadGear(owner.uid);

                if (gears == null)
                {
                    return;
                }

                foreach (DB_Gear gear in gears)
                {
                    DB_Unit unit = new DB_Unit()
                    {
                        storage = new DB_StorageUnit()
                        {
                            grid_x = 0,
                            grid_y = 0,
                            rotation = 0,
                            unit_attributes_id = gear.unit_attributes_id
                        },

                        attributes = new DB_UnitAttributes()
                        {
                            item_id = gear.attributes.item_id,
                            durability = gear.attributes.durability,
                            unit_storage_id = gear.attributes.unit_storage_id,
                            amount = gear.attributes.amount,
                        }
                    };

                    ItemObject item = new ItemObject(owner.Id, unit);
                    Storage part = parts[gear.part];
                    if (false == part.InsertItem(item))
                    {
                        throw new Exception($"장비의 파트({gear.part})가 중복되어 있음");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"[LoadGear] : {e.Message.ToString()}");
            }
        }

        public async Task<bool> InsertGear(EGearPart part, ItemObject item, IDbTransaction transaction = null)
        {

            if (item.Type == "Backpack" && item.UnitStorageId == null)
            {
                int storage_id = await DatabaseHandler.GameDB.InsertGetStorageId(transaction);
                item.UnitStorageId = storage_id;
            }

            int ret = await DatabaseHandler.GameDB.InsertGear(owner.uid, item, part, transaction);
            if (ret == 0)
            {
                throw new Exception($"데이터베이스에서 장비[{part}]아이템을 삽입하지 못함");
            }
            return true;
        }

        public async Task<bool> DeleteGear(EGearPart part, ItemObject item, IDbTransaction transaction = null)
        {

            if (item.Type == "Backpack" && item.UnitStorageId != null)
            {
                await DatabaseHandler.GameDB.DeleteStorage(item.UnitStorageId, transaction);
            }

            int ret = await DatabaseHandler.GameDB.DeleteGear(owner.uid, item, part, transaction);
            if (ret == 0)
            {
                throw new Exception($"데이터베이스에서 장비[{part}]아이템을 삭제하지 못함");
            }
            return true;
        }


    }
}
