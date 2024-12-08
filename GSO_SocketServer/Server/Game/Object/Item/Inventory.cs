using Server.Database.Game;
using Server.Database.Handler;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Game
{

    public class Inventory : GameObject
    {
        public Player owner;                //소유자 (플레이어)
        public int storage_id = 0;          //가방 아이디

        public Storage storage = new Storage();
        private List<int> initItemIds = new List<int>();




        public enum ItemType
        {
            Weapon = 100,        // 100번대 : 무기
            Armor = 200,         // 200번대 : 방어구
            Bag = 300,           // 300번대 : 가방
            RecoveryBuff = 400,  // 400번대 : 회복, 버프
            Ammo = 500,          // 500번대 : 탄알
            Loot = 600           // 600번대 : 전리품
        }



        public ItemObject FindItemByType(ItemType itemType)
        {
            List<int> itemIds = storage.GetAllItemObjectIds();


            foreach (int itemId in itemIds)
            {
                if (itemId / 100 == (int)itemType / 100)
                {
                    var items = storage.FindItemsByItemID(itemId);
                    if (items != null && items.Count > 0)
                    {
                        return items[0];
                    }
                }
            }

            return null;
        }



        public List<ItemObject> FindItemAllByType(ItemType itemType)
        {
            List<ItemObject> items = storage.Items;
            List<ItemObject> findItems = new List<ItemObject>();
            foreach (ItemObject item in items)
            {
                if (item.ItemId / 100 == (int)itemType / 100)
                {
                    if (item != null && item.Amount > 0)
                    {
                        findItems.Add(item);
                    }
                }
            }

            return findItems;
        }





        /// <summary>
        /// 처음 접속한 이후 데이터베이스의 인벤토리 데이터 가져오기
        /// </summary>
        public Inventory(Player owner)
        {
            this.owner = owner;
            Load().Wait();
        }

        /// <summary>
        /// 새로운 가방을 장착 시
        /// </summary>
        public void MakeInventory(int storage_id)
        {

        }

        /// <summary>
        /// 가방에 따른 인벤토리 초기화
        /// </summary>
        public void InitInventory()
        {
            try
            {
                //장비에 착용되어 있는 가방을 불러옴d
                ItemObject backpackItem = owner.gear.GetPartItem(EGearPart.Backpack);

                if (backpackItem == null)
                {
                    return;
                }

                //마스터 테이블의 아이템 데이터 불러와서 가방의 정보 얻기
                FMasterItemBackpack backpackData = DatabaseHandler.Context.MasterItemBackpack.Find(backpackItem.ItemId);
                int scaleX = backpackData.total_scale_x;
                int scaleY = backpackData.total_scale_y;
                double weight = backpackData.total_weight;

                this.storage_id = backpackItem.UnitStorageId.Value;
                storage.Init(scaleX, scaleY, weight);

                Console.WriteLine($"Player.UID[{owner.UID}] 가방 아이디 {this.storage_id}");

            }
            catch (Exception e)
            {
                Console.WriteLine($"[InitInventory] : {e.Message.ToString()}");
            }
        }

        /// <summary>
        /// 저장소 아이디에 있는 모든 아이템 불러오기
        /// </summary>
        public async Task Load()
        {
            try
            {
                Console.WriteLine($"Player.UID[{owner.UID}] 인벤토리 로드 시작");

                InitInventory();
                if(this.storage_id == 0)
                {
                    return;
                }

                IEnumerable<DB_ItemUnit> units = await DatabaseHandler.GameDB.LoadInventory(this.storage_id);
                if (units == null)
                {
                    return;
                }

                Console.WriteLine($"Player.UID[{owner.UID}] 인벤토리 아이템 개수 {units.Count()}");
                foreach (DB_ItemUnit unit in units)
                {
                    ItemObject newItem = ObjectManager.Instance.Add<ItemObject>();
                    newItem.Init(owner, unit);

                    bool error = storage.InsertItem(newItem);
                    if (error == false)
                    {
                        Console.WriteLine($"Player.UID[{owner.UID}] 인벤토리 {newItem.Data.name}아이템 삽입 실패");
                    }

                }

                Console.WriteLine($"Player.UID[{owner.UID}] 인벤토리 로드 완료");

            }
            catch (Exception e)
            {
                Console.WriteLine($"[Inventory.Load] : {e.Message.ToString()}");
            }

            
        }

        public async Task Clear()
        {
            using (var database = DatabaseHandler.GameDB)
            {
                using (var transaction = database.GetConnection().BeginTransaction())
                {
                    try
                    {
                        foreach (ItemObject item in this.storage.Items)
                        {
                            if (item == null)
                            {
                                continue;
                            }

                            DB_ItemUnit unit = item.Unit;
                            int ret = await database.DeleteItem(storage_id, unit, transaction);
                            if (ret == 0)
                            {
                                throw new Exception("데이터베이스에서 아이템을 삭제하지 못함");
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[Inventory.Clear] Error :{e.Message.ToString()}");
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
                        foreach (ItemObject item in this.storage.Items)
                        {
                            if (item == null)
                            {
                                continue;
                            }

                            int ret = await database.InsertItem(storage_id, item, transaction);
                            if (ret == 0)
                            {
                                throw new Exception("데이터베이스에서 아이템을 삽입하지 못함");
                            }
                        }
                        
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[Inventory.Save] Error : {e.Message.ToString()}");
                        transaction.Rollback();
                    }
                }
            }
        }

        //public async Task<bool> InsertItem(ItemObject item, GameDB database, IDbTransaction transaction = null)
        //{

        //    int ret = await database.InsertItem(storage_id, item, transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception("데이터베이스에서 아이템을 삽입하지 못함");
        //    }
        //    return true;
        //}

        /// <summary>
        /// 자신의 인벤토리에 있는 아이템 삭제
        /// </summary>
        //public async Task<bool> DeleteItem(ItemObject item, GameDB database, IDbTransaction transaction = null)
        //{
        //    DB_ItemUnit unit = item.Unit;
        //    int ret = await database.DeleteItem(storage_id, unit, transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception("데이터베이스에서 아이템을 삭제하지 못함");
        //    }
        //    return true;
        //}

        //public async Task<bool> UpdateItem(ItemObject oldItem, ItemObject newItem, GameDB database, IDbTransaction transaction = null)
        //{
        //    int ret = await database.UpdateItemAttributes(newItem, transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception("인벤토리에서 아이템 업데이트 안됨");
        //    }

        //    ret = await database.UpdateItem(storage_id, oldItem, newItem, transaction);
        //    if (ret == 0)
        //    {
        //        throw new Exception("인벤토리에서 아이템 업데이트 안됨");
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

        //public List<int> GetInventoryObjectIds()
        //{
        //    return storage.GetAllItemObjectIds().ToList();
        //}

        //public List<int> GetInitInventoryObjectIds()
        //{
        //    return initItemIds;
        //}

        //public void ClearInventory()
        //{
        //    storage.ClearStorage();
        //}
    }
}
