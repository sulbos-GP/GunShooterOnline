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

    public class DailyQuestReq
    {
        public int QuestId { get; set; } = 0;
    }

    public class DailyQuestRes : ErrorCodeDTO
    {
        public FUser? User { get; set; } = null;
        public List<FUserRegisterQuest>? DailyQuset { get; set; } = null;
    }
}
