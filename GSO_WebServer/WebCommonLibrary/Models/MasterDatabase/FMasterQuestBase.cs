
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterQuestBase
    {
        //퀘스트 아이디
        public int quest_id { get; set; } = 0;
            
        //퀘스트 타입
        public string type { get; set; } = string.Empty;
            
        //퀘스트 카테고리
        public string category { get; set; } = string.Empty;
            
        //퀘스트 제목
        public string title { get; set; } = string.Empty;
            
        //퀘스트 목표
        public int target { get; set; } = 0;
            
        //퀘스트 태그
        public string tag { get; set; } = string.Empty;
            
        //보상 아이디
        public int reward_id { get; set; } = 0;
            
        //연계 퀘스트
        public int? next_quest_id { get; set; } = null;
            
        //선행 조건
        public int? start_condition_id { get; set; } = null;
            
    }

}