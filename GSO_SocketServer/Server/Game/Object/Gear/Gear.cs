using Google.Protobuf.Protocol;
using Server.Database.Game;
using Server.Database.Handler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace Server.Game.Object.Gear
{
    public class Gear : GameObject
    {
        private Dictionary<string, Storage> parts = new Dictionary<string, Storage>();
        private List<int> initPartItemIds = new List<int>();
        private Player owner;

        public Gear(Player owner) 
        {
            this.owner = owner;
            InitGear();
            LoadGear().Wait();
        }

        public string ConvertPartToString(EGearPart part)
        {
            string partString = "";
            switch (part)
            {
                case EGearPart.MainWeapon:      partString =  "main_weapon";    break;
                case EGearPart.SubWeapon:       partString =  "sub_weapon";     break;
                case EGearPart.Armor:           partString =  "armor";          break;
                case EGearPart.Backpack:        partString =  "backpack";       break;
                case EGearPart.PocketFirst:     partString =  "pocket_first";   break;
                case EGearPart.PocketSecond:    partString =  "pocket_second";  break;
                case EGearPart.PocketThird:     partString =  "pocket_third";   break;
            }
            return partString;
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

        public IEnumerable<PS_GearInfo> GetPartItems(int viewerId)
        {

            List<PS_GearInfo> infos = new List<PS_GearInfo>();

            EGearPart[] statuses = (EGearPart[])System.Enum.GetValues(typeof(EGearPart));
            for (int i = 1; i < statuses.Length; i++)
            {
                ItemObject item = GetPartItem(statuses[i]);
                if(item == null)
                {
                    continue;
                }

                PS_GearInfo info = new PS_GearInfo();
                info.Part = (PE_GearPart)i;
                info.Item = item.ConvertItemInfo(viewerId);

                infos.Add(info);

            }
            return infos;
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
                IEnumerable<DB_GearUnit> gears = await DatabaseHandler.GameDB.LoadGear(owner.UID);

                if (gears == null)
                {
                    return;
                }

                foreach (DB_GearUnit gear in gears)
                {
                    DB_ItemUnit unit = new DB_ItemUnit()
                    {
                        storage = new DB_StorageUnit()
                        {
                            grid_x = 0,
                            grid_y = 0,
                            rotation = 0,
                            unit_attributes_id = gear.gear.unit_attributes_id
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
                    Storage part = parts[gear.gear.part];
                    if (false == part.InsertItem(item))
                    {
                        throw new Exception($"장비의 파트({gear.gear.part})가 중복되어 있음");
                    }
                    initPartItemIds.Add(item.Id);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"[LoadGear] : {e.Message.ToString()}");
            }
        }

        public async Task<bool> InsertGear(EGearPart part, ItemObject item, GameDB database, IDbTransaction transaction = null)
        {

            if (item.Type == "Backpack" && item.UnitStorageId == null)
            {
                int storage_id = await database.InsertGetStorageId(transaction);
                item.UnitStorageId = storage_id;
            }

            int ret = await database.InsertGear(owner.UID, item, ConvertPartToString(part), transaction);
            if (ret == 0)
            {
                throw new Exception($"장비[{part}]에서 아이템을 삽입하지 못함");
            }
            return true;
        }

        public async Task<bool> DeleteGear(EGearPart part, ItemObject item, GameDB database, IDbTransaction transaction = null)
        {

            if (item.Type == "Backpack" && item.UnitStorageId != null)
            {
                await database.DeleteStorage(item.UnitStorageId, transaction);
            }

            int ret = await database.DeleteGear(owner.UID, item, ConvertPartToString(part), transaction);
            if (ret == 0)
            {
                throw new Exception($"장비[{part}]에서 아이템을 삭제하지 못함");
            }
            return true;
        }

        public async Task<bool> UpdateGear(EGearPart part, ItemObject oldItem, ItemObject newItem, GameDB database, IDbTransaction transaction = null)
        {
            int ret = await database.UpdateItemAttributes(newItem, transaction);
            if (ret == 0)
            {
                throw new Exception("인벤토리에서 아이템 업데이트 안됨");
            }

            ret = await database.UpdateGear(owner.UID, oldItem, newItem, ConvertPartToString(part), transaction);
            if (ret == 0)
            {
                throw new Exception("장비에서 아이템 업데이트 안됨");
            }
            return true;
        }

        public async Task<bool> UpdateItemAttributes(ItemObject item, GameDB database, IDbTransaction transaction = null)
        {
            int ret = await database.UpdateItemAttributes(item, transaction);
            if (ret == 0)
            {
                throw new Exception("인벤토리에서 아이템 속성 업데이트 안됨");
            }
            return true;
        }

        public List<int> GetPartObjectIds()
        {
            List<int> itemIds = new List<int>();
            foreach (var (part, item) in parts)
            {
                var items = item.GetItemObjectIds();
                if(items.Count == 1)
                {
                    itemIds.Add(items[0]);
                }
            }
            return itemIds;
        }

        public List<int> GetInitPartObjectIds()
        {
            return initPartItemIds;
        }

    }
}
