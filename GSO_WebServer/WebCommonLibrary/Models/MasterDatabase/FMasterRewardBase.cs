
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterRewardBase
    {
        //보상 아이디
        public int reward_id { get; set; } = 0;
            
        //돈
        public int money { get; set; } = 0;
            
        //티켓
        public int ticket { get; set; } = 0;
            
        //가챠
        public int gacha { get; set; } = 0;
            
        //경험치
        public int experience { get; set; } = 0;
            
        //보상 박스 아이디
        public int? reward_box_id { get; set; } = null;
            
    }

}