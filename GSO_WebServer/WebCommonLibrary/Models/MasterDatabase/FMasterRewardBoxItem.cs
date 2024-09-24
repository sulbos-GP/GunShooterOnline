
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterRewardBoxItem
    {
        //보상 박스 아이템 아이디
        public int reward_box_item_id { get; set; } = 0;
            
        //보상 박스 아이디
        public int reward_box_id { get; set; } = 0;
            
        //아이템 코드
        public string item_code { get; set; } = string.Empty;
            
        //아이템 x 위치
        public int x { get; set; } = 0;
            
        //아이템 y 위치
        public int y { get; set; } = 0;
            
        //아이템 회전
        public int rotation { get; set; } = 0;
            
        //아이템 수량
        public int amount { get; set; } = 0;
            
    }

}