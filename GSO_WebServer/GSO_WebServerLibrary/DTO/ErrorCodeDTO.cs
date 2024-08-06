using GSO_WebServerLibrary.Error;

namespace GSO_WebServerLibrary.DTO
{
    public class ErrorCodeDTO
    {
        public WebErrorCode error_code { get; set; } = WebErrorCode.None;

        public string error_description { get; set; } = string.Empty;
    }
}
