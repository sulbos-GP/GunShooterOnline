
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
          
    public class FUserSkill
    {
        //유저 아이디
        public int uid { get; set; } = 0;
            
        //레이팅
        public double rating { get; set; } = 1500;
            
        //표준 편차
        public double deviation { get; set; } = 350;
            
        //변동성
        public double volatility { get; set; } = 0.06;
            
    }

}