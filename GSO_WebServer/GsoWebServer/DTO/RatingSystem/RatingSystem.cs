using GSO_WebServerLibrary;
using GsoWebServer.DTO;
using GsoWebServer.Models.GameDB;

namespace GsoWebServer.Models.Statistic
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
