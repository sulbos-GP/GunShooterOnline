namespace AuthenticationServer.Models
{
    public class DbConfig
    {
        public string? GameDB { get; set; }
        public string? Redis { get; set; }
    }

    public class GoogleConfig
    {
        public string? ApplicationName { get; set; }
        public string? UserId { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? RedirectUri { get; set; }
    }
}
