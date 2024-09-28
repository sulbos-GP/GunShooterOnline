
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterItemBase
    {
        //아이템 아이디
        public int item_id { get; set; } = 0;
            
        //아이템 코드
        public string code { get; set; } = string.Empty;
            
        //아이템 이름
        public string name { get; set; } = string.Empty;
            
        //아이템 무게
        public double weight { get; set; } = 0;
            
        //아이템 타입
        public string type { get; set; } = string.Empty;
            
        //아이템 설명
        public int description { get; set; } = 0;
            
        //아이템 가로 크기
        public int scale_x { get; set; } = 0;
            
        //아이템 세로 크기
        public int scale_y { get; set; } = 0;
            
        //아이템 구매 가격
        public int purchase_price { get; set; } = 0;
            
        //아이템 조회 시간
        public double inquiry_time { get; set; } = 0;
            
        //아이템 판매 가격
        public int sell_price { get; set; } = 0;
            
        //수량
        public int amount { get; set; } = 0;
            
        //아이템 아이콘 경로
        public string icon { get; set; } = string.Empty;
            
    }

}