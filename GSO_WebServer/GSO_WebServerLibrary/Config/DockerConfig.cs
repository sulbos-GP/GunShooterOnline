using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Config
{
    public class DockerConfig
    {
        public string EndPoint { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
