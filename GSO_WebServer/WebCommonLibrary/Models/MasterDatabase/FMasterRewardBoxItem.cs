
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterRewardBoxItem
    {		
        public int reward_box_item_id { get; set; } = 0;		
        public int reward_box_id { get; set; } = 0;		
        public string item_code { get; set; } = string.Empty;		
        public int x { get; set; } = 0;		
        public int y { get; set; } = 0;		
        public int rotation { get; set; } = 0;		
        public int amount { get; set; } = 0;
    }

}