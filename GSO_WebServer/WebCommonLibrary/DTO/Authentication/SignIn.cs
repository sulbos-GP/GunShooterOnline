
using WebCommonLibrary.DTO.DataLoad;

namespace WebCommonLibrary.DTO.Authentication
{
    public class SignInReq
    {
        public string user_id { get; set; } = string.Empty;

        public string server_code { get; set; } = string.Empty;

        public string service { get; set; } = string.Empty;
    }

    public class SignInRes : ErrorCodeDTO
    {
        public int          uid { get; set; } = 0;

        public string       access_token { get; set; } = string.Empty;

        public long         expires_in { get; set; } = 0;

        public string       scope { get; set; } = string.Empty;

        public string       token_type { get; set; } = string.Empty;

        public DataLoadUserInfo? userData { get; set; } = null;
    }

}
