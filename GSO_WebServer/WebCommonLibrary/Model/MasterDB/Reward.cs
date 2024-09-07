using System;
using System.Collections.Generic;
using System.Text;

namespace WebCommonLibrary.Model.MasterDB
{
    public class RewardBase
    {
        public int reward_id { get; set; } = 0;
        public int money { get; set; } = 0;
        public int ticket { get; set; } = 0;
        public int gacha { get; set; } = 0;
        public int? reward_box_id { get; set; } = null;
    }

    public class RewardBox
    {
        public int reward_box_id { get; set; } = 0;
    }

    public class LevelReward
    {
        public int reward_level_id { get; set; } = 0;
        public int reward_id { get; set; } = 0;
        public int level { get; set; } = 0;
        public int experience { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string icon { get; set; } = string.Empty;
    }
}
