using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.Game
{
    public class ReceivedLevelRewardReq
    {
        public int level { get; set; } = 0;
    }

    public class ReceivedLevelRewardRes : ErrorCodeDTO
    {
        public List<FUserLevelReward>? LevelReward { get; set; } = null;
        public FUser? user { get; set; } = null;
    }
}
