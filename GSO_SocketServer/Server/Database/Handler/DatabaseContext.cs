using Server.Database.Data;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Server.Database.Handler
{
    
    //**** Context 확인용 ****//
    public enum EDatabaseTable
    { 
        [Description ("master_item_base")]
        ItemBase,

    }

    public class DatabaseContext
    {
	    #region DatabaseTable 
        private DatabaseTable<DB_ItemBase> itemBase = new DatabaseTable<DB_ItemBase>();
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
            

        public async Task LoadDatabaseContext()
        { 
            ItemBase.LoadTable(await DatabaseHandler.MasterDB.LoadTable<DB_ItemBase>("master_item_base"));
        }
        
        
        public DB_ItemBase GetItemBase(int id)
        {
            return ItemBase.Get(id);
        }
            

    }
}