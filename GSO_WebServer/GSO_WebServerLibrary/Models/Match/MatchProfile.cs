using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Models.Match
{
    public class MatchProfile
    {
        public string match_id { get; set; } = string.Empty;
        public string host { get; set; } = string.Empty;
        public short port { get; set; } = 0;
    }
}
