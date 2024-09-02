using System.Collections.Generic;
using WebCommonLibrary.DTO;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.Models.RatingSystem
{
    public class MatchOutComeReq
    {
        public string room_token { get; set; } = string.Empty;
        public Dictionary<int, MatchOutcomeInfo>? outcomes { get; set; } = null;
    }

    public class MatchOutComeRes : ErrorCodeDTO
    {
        
    }

}
