using WebCommonLibrary.Error;

namespace WebCommonLibrary.DTO
{
    public class ErrorCodeDTO
    {
        public WebErrorCode error_code { get; set; } = WebErrorCode.None;

        public string error_description { get; set; } = string.Empty;
    }
}
