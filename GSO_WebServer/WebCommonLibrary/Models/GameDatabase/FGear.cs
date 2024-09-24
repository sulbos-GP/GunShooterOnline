
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
    public enum EPart
    {
        None,		
        main_weapon,		
        sub_weapon,		
        armor,		
        backpack,		
        pocket_first,		
        pocket_second,		
        pocket_third,
    }
          
    public class FGear
    {
        //장비 아이디
        public int gear_id { get; set; } = 0;
            
        //유저 아이디
        public int uid { get; set; } = 0;
            
        //슬롯 타입
        public EPart part { get; set; } = EPart.main_weapon;
            
        //아이템 속성
        public int unit_attributes_id { get; set; } = 0;
            
    }

}