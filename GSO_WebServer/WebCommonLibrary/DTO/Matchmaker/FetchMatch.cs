using System.Collections.Generic;
using WebCommonLibrary.Models.Match;

namespace WebCommonLibrary.DTO.Matchmaker
{
    public class FetchMatchReq
    {
        public int playerCount { get; set; } = 0;
    }

    public class FetchMatchRes : ErrorCodeDTO
    {
        public MatchProfile? match_profile { get; set; } = null;
    }

    public class DispatchMatchPlayerReq
    {
        public MatchProfile? match_profile { get; set; } = null;
        public List<int>? players { get; set; } = null;
    }

    public class DispatchMatchPlayerRes : ErrorCodeDTO
    {

    }

    public class NotifyStartMatchReq
    {
        public List<int>? players { get; set; } = null;
        public MatchProfile? match_profile { get; set; } = null;
    }

    public class NotifyStartMatchRes : ErrorCodeDTO
    {
        
    }

}
