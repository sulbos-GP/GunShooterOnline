using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.Game
{
    /// <summary>
    /// 자신의 Uid를 통해 장비 불러오기
    /// </summary>
    public class LoadGearReq
    {

    }

    public class LoadGearRes : ErrorCodeDTO
    {
        public List<DB_GearUnit>? gears { get; set; } = null;
    }
}
