
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
          
    public class FStorageUnit
    {
        //저장소 슬롯 아이디
        public int storage_unit_id { get; set; } = 0;
            
        //저장소 아이디 (가방 ID 또는 캐비넷 ID)
        public int storage_id { get; set; } = 0;
            
        //아이템 x위치
        public int grid_x { get; set; } = 0;
            
        //아이템 y위치
        public int grid_y { get; set; } = 0;
            
        //아이템 회전
        public int rotation { get; set; } = 0;
            
        //아이템 속성 아이디
        public int unit_attributes_id { get; set; } = 0;
            
    }

}