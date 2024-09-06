
using System.Collections.Generic;

namespace WebCommonLibrary.DTO.GameServer
{
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

    public class MatchPlayersReq
    {
        public string container_id { get; set; } = string.Empty;
    }

    public class MatchPlayersRes : ErrorCodeDTO
    {
        public List<int>? players { get; set; } = null;
    }
}
