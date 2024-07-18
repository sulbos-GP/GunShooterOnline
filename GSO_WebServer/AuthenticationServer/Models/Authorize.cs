namespace AuthenticationServer.Models
{
    public class AccessTokenReq
    {
        public string? service {  get; set; }
        public string? user_id { get; set; }
        public string? server_code { get; set; }
    }

    public class AccessTokenRes
    {
        public ErrorCode    error { get; set; }
        public long         uid { get; set; }
        public string?      access_token { get; set; }
        public long?        expires_in { get; set; }
        public string?      scope { get; set; }
        public string?      token_type { get; set; }
    }
}
