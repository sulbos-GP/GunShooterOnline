
namespace WebCommonLibrary.DTO.Authentication
{
    public class GoogleAuthenticationReq
    {
    }

    public class GoogleAuthenticationRes : ErrorCodeDTO
    {

    }

    public class AuthenticationReq
    {
        public string user_id { get; set; } = string.Empty;

        public string service { get; set; } = string.Empty;
    }

    public class AuthenticationRes : ErrorCodeDTO
    {

    }

}
