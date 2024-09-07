using System.Collections.Generic;
using WebCommonLibrary.Models.Match;

namespace WebCommonLibrary.DTO.Matchmaker
{
    public class FetchMatchReq
    {
        public List<int>? players { get; set; } = null;
    }

    public class FetchMatchRes : ErrorCodeDTO
    {
        public MatchProfile? match_profile { get; set; } = null;
    }

    public class DispatchMatchPlayerReq
    {

    }

    public class DispatchMatchPlayerRes : ErrorCodeDTO
    {

    }

}
