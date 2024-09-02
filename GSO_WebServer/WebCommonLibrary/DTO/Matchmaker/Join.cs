
namespace WebCommonLibrary.DTO.Matchmaker
{
    public class JoinMatchReq
    {
        public string world { get; set; } = string.Empty;
        public string region { get; set; } = string.Empty;
    }

    public class JoinMatchRes : ErrorCodeDTO
    {

    }
}
