using System.ComponentModel;
using System.Threading.Tasks;
using WebCommonLibrary.Models.MasterDB;
using WebCommonLibrary.Models.GameDB;

namespace Server.Database.Handler
{
    
    //**** Context 확인용 ****//
    public enum EDatabaseTable
    { 
        [Description ("master_item_base")]
        ItemBase,

        [Description ("master_item_backpack")]
        ItemBackpack,

        [Description ("master_reward_base")]
        RewardBase,

        [Description ("master_reward_box")]
        RewardBox,

        [Description ("master_reward_level")]
        RewardLevel,

    }

    public class DatabaseContext
    {
	    #region DatabaseTable 
        private DatabaseTable<DB_ItemBase> itemBase = new DatabaseTable<DB_ItemBase>();
        private DatabaseTable<DB_ItemBackpack> itemBackpack = new DatabaseTable<DB_ItemBackpack>();
        private DatabaseTable<DB_RewardBase> rewardBase = new DatabaseTable<DB_RewardBase>();
        private DatabaseTable<DB_RewardBox> rewardBox = new DatabaseTable<DB_RewardBox>();
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
            
        public DatabaseTable<DB_RewardLevel> RewardLevel
        {
            get
            {
                return rewardLevel;
            }
        }
            

        public async Task LoadDatabaseContext()
        { 
            ItemBase.LoadTable(await DatabaseHandler.MasterDB.LoadTable<DB_ItemBase>("master_item_base"));
            ItemBackpack.LoadTable(await DatabaseHandler.MasterDB.LoadTable<DB_ItemBackpack>("master_item_backpack"));
            RewardBase.LoadTable(await DatabaseHandler.MasterDB.LoadTable<DB_RewardBase>("master_reward_base"));
            RewardBox.LoadTable(await DatabaseHandler.MasterDB.LoadTable<DB_RewardBox>("master_reward_box"));
            RewardLevel.LoadTable(await DatabaseHandler.MasterDB.LoadTable<DB_RewardLevel>("master_reward_level"));
        }
        
        
        public DB_ItemBase GetItemBase(int id)
        {
            return ItemBase.Get(id);
        }
            
        public DB_ItemBackpack GetItemBackpack(int id)
        {
            return ItemBackpack.Get(id);
        }
            
        public DB_RewardBase GetRewardBase(int id)
        {
            return RewardBase.Get(id);
        }
            
        public DB_RewardBox GetRewardBox(int id)
        {
            return RewardBox.Get(id);
        }
            
        public DB_RewardLevel GetRewardLevel(int id)
        {
            return RewardLevel.Get(id);
        }
            

    }
}