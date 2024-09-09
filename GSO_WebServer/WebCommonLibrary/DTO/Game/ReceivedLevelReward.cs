using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.Game
{
    public class ReceivedLevelRewardReq
    {
        public int level { get; set; } = 0;
    }

    public class ReceivedLevelRewardRes : ErrorCodeDTO
    {
        public List<UserLevelReward>? LevelReward { get; set; } = null;
    }
}
