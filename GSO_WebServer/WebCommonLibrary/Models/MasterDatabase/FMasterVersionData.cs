
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterVersionData
    {
        //데이터 버전
        public int major { get; set; } = 0;
            
        //신규 데이터
        public int minor { get; set; } = 0;
            
        //데이터 수정
        public int patch { get; set; } = 0;
            
    }

}