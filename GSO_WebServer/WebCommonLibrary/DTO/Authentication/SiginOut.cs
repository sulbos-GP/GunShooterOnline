
namespace WebCommonLibrary.DTO.Authentication
{
    public class SignOutReq
    {
        public string cause { get; set; } = string.Empty;
    }

    public class SignOutRes : ErrorCodeDTO
    {

    }
}
