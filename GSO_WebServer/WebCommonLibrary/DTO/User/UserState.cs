using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Enum;

namespace WebCommonLibrary.DTO.User
{
    public class UpdateUserStateReq
    {
        public EUserState state { get; set; } = EUserState.None;
    }

    public class UpdateUserStateRes : ErrorCodeDTO
    {

    }
}
