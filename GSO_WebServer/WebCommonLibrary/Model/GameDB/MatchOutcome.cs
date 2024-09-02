namespace WebCommonLibrary.Models.GameDB
{
    /// <summary>
    /// 플레이어마다 매치 결과
    /// </summary>
    public class MatchOutcomeInfo
    {
        public int kills { get; set; } = 0;
        public int death { get; set; } = 0;
        public int damage { get; set; } = 0;
        public int farming { get; set; } = 0;
        public int escape { get; set; } = 0;
        public int survival_time { get; set; } = 0;
    }

    /// <summary>
    /// 매치에 대한 전체적인 기록
    /// </summary>
    public class MatchHistory
    {

    }
}
