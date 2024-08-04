using GSO_WebServerLibrary.Models.Match;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.DTO.Match
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
