using WebCommonLibrary;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.User
{
    public class SetNicknameReq
    {
        public string new_nickname { get; set; } = string.Empty;
    }

    public class SetNicknameRes : ErrorCodeDTO
    {
        public UserInfo? user { get; set; } = null;
    }
}
