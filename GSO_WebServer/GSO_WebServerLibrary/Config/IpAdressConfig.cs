using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Config
{
    public class EndPointConfig
    {
        public string Authorization { get; set; } = string.Empty;
        public string Center { get; set; } = string.Empty;
        public string Matchmaker { get; set; } = string.Empty;
        public string GameServerManager { get; set; } = string.Empty;
    }
}
