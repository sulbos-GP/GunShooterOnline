using GSO_WebServerLibrary;
using System.ComponentModel.DataAnnotations;

namespace GsoWebServer.DTO.User
{
    public class SetNicknameReq
    {
        [Required]
        public string new_nickname { get; set; } = string.Empty;
    }

    public class SetNicknameRes : ErrorCodeDTO
    {
        public string nickname { get; set; } = string.Empty;
    }
}
