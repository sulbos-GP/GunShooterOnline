
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebCommonLibrary.Reposiotry;
using WebCommonLibrary.Models.MasterDatabase;
using WebCommonLibrary.Reposiotry.Interfaces;

namespace WebCommonLibrary.Reposiotry.MasterDatabase
{
    public class MasterDatabaseContext : DbContext, IMasterDatabaseContext
    {
        private readonly string _connectionString;

        #region DatabaseContext
        public DbTable<FMasterItemBackpack> MasterItemBackpack { get; }
        public DbTable<FMasterItemWeapon> MasterItemWeapon { get; }
        public DbTable<FMasterItemBase> MasterItemBase { get; }
        public DbTable<FMasterItemUse> MasterItemUse { get; }
        public DbTable<FMasterRewardBox> MasterRewardBox { get; }
        public DbTable<FMasterRewardBase> MasterRewardBase { get; }
        public DbTable<FMasterRewardBoxItem> MasterRewardBoxItem { get; }
        public DbTable<FMasterRewardLevel> MasterRewardLevel { get; }
        public DbTable<FMasterVersionApp> MasterVersionApp { get; }
        public DbTable<FMasterVersionData> MasterVersionData { get; }
        #endregion

        public MasterDatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
            
            MasterItemBackpack = LoadMasterItemBackpack().Result;
            MasterItemWeapon = LoadMasterItemWeapon().Result;
            MasterItemBase = LoadMasterItemBase().Result;
            MasterItemUse = LoadMasterItemUse().Result;
            MasterRewardBox = LoadMasterRewardBox().Result;
            MasterRewardBase = LoadMasterRewardBase().Result;
            MasterRewardBoxItem = LoadMasterRewardBoxItem().Result;
            MasterRewardLevel = LoadMasterRewardLevel().Result;
            MasterVersionApp = LoadMasterVersionApp().Result;
            MasterVersionData = LoadMasterVersionData().Result;
        }

        public void Dispose()
        {

        }

        
        private async Task<DbTable<FMasterItemBackpack>> LoadMasterItemBackpack()
        {
            string query = "SELECT item_id, total_scale_x, total_scale_y, total_weight FROM master_item_backpack;";
            return await LoadDatabaseTable<FMasterItemBackpack>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterItemWeapon>> LoadMasterItemWeapon()
        {
            string query = "SELECT item_id, attack_range, damage, distance, reload_round, attack_speed, reload_time, bullet FROM master_item_weapon;";
            return await LoadDatabaseTable<FMasterItemWeapon>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterItemBase>> LoadMasterItemBase()
        {
            string query = "SELECT item_id, code, name, weight, type, description, scale_x, scale_y, purchase_price, inquiry_time, sell_price, amount, icon FROM master_item_base;";
            return await LoadDatabaseTable<FMasterItemBase>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterItemUse>> LoadMasterItemUse()
        {
            string query = "SELECT item_id, energy, active_time, duration, effect, cool_time FROM master_item_use;";
            return await LoadDatabaseTable<FMasterItemUse>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterRewardBox>> LoadMasterRewardBox()
        {
            string query = "SELECT reward_box_id, box_scale_x, box_scale_y FROM master_reward_box;";
            return await LoadDatabaseTable<FMasterRewardBox>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterRewardBase>> LoadMasterRewardBase()
        {
            string query = "SELECT reward_id, money, ticket, gacha, experience, reward_box_id FROM master_reward_base;";
            return await LoadDatabaseTable<FMasterRewardBase>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterRewardBoxItem>> LoadMasterRewardBoxItem()
        {
            string query = "SELECT reward_box_item_id, reward_box_id, item_code, x, y, rotation, amount FROM master_reward_box_item;";
            return await LoadDatabaseTable<FMasterRewardBoxItem>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterRewardLevel>> LoadMasterRewardLevel()
        {
            string query = "SELECT reward_id, level, name, icon FROM master_reward_level;";
            return await LoadDatabaseTable<FMasterRewardLevel>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterVersionApp>> LoadMasterVersionApp()
        {
            string query = "SELECT major, minor, patch FROM master_version_app;";
            return await LoadDatabaseTable<FMasterVersionApp>(_connectionString, query);
        }
            
        private async Task<DbTable<FMasterVersionData>> LoadMasterVersionData()
        {
            string query = "SELECT major, minor, patch FROM master_version_data;";
            return await LoadDatabaseTable<FMasterVersionData>(_connectionString, query);
        }
            

        
        public override bool IsValidContext()
        {
            if ( 
                MasterItemBackpack.IsValid() == false || 
                MasterItemWeapon.IsValid() == false || 
                MasterItemBase.IsValid() == false || 
                MasterItemUse.IsValid() == false || 
                MasterRewardBox.IsValid() == false || 
                MasterRewardBase.IsValid() == false || 
                MasterRewardBoxItem.IsValid() == false || 
                MasterRewardLevel.IsValid() == false || 
                MasterVersionApp.IsValid() == false || 
                MasterVersionData.IsValid() == false ||
                false)
            {
                return false;
            }

            return true;
        }
            
    }
}
            