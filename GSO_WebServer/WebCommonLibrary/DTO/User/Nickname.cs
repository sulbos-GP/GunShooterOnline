using WebCommonLibrary;

namespace WebCommonLibrary.DTO.User
{
    public class SetNicknameReq
    {
        public string new_nickname { get; set; } = string.Empty;
    }

    public class SetNicknameRes : ErrorCodeDTO
    {
        public string nickname { get; set; } = string.Empty;
    }
}
