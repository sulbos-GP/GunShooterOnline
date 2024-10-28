
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Models.GameDB;

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
        public ClientCredential? credential { get; set; } = null;

        public DataLoadUserInfo? userData { get; set; } = null;

        public DailyLoadInfo? DailyLoads { get; set; } = null;
    }

}
