
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
    public enum EStorage_type
    {
        None,		
        backpack,		
        cabinet,
    }
          
    public class FStorage
    {
        //저장소 아이디
        public int storage_id { get; set; } = 0;
            
        //유저 아이디
        public int? uid { get; set; } = null;
            
        //저장소 타입(가방/캐비넷)
        public EStorage_type storage_type { get; set; } = EStorage_type.backpack;
            
    }

}