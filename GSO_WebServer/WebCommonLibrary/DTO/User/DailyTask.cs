using System.Collections.Generic;
using WebCommonLibrary;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.User
{
    public class DailyTaskReq
    {
        
    }

    public class DailyTaskRes : ErrorCodeDTO
    {
        public DailyLoadInfo? DailyLoads { get; set; } = null;
    }
}
