
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
}
