namespace GsoWebServer.Models.GameDB
{
    /// <summary>
    /// 플레이어의 메타데이터 (토탈 게임에 따른 평균내기)
    /// </summary>
    public class UserMetadataInfo
    {
        public int total_games { get; set; } = 0;      //전체 게임 횟수
        public int kills { get; set; } = 0;            //사살 횟수
        public int deaths { get; set; } = 0;           //죽은 횟수
        public int damage { get; set; } = 0;           //사살 횟수
        public int farming { get; set; } = 0;          //파밍 횟수
        public int escape { get; set; } = 0;           //탈출 횟수
        public int survival_time { get; set; } = 0;    //생존 시간
    }
}
