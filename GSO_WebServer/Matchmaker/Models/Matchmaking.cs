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
        public long     match_start_time { get; set; } = 0;     //매칭 시작 시간
    }


}
