using GSO_WebServerLibrary;

namespace Matchmaker.Models
{


    /// <summary>
    /// 플레이어 스캔 되었을때 정보
    /// </summary>
    public class PlayerInfo
    {
        public double       rating { get; set; } = 0.0;
        public TicketInfo?  ticket { get; set; } = null;
    }

    /// <summary>
    /// 매칭 큐에 들어갈 정보 (Redis)
    /// </summary>
    public class TicketInfo
    {
        public string   client_id { get; set; } = string.Empty; //클라이언트 아이디
        public string   world { get; set; } = string.Empty;     //맵
        public string   region { get; set; } = string.Empty;    //리전
        public long     latency { get; set; } = 0;              //레이턴시
        public int      match_start_time { get; set; } = 0;     //매칭 시작 시간
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
