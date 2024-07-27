using System.ComponentModel.DataAnnotations;

namespace GsoWebServer.DTO.Authentication
{
    public class GoogleAuthenticationReq
    {
    }

    public class GoogleAuthenticationRes : ErrorCodeDTO
    {

    }

    public class AuthenticationReq
    {
        [Required]
        public string user_id { get; set; } = string.Empty;

        [Required]
        public string service { get; set; } = string.Empty;
    }

    public class AuthenticationRes : ErrorCodeDTO
    {

    }

}
