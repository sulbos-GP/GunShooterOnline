
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
    public enum EEffect
    {
        None,		
        immediate,		
        buff,
    }
          
    public class FMasterItemUse
    {
        //아이템 아이디
        public int item_id { get; set; } = 0;
            
        //회복 아이템의 회복량
        public int energy { get; set; } = 0;
            
        //효과발동 시간
        public double active_time { get; set; } = 0;
            
        //회복 아이템의 지속시간
        public double duration { get; set; } = 0;
            
        //해당 아이템의 효과 타입
        public EEffect effect { get; set; } = EEffect.immediate;
            
        //재사용 대기시간
        public double cool_time { get; set; } = 0;
            
    }

}