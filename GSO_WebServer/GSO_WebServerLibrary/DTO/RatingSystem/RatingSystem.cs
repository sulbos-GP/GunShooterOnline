using GSO_WebServerLibrary;
using GSO_WebServerLibrary.DTO;
using GSO_WebServerLibrary.Models.GameDB;

namespace GSO_WebServerLibrary.Models.Statistic
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
