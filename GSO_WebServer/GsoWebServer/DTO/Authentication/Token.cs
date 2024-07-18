using GSO_WebServerLibrary;
using System.ComponentModel.DataAnnotations;

namespace GsoWebServer.DTO.Authentication
{
    public class RefreshTokenReq
    {
        [Required]
        public long uid { get; set; } = 0;

        [Required]
        public string access_token { get; set; } = string.Empty;

        [Required]
        public string refresh_token { get; set; } = string.Empty;
    }

    public class RefreshTokenRes
    {
        [Required]
        public WebErrorCode error { get; set; } = WebErrorCode.None;

        [Required]
        public int uid { get; set; } = 0;

        [Required]
        public string access_token { get; set; } = string.Empty;

        public long expires_in { get; set; } = 0;

        public string scope { get; set; } = string.Empty;

        public string token_type { get; set; } = string.Empty;
    }
}
