using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommonLibrary.DTO.Middleware
{
    public class VersionInfo
    {
        public int major { get; set; } = 0;
        public int minor { get; set; } = 0;
        public int patch { get; set; } = 0;
    }
}
