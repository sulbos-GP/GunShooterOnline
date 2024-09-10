using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Models.MasterDB;

namespace WebCommonLibrary.DTO.Middleware
{
    public class UpgradeVersionRes : ErrorCodeDTO
    {
        public DB_Version? appVersion { get; set; } = null;
        public DB_Version? dataVersion { get; set; } = null;
    }
}
