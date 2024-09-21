
using System;
using WebCommonLibrary.Models.MasterDatabase;

namespace WebCommonLibrary.Reposiotry.Interfaces
{

    public interface IMasterDatabaseContext : IDisposable
    {
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
    }

}