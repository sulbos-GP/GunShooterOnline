using GSO_WebServerLibrary;

namespace GsoWebServer.Models.Statistic
{
    public class MatchOutComeReq
    {
        public string room_token { get; set; } = string.Empty;
        public List<string>? user_id { get; set; } = null;
        public List<MatchOutcome>? user_match_outcome { get; set; } = null;
    }

    public class MatchOutComeRes
    {
        public WebErrorCode error { get; set; }
    }

     /// <summary>
     /// 매치에 대한 전체적인 기록
     /// </summary>
     public class MatchHistory
    {
        
        

    }

    /// <summary>
    /// 플레이어마다 매치 결과
    /// </summary>
    public class MatchOutcome()
    {
        public long kills { get; set; } = 0;
        public long death { get; set; } = 0;
        public long damage { get; set; } = 0;
        public long farming { get; set; } = 0;
        public long escape { get; set; } = 0;
        public long survival_time { get; set; } = 0;
    }

    /// <summary>
    /// 플레이어의 메타데이터 (토탈 게임에 따른 평균내기)
    /// </summary>
    public class PlayerMetadata
    {
        //public long total_games { get; set; } = 0;    //전체 게임 횟수
        public long kills { get; set; } = 0;            //사살 횟수
        public long deaths { get; set; } = 0;           //죽은 횟수
        public long damage { get; set; } = 0;           //사살 횟수
        public long farming { get; set; } = 0;          //파밍 횟수
        public long escape { get; set; } = 0;           //탈출 횟수
        public long survival_time { get; set; } = 0;    //생존 시간
    }

    /// <summary>
    /// 플레이어의 실력(스킬)
    /// </summary>
    public class PlayerSkill
    {
        public long uid { get; set; }
        public double rating { get; set; }
        public double deviation { get; set; }
        public double volatility { get; set; }
    }
}
