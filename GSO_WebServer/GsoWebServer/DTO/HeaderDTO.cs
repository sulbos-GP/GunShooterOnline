using Microsoft.AspNetCore.Mvc;

namespace GsoWebServer.DTO
{
    public class HeaderDTO
    {
        [FromHeader]
        public int uid { get; set; }
    }
}
