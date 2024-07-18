namespace GsoWebServer.Models.GameDB
{
    public class MatchOutcomeInfo
    {
        public long kills { get; set; } = 0;
        public long death { get; set; } = 0;
        public long damage { get; set; } = 0;
        public long farming { get; set; } = 0;
        public long escape { get; set; } = 0;
        public long survival_time { get; set; } = 0;
    }
}
