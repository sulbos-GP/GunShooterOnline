
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterVersionApp
    {
        //앱 버전
        public int major { get; set; } = 0;
            
        //신규 기능
        public int minor { get; set; } = 0;
            
        //버그 수정
        public int patch { get; set; } = 0;
            
    }

}