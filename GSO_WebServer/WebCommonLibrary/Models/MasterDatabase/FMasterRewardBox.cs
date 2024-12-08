
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterRewardBox
    {
        //보상 박스 아이디
        public int reward_box_id { get; set; } = 0;
            
        //박스 이름
        public string name { get; set; } = "Box";
            
        //박스 x크기
        public int scale_x { get; set; } = 0;
            
        //박스 x크기
        public int scale_y { get; set; } = 0;
            
    }

}