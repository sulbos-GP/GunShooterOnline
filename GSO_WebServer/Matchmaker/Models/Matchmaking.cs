using GSO_WebServerLibrary;

namespace Matchmaker.Models
{
    public class JoinMatchMakingReq
    {
        public int      uid { get; set; } = 0;
        public string   access_token { get; set; } = string.Empty;
    }

    public class JoinMatchMakingRes
    {
        public WebErrorCode error { get; set; } = WebErrorCode.None;
    }

    public class CancleMatchMakingReq
    {
        public int      uid { get; set; } = 0;
        public string   access_token { get; set; } = string.Empty;
    }

    public class CancleMatchMakingRes
    {
        public WebErrorCode error { get; set; } = WebErrorCode.None;
    }

    /// <summary>
    /// 매칭 큐에 들어갈 정보 (Redis)
    /// </summary>
    public class MatchQueueInfo
    {
        public string   uid { get; set; } = string.Empty;
        public double   rating { get; set; } = 0.0;
        public double   deviation { get; set; } = 0.0;
        public string   state { get; set; } = string.Empty;
        public long     count { get; set; } = 0;
        public long     latency { get; set; } = 0;
        public long     match_start_time { get; set; } = 0;
    }

    public enum EPlayerState
    {
        Join = 0,
        Ready = 1,
        Cancle = 2,

    }

    /// <summary>
    /// 매칭 큐에 들어갈 정보 (Redis)
    /// </summary>
    public class TicketInfo
    {
        public string   key { get; set; } = string.Empty;
        public string   state { get; set; } = string.Empty;
        public double   skill_level { get; set; } = 0.0;
        public long     count { get; set; } = 0;
        public long     latency { get; set; } = 0;
        public long     match_start_time { get; set; } = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public class EnterMatchRoomInfoRes
    {
        public string end_point {  get; set; } = string.Empty;
        public string room_token { get; set; } = string.Empty;
    }

    /// <summary>
    /// 플레이어의 실력(스킬)
    /// </summary>
    public class PlayerSkill
    {
        public double rating { get; set; }          = 1500; //점수      defualt=1500
        public double deviation { get; set; }       = 350;  //표준 편차 defualt=350
        public double volatility { get; set; }      = 0.06; //변동성    defualt=0.06
    }


}
