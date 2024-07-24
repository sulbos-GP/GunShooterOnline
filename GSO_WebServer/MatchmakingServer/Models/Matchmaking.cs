using GSO_WebServerLibrary;

namespace Matchmaker.Models
{
    public class JoinMatchMakingReq
    {
        public string user_id { get; set; } = string.Empty;
        public string access_token { get; set; } = string.Empty;
    }

    public class JoinMatchMakingRes
    {
        public WebErrorCode error { get; set; } = WebErrorCode.None;
    }

    public class CancleMatchMakingReq
    {
        public string user_id { get; set; } = string.Empty;
        public string access_token { get; set; } = string.Empty;
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
        public long     time { get; set; } = 0;
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
