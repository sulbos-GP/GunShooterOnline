
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterRewardLevel
    {
        //보상 정보
        public int reward_id { get; set; } = 0;
            
        //레벨
        public int level { get; set; } = 0;
            
        //보상 이름
        public string name { get; set; } = string.Empty;
            
        //보상 아이콘
        public string icon { get; set; } = string.Empty;
            
    }

}