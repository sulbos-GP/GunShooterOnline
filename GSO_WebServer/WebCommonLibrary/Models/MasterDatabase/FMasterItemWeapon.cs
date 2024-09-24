
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterItemWeapon
    {
        //아이템 아이디
        public int item_id { get; set; } = 0;
            
        //공격 범위
        public int attack_range { get; set; } = 0;
            
        //공격 데미지
        public int damage { get; set; } = 0;
            
        //공격 거리
        public int distance { get; set; } = 0;
            
        //재장전 수
        public int reload_round { get; set; } = 0;
            
        //공격 속도
        public double attack_speed { get; set; } = 0;
            
        //재장전 시간
        public int reload_time { get; set; } = 0;
            
        //사용 탄환
        public string bullet { get; set; } = string.Empty;
            
    }

}