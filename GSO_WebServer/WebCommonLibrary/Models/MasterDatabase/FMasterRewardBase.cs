
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterRewardBase
    {		
        public int reward_id { get; set; } = 0;		
        public int money { get; set; } = 0;		
        public int ticket { get; set; } = 0;		
        public int gacha { get; set; } = 0;		
        public int experience { get; set; } = 0;		
        public int? reward_box_id { get; set; } = null;
    }

}