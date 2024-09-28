
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterItemBackpack
    {
        //아이템 아이디
        public int item_id { get; set; } = 0;
            
        //가방 x크기
        public int total_scale_x { get; set; } = 0;
            
        //가방 y크기
        public int total_scale_y { get; set; } = 0;
            
        //가방 무게
        public double total_weight { get; set; } = 0;
            
    }

}