namespace AuthenticationServer.Models
{
    public class GameUser
    {
        public long uid { get; set; }
        public string? id { get; set; }
        public string? service { get; set; }
    }

    public class NewGameUser
    {
        public string? id { get; set; }
        public string? service { get; set; }
    }
}
