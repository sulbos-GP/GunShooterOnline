using GSO_WebServerLibrary;

namespace GsoWebServer.DTO
{
    public class ErrorCodeDTO
    {
        public WebErrorCode error { get; set; } = WebErrorCode.None;
    }
}
