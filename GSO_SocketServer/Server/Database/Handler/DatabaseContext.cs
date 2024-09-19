using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using WebCommonLibrary.Models.MasterDB;
using WebCommonLibrary.Models.GameDB;

namespace Server.Database.Handler
{
    
    public class DatabaseTable<T> where T : class
    {

        protected Dictionary<int, T> datas = new Dictionary<int, T>();

        public void LoadTable(IEnumerable<T> table)
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (T data in table)
            {
                object? obj = fields[0].GetValue(data);
                if (obj == null)
                {
                    continue;
                }
                datas.Add((int)obj, data);
            }
        }

        public T Get(int id)
        {
            return datas[id];
        }

        public Dictionary<int, T> GetList()
        {
            return datas;
        }
    }

    public class DatabaseContext
    {
	    #region DatabaseTables 
        private DatabaseTable<DB_ItemBase> itemBase = new DatabaseTable<DB_ItemBase>();
        private DatabaseTable<DB_ItemBackpack> itemBackpack = new DatabaseTable<DB_ItemBackpack>();
        private DatabaseTable<DB_ItemUse> itemUse = new DatabaseTable<DB_ItemUse>();
        private DatabaseTable<DB_ItemWeapon> itemWeapon = new DatabaseTable<DB_ItemWeapon>();
        private DatabaseTable<DB_RewardBase> rewardBase = new DatabaseTable<DB_RewardBase>();
        private DatabaseTable<DB_RewardBox> rewardBox = new DatabaseTable<DB_RewardBox>();
        private DatabaseTable<DB_RewardBoxItem> rewardBoxItem = new DatabaseTable<DB_RewardBoxItem>();
        private DatabaseTable<DB_RewardLevel> rewardLevel = new DatabaseTable<DB_RewardLevel>();
	    #endregion

        public DatabaseContext()
        {
            
        }

        
        public DatabaseTable<DB_ItemBase> ItemBase
        {
            get
            {
                return itemBase;
            }
        }
            
        public DatabaseTable<DB_ItemBackpack> ItemBackpack
        {
            get
            {
                return itemBackpack;
            }
        }
            
        public DatabaseTable<DB_ItemUse> ItemUse
        {
            get
            {
                return itemUse;
            }
        }
            
        public DatabaseTable<DB_ItemWeapon> ItemWeapon
        {
            get
            {
                return itemWeapon;
            }
        }
            
        public DatabaseTable<DB_RewardBase> RewardBase
        {
            get
            {
                return rewardBase;
            }
        }
            
        public DatabaseTable<DB_RewardBox> RewardBox
        {
            get
            {
                return rewardBox;
            }
        }
            
        public DatabaseTable<DB_RewardBoxItem> RewardBoxItem
        {
            get
            {
                return rewardBoxItem;
            }
        }
            
        public DatabaseTable<DB_RewardLevel> RewardLevel
        {
            get
            {
                return rewardLevel;
            }
        }
            

        public async Task LoadDatabaseContext()
        { 
            itemBase = await LoadTable<DB_ItemBase>("master_item_base");
            itemBackpack = await LoadTable<DB_ItemBackpack>("master_item_backpack");
            itemUse = await LoadTable<DB_ItemUse>("master_item_use");
            itemWeapon = await LoadTable<DB_ItemWeapon>("master_item_weapon");
            rewardBase = await LoadTable<DB_RewardBase>("master_reward_base");
            rewardBox = await LoadTable<DB_RewardBox>("master_reward_box");
            rewardBoxItem = await LoadTable<DB_RewardBoxItem>("master_reward_box_item");
            rewardLevel = await LoadTable<DB_RewardLevel>("master_reward_level");
        }

        private async Task<DatabaseTable<T>> LoadTable<T>(string name) where T : class
        {
            DatabaseTable<T> table = new DatabaseTable<T>();
            var datas = await DatabaseHandler.MasterDB.LoadTable<T>(name);
            table.LoadTable(datas);
            return table;
        }
        
        
        public DB_ItemBase GetItemBase(int id)
        {
            return ItemBase.Get(id);
        }
            
        public DB_ItemBackpack GetItemBackpack(int id)
        {
            return ItemBackpack.Get(id);
        }
            
        public DB_ItemUse GetItemUse(int id)
        {
            return ItemUse.Get(id);
        }
            
        public DB_ItemWeapon GetItemWeapon(int id)
        {
            return ItemWeapon.Get(id);
        }
            
        public DB_RewardBase GetRewardBase(int id)
        {
            return RewardBase.Get(id);
        }
            
        public DB_RewardBox GetRewardBox(int id)
        {
            return RewardBox.Get(id);
        }
            
        public DB_RewardBoxItem GetRewardBoxItem(int id)
        {
            return RewardBoxItem.Get(id);
        }
            
        public DB_RewardLevel GetRewardLevel(int id)
        {
            return RewardLevel.Get(id);
        }
            

        
        public Dictionary<int, DB_ItemBase> GetItemBaseList()
        {
            return ItemBase.GetList();
        }
            
        public Dictionary<int, DB_ItemBackpack> GetItemBackpackList()
        {
            return ItemBackpack.GetList();
        }
            
        public Dictionary<int, DB_ItemUse> GetItemUseList()
        {
            return ItemUse.GetList();
        }
            
        public Dictionary<int, DB_ItemWeapon> GetItemWeaponList()
        {
            return ItemWeapon.GetList();
        }
            
        public Dictionary<int, DB_RewardBase> GetRewardBaseList()
        {
            return RewardBase.GetList();
        }
            
        public Dictionary<int, DB_RewardBox> GetRewardBoxList()
        {
            return RewardBox.GetList();
        }
            
        public Dictionary<int, DB_RewardBoxItem> GetRewardBoxItemList()
        {
            return RewardBoxItem.GetList();
        }
            
        public Dictionary<int, DB_RewardLevel> GetRewardLevelList()
        {
            return RewardLevel.GetList();
        }
            

    }
}