using GSO_WebServerLibrary;
using GSO_WebServerLibrary.DTO.DataLoad;
using System.ComponentModel.DataAnnotations;

namespace GSO_WebServerLibrary.DTO.Authentication
{
    public class SignInReq
    {
        [Required]
        public string user_id { get; set; } = string.Empty;

        [Required]
        public string server_code { get; set; } = string.Empty;

        [Required]
        public string service { get; set; } = string.Empty;
    }

    public class SignInRes : ErrorCodeDTO
    {
        [Required]
        public int          uid { get; set; } = 0;

        [Required]
        public string       access_token { get; set; } = string.Empty;

        public long         expires_in { get; set; } = 0;

        public string       scope { get; set; } = string.Empty;

        public string       token_type { get; set; } = string.Empty;

        public DataLoadUserInfo? userData { get; set; } = null;
    }

}
