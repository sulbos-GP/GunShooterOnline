namespace GsoWebServer.Models.Config
{
    public class DbConfig
    {
        public string GameDB { get; set; } = string.Empty;
        public string MasterDB { get; set; } = string.Empty;
        public string Redis { get; set; } = string.Empty;
    }

    public class GoogleConfig
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
    }
}
