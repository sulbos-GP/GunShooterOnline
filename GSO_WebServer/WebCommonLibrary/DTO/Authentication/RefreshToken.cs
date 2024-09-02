
namespace WebCommonLibrary.DTO.Authentication
{
    public class RefreshTokenReq
    {
        public int uid { get; set; } = 0;
    }

    public class RefreshTokenRes : ErrorCodeDTO
    {
        public int uid { get; set; } = 0;
        public string access_token { get; set; } = string.Empty;

        public long expires_in { get; set; } = 0;

        public string scope { get; set; } = string.Empty;

        public string token_type { get; set; } = string.Empty;
    }
}
