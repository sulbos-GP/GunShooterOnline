
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
          
    public class FUserLevelReward
    {
        //레벨 보상 아이디
        public int reward_level_id { get; set; } = 0;
            
        //유저 아이디
        public int uid { get; set; } = 0;
            
        //보상 아이디
        public int reward_id { get; set; } = 0;
            
        //수령 확인
        public bool received { get; set; } = false;
            
        //수령 확인 날짜
        public DateTime? received_dt { get; set; } = null;
            
    }

}