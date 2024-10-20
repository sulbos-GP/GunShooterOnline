
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
          
    public class FUserRegisterQuest
    {
        //등록된 퀘스트 아이디
        public int register_quest_id { get; set; } = 0;
            
        //유저 아이디
        public int uid { get; set; } = 0;
            
        //퀘스트 아이디
        public int quest_id { get; set; } = 0;
            
        //퀘스트 진행도
        public int progress { get; set; } = 0;
            
        //퀘스트 완료 여부
        public bool completed { get; set; } = false;
            
        //퀘스트 수령 날짜
        public DateTime register_dt { get; set; } = DateTime.MinValue;
            
    }

}