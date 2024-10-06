using System.Collections.Generic;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.Performance
{
    public class PlayerStatsReq
    {
        public string room_token { get; set; } = string.Empty;
        public int uid { get; set; } = 0;
        public MatchOutcome? outcome { get; set; } = null;
    }

    public class PlayerStatsRes : ErrorCodeDTO
    {
        
    }

}
