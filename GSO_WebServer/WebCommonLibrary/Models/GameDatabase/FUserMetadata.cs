
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
          
    public class FUserMetadata
    {
        //유저 아이디
        public int uid { get; set; } = 0;
            
        //게임
        public int total_games { get; set; } = 0;
            
        //킬
        public int kills { get; set; } = 0;
            
        //데스
        public int deaths { get; set; } = 0;
            
        //대미지
        public int damage { get; set; } = 0;
            
        //파밍
        public int farming { get; set; } = 0;
            
        //탈출
        public int escape { get; set; } = 0;
            
        //생존
        public int survival_time { get; set; } = 0;
            
    }

}