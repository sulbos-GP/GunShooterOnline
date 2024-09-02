namespace WebCommonLibrary.Models.GameDB
{

    /// <summary>
    /// 플레이어의 실력(스킬)
    /// </summary>
    public class UserSkillInfo
    {
        public double rating { get; set; }
        public double deviation { get; set; }
        public double volatility { get; set; }
    }
}
