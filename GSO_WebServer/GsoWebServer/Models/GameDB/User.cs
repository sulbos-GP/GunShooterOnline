namespace GsoWebServer.Models.GameDB
{
    public class UserInfo
    {
        public int uid { get; set; } = 0;
        public string player_id { get; set; } = string.Empty;
        public string nickname { get; set; } = string.Empty;
        public string service { get; set; } = string.Empty;
        public DateTime create_dt { get; set; } = DateTime.MinValue;
        public DateTime recent_login_dt { get; set; } = DateTime.MinValue;
    }
}
