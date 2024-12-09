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
using static Humanizer.In;
using WebCommonLibrary.Models.MasterDatabase;
using System.ComponentModel;
using System.Reflection;
using System.Transactions;
using StackExchange.Redis;
using System.Data.Common;
using Server.Game.Object.Item;

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
            Load().Wait();
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

            if(part == EGearPart.None)
            {
                return null;
            }

            Storage storage = parts[ConvertPartToString(part)];
            if(storage.ItemCount == 0)
            {
                return null;
            }
            return storage.GetItem();
        }

        public List<ItemObject> GetPartItems()
        {
            List<ItemObject> items = new List<ItemObject>();
            EGearPart[] statuses = (EGearPart[])System.Enum.GetValues(typeof(EGearPart));
            for (int i = 1; i < statuses.Length; i++)
            {
                ItemObject item = GetPartItem(statuses[i]);
                if (item == null)
                {
                    continue;
                }

                items.Add(item);

            }
            return items;
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

        public async Task Load()
        {

            try
            {
                Console.WriteLine($"Player.UID[{owner.UID}] 장비 로드 시작");

                IEnumerable<DB_GearUnit> gears = await DatabaseHandler.GameDB.LoadGear(owner.UID);
                if (gears == null)
                {
                    return;
                }

                Console.WriteLine($"Player.UID[{owner.UID}] 장비 아이템 개수 {gears.Count()}");
                foreach (DB_GearUnit gear in gears)
                {
                    DB_ItemUnit unit = new DB_ItemUnit()
                    {
                        storage = new DB_StorageUnit()
                        {
                            grid_x = 0,
                            grid_y = 0,
                            rotation = 0,
                            unit_attributes_id = gear.gear.unit_attributes_id,
                        },

                        attributes = new DB_UnitAttributes()
                        {
                            item_id = gear.attributes.item_id,
                            durability = gear.attributes.durability,
                            unit_storage_id = gear.attributes.unit_storage_id,
                            amount = gear.attributes.amount,
                        }
                    };

                    ItemObject item = ObjectManager.Instance.Add<ItemObject>();
                    item.Init(owner, unit);

                    Storage part = parts[gear.gear.part];
                    if (false == part.InsertItem(item))
                    {
                        throw new Exception($"장비의 파트({gear.gear.part})가 중복되어 있음");
                    }
                    else
                    {
                        Console.WriteLine($"{gear.gear.part.ToString()}부위 {item.Data.name.ToString()}장착");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Gear.Load] Error : {e.Message.ToString()}");
            }

            ItemObject haveBackpack = GetPartItem(EGearPart.Backpack);
            if (haveBackpack == null)
            {
                CreateDefaultBackpack();
            }



            Console.WriteLine($"Player.UID[{owner.UID}] 장비 로드 완료");
        }
        
        public async Task Clear()
        {
            using (var database = DatabaseHandler.GameDB)
            {
                using (var transaction = database.GetConnection().BeginTransaction())
                {
                    try
                    {

                        EGearPart[] parts = (EGearPart[])System.Enum.GetValues(typeof(EGearPart));
                        for (int i = 1; i < parts.Length; i++)
                        {
                            EGearPart part = parts[i];
                            ItemObject item = GetPartItem(part);
                            if (item == null || item.UnitAttributesId == 0)
                            {
                                continue;
                            }

                            int ret = await database.DeleteGear(owner.UID, item, ConvertPartToString(part), transaction);
                            if (ret == 0)
                            {
                                throw new Exception($"장비[{part}]에서 아이템을 삭제하지 못함");
                            }

                            if (part == EGearPart.Backpack)
                            {
                                await database.DeleteStorage(item.UnitStorageId, transaction);
                            }
                        }

                        //transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[Gear.Clear] Error: {e.Message.ToString()}");
                        transaction.Rollback();
                    }
                }
            }
        }

        public async Task Save()
        {
            using (var database = DatabaseHandler.GameDB)
            {
                using (var transaction = database.GetConnection().BeginTransaction())
                {
                    try
                    {
                        EGearPart[] parts = (EGearPart[])System.Enum.GetValues(typeof(EGearPart));
                        for (int i = 1; i < parts.Length; i++)
                        {
                            EGearPart part = parts[i];
                            ItemObject item = GetPartItem(part);
                            if (item == null)
                            {
                                continue;
                            }

                            if (part == EGearPart.Backpack)
                            {
                                int storage_id = await database.InsertGetStorageId(transaction);
                                owner.inventory.storage_id = storage_id;
                                item.UnitStorageId = storage_id;
                            }

                            int ret = await database.InsertGear(owner.UID, item, ConvertPartToString(part), transaction);
                            if (ret == 0)
                            {
                                throw new Exception($"장비[{part}]에서 아이템을 삽입하지 못함");
                            }

                        }
                        //transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[Gear.Save] {e.Message.ToString()}");
                        transaction.Rollback();
                    }
                }
            }
        }

        public void CreateDefaultBackpack()
        {
            

            FMasterItemBase defaultBackpackBaseData = DatabaseHandler.Context.MasterItemBase.FirstOrDefault(data => data.Value.code == "ITEM_B000").Value;
            if (defaultBackpackBaseData == null)
            {
                Console.WriteLine("기본 가방의 베이스 데이터를 얻을 수 없습니다.");
                return;
            }

            FMasterItemBackpack defaultBackpackData = DatabaseHandler.Context.MasterItemBackpack.Find(defaultBackpackBaseData.item_id);
            if (defaultBackpackData == null)
            {
                Console.WriteLine("기본 가방의 디테일 데이터를 얻을 수 없습니다.");
                return;
            }

            EGearPart type = EGearPart.Backpack;
            FieldInfo field = type.GetType().GetField(type.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            string description = attribute?.Description ?? type.ToString();

            DB_ItemUnit unit = new DB_ItemUnit()
            {
                storage = new DB_StorageUnit()
                {
                    grid_x = 0,
                    grid_y = 0,
                    rotation = 0,
                    unit_attributes_id = 0
                },

                attributes = new DB_UnitAttributes()
                {
                    item_id = defaultBackpackBaseData.item_id,
                    durability = 0,
                    unit_storage_id = 0,
                    amount = 1,
                }
            };

            ItemObject item = ObjectManager.Instance.Add<ItemObject>();
            item.Init(owner, unit);

            Storage part = parts[description];
            if (false == part.InsertItem(item))
            {
                throw new Exception($"장비의 파트({description})가 중복되어 있음");
            }
            else
            {
                Console.WriteLine($"{description}부위 {item.Data.name.ToString()}장착");
            }
        }


        //public async Task<bool> InsertGear(EGearPart part, ItemObject item, GameDB database, IDbTransaction transaction = null)
        //{

        //    if (item.Type == "Backpack" && item.UnitStorageId == null)
        //    {
        //        int storage_id = await database.InsertGetStorageId(transaction);
        //        item.UnitStorageId = storage_id;
        //    }

        //    int ret = await database.InsertGear(owner.UID, item, ConvertPartToString(part), transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception($"장비[{part}]에서 아이템을 삽입하지 못함");
        //    }
        //    return true;
        //}

        //public async Task<bool> DeleteGear(EGearPart part, ItemObject item, GameDB database, IDbTransaction transaction = null)
        //{

        //    if (item.Type == "Backpack" && item.UnitStorageId != null)
        //    {
        //        await database.DeleteStorage(item.UnitStorageId, transaction);
        //    }

        //    int ret = await database.DeleteGear(owner.UID, item, ConvertPartToString(part), transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception($"장비[{part}]에서 아이템을 삭제하지 못함");
        //    }
        //    return true;
        //}

   



        //public async Task<bool> UpdateGear(EGearPart part, ItemObject oldItem, ItemObject newItem, GameDB database, IDbTransaction transaction = null)
        //{
        //    int ret = await database.UpdateItemAttributes(newItem, transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception("인벤토리에서 아이템 업데이트 안됨");
        //    }

        //    ret = await database.UpdateGear(owner.UID, oldItem, newItem, ConvertPartToString(part), transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception("장비에서 아이템 업데이트 안됨");
        //    }
        //    return true;
        //}

        //public async Task<bool> UpdateItemAttributes(ItemObject item, GameDB database, IDbTransaction transaction = null)
        //{
        //    int ret = await database.UpdateItemAttributes(item, transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception("인벤토리에서 아이템 속성 업데이트 안됨");
        //    }
        //    return true;
        //}
    }
}
