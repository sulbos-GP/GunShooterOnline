using System.Collections.Generic;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.DataLoad
{

    public class DataLoadUserInfo
    {
        public FUser? UserInfo { get; set; } = null;
        public FUserSkill? SkillInfo { get; set; } = null;
        public FUserMetadata? MetadataInfo { get; set; } = null;
        public List<FUserLevelReward>? LevelReward { get; set; } = null;
        public List<DB_GearUnit>? gears { get; set; } = null;
        public List<DB_ItemUnit>? items { get; set; } = null;
    }

    public class DailyLoadInfo
    {
        public List<FUserRegisterQuest>? DailyQuset { get; set; } = null;
    }
}
