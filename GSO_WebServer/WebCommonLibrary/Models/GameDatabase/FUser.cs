
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.GameDatabase
{
        
          
    public class FUser
    {
        //유저 아이디
        public int uid { get; set; } = 0;
            
        //플레이어 아이디
        public string player_id { get; set; } = string.Empty;
            
        //닉네임
        public string? nickname { get; set; } = null;
            
        //
        public int experience { get; set; } = 100;
            
        //
        public int money { get; set; } = 0;
            
        //
        public int ticket { get; set; } = 0;
            
        //
        public int gacha { get; set; } = 0;
            
        //서비스
        public string service { get; set; } = string.Empty;
            
        //갱신 토큰
        public string? refresh_token { get; set; } = null;
            
        //생성 일시
        public DateTime create_dt { get; set; } = DateTime.MinValue;
            
        //최근 로그인 일시
        public DateTime recent_login_dt { get; set; } = DateTime.MinValue;
            
    }

}