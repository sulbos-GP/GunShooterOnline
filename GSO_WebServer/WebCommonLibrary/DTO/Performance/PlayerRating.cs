using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.Performance
{
    public class PlayerRatingReq
    {
        public string room_token { get; set; } = string.Empty;
        public Dictionary<int, MatchOutcome>? outcomes { get; set; } = null;
    }

    public class PlayerRatingRes : ErrorCodeDTO
    {

    }

}
