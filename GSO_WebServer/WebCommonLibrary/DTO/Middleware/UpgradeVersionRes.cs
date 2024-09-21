using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Models.MasterDatabase;
using WebCommonLibrary.Models.MasterDB;

namespace WebCommonLibrary.DTO.Middleware
{
    public class UpgradeVersionRes : ErrorCodeDTO
    {
        public FMasterVersionApp? appVersion { get; set; } = null;
        public FMasterVersionData? dataVersion { get; set; } = null;
    }
}
