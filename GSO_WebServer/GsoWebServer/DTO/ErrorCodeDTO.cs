using GSO_WebServerLibrary;
using System.ComponentModel.DataAnnotations;

namespace GsoWebServer.DTO
{
    public class ErrorCodeDTO
    {
        [Required]
        public WebErrorCode error_code { get; set; } = WebErrorCode.None;

        [Required]
        public string error_description { get; set; } = string.Empty;
    }
}
