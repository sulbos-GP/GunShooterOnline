using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Config
{
    public class DatabaseConfig
    {
        public string GameDB { get; set; } = string.Empty;
        public string MasterDB { get; set; } = string.Empty;
        public string Redis { get; set; } = string.Empty;
    }
}
