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

    }

    public class DatabaseContext
    {
	    #region DatabaseTable 
        private DatabaseTable<DB_ItemBase> itemBase = new DatabaseTable<DB_ItemBase>();
        private DatabaseTable<DB_ItemBackpack> itemBackpack = new DatabaseTable<DB_ItemBackpack>();
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
            

        public async Task LoadDatabaseContext()
        { 
            ItemBase.LoadTable(await DatabaseHandler.MasterDB.LoadTable<DB_ItemBase>("master_item_base"));
            ItemBackpack.LoadTable(await DatabaseHandler.MasterDB.LoadTable<DB_ItemBackpack>("master_item_backpack"));
        }
        
        
        public DB_ItemBase GetItemBase(int id)
        {
            return ItemBase.Get(id);
        }
            
        public DB_ItemBackpack GetItemBackpack(int id)
        {
            return ItemBackpack.Get(id);
        }
            

    }
}