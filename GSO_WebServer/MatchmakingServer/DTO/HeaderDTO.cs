using Microsoft.AspNetCore.Mvc;

namespace Matchmaker.DTO
{
    public class HeaderDTO
    {
        [FromHeader(Name = "uid")]
        public int uid { get; set; } = 0;

        [FromHeader(Name = "access_token")]
        public string access_token { get; set; } = string.Empty;
    }
}
