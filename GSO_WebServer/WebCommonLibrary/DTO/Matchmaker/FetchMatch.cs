using WebCommonLibrary.Models.Match;

namespace WebCommonLibrary.DTO.Matchmaker
{
    public class FetchMatchReq
    {

    }

    public class FetchMatchRes : ErrorCodeDTO
    {
        public MatchProfile? match_profile { get; set; } = null;
    }

    public class RequestReadyMatchReq
    {
        public string container_id { get; set; } = string.Empty;
    }

    public class RequestReadyMatchRes : ErrorCodeDTO
    {
    }

    public class ShutdownMatchReq
    {
        public string container_id { get; set; } = string.Empty;
    }

    public class ShutdownMatchRes : ErrorCodeDTO
    {
    }
}
