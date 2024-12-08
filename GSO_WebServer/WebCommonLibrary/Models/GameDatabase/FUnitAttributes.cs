
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
          
    public class FUnitAttributes
    {
        //유닛 속성 아이디
        public int unit_attributes_id { get; set; } = 0;
            
        //아이템 아이디
        public int item_id { get; set; } = 0;
            
        //아이템 내구도
        public int durability { get; set; } = 0;
            
        //저장소 아이디 (가방 전용)
        public int? unit_storage_id { get; set; } = null;
            
        //아이템 스택 카운터
        public int amount { get; set; } = 0;
            
    }

}