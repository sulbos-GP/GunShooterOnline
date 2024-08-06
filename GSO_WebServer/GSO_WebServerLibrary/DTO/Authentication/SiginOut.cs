using GSO_WebServerLibrary;
using GSO_WebServerLibrary.DTO.DataLoad;
using System.ComponentModel.DataAnnotations;

namespace GSO_WebServerLibrary.DTO.Authentication
{
    public class SignOutReq
    {
        public string cause { get; set; } = string.Empty;
    }

    public class SignOutRes : ErrorCodeDTO
    {

    }
}
