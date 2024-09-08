namespace WebCommonLibrary.Models.MemoryDB
{
    public class AuthUserDataInfo
    {
        public string uid { get; set; } = string.Empty;
        public string user_id { get; set; } = string.Empty;
        public string access_token { get; set; } = string.Empty;
    }

    public class RefreshDataInfo
    {
        public string uid { get; set; } = string.Empty;
        public string user_id { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
    }
}
