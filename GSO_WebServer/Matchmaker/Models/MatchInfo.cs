namespace Matchmaker.Models
{
    /// <summary>
    /// 매치 방 정보(Redis)
    /// </summary>
    public class MatchInfo
    {
        public string   token { get; set; } = string.Empty;
        public string   host { get; set; } = string.Empty;
        public short    port { get; set; } = 0;
    }
}
