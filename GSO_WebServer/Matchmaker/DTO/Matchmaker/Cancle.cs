using GSO_WebServerLibrary;
using GSO_WebServerLibrary.DTO;

namespace Matchmaker.DTO.Matchmaker
{
    public class CancleMatchReq
    {
        public string world { get; set; } = string.Empty;
        public string region { get; set; } = string.Empty;
    }

    public class CancleMatchRes : ErrorCodeDTO
    {
    }
}
