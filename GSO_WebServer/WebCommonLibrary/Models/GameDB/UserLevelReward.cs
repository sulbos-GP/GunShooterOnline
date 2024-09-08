using System;
using System.Collections.Generic;
using System.Text;

namespace WebCommonLibrary.Models.GameDB
{
    public class UserLevelReward
    {
        public int reward_id { get; set; } = 0;
        public bool received { get; set; } = false;
        public DateTime received_dt { get; set; } = DateTime.MinValue;
    }
}
