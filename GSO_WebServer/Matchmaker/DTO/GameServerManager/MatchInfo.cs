namespace Matchmaker.DTO.GameServerManager
{
    public class MatchInfo
    {
        public string token { get; set; } = string.Empty;
        public string ip { get; set; } = string.Empty;
        public short port { get; set; } = 0;
    }
}
