using WebCommonLibrary.Models.MasterDB;

namespace GSO_WebServerLibrary.Reposiotry.Interfaces
{

    public interface IMasterDataDB : IDisposable
    {

        public DB_Version GetAppVersion();
        public DB_Version GetDataVersion();

        
        public DB_ItemBase GetItemBase(int id);
        public DB_ItemBackpack GetItemBackpack(int id);
        public DB_ItemUse GetItemUse(int id);
        public DB_ItemWeapon GetItemWeapon(int id);
        public DB_RewardBase GetRewardBase(int id);
        public DB_RewardBox GetRewardBox(int id);
        public DB_RewardLevel GetRewardLevel(int id);
    
        
        public Dictionary<int, DB_ItemBase> GetItemBaseList();
        public Dictionary<int, DB_ItemBackpack> GetItemBackpackList();
        public Dictionary<int, DB_ItemUse> GetItemUseList();
        public Dictionary<int, DB_ItemWeapon> GetItemWeaponList();
        public Dictionary<int, DB_RewardBase> GetRewardBaseList();
        public Dictionary<int, DB_RewardBox> GetRewardBoxList();
        public Dictionary<int, DB_RewardLevel> GetRewardLevelList();
    }
}