using GSO_WebServerLibrary;
using GSO_WebServerLibrary.DTO;
using System.ComponentModel.DataAnnotations;

namespace GsoWebServer.DTO.Authentication
{
    public class SignOutReq
    {
        public string cause { get; set; } = string.Empty;
    }

    public class SignOutRes : ErrorCodeDTO
    {

    }
}
