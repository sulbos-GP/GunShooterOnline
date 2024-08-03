using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Models.Match
{
    public class MatchProfile
    {
        public string   container_id { get; set; } = string.Empty;
        public string   world { get; set; } = string.Empty;
        public string   host_ip { get; set; } = string.Empty;
        public int      container_port { get; set; } = 0;
        public int      host_port { get; set; } = 0;
    }
}
