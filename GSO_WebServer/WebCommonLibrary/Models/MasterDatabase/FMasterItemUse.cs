
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
        public int item_id { get; set; } = 0;		
        public int energy { get; set; } = 0;		
        public double active_time { get; set; } = 0;		
        public double duration { get; set; } = 0;		
        public EEffect effect { get; set; } = EEffect.immediate;		
        public double cool_time { get; set; } = 0;
    }

}