
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GSO_WebServerLibrary.DTO
{
    public class HeaderDTO
    {
        [Required]
        [FromHeader(Name = "uid")]
        public int uid { get; set; } = 0;

        [Required]
        [FromHeader(Name = "access_token")]
        public string access_token { get; set; } = string.Empty;
    }
}
