﻿using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.Game
{

    /// <summary>
    /// 저장소 아이디를 통해 저장소 내부의 아이템을 불러오기 위한 요청
    /// </summary>
    public class LoadStorageReq
    {
        
    }

    public class LoadStorageRes : ErrorCodeDTO
    {
        public List<DB_GearUnit>? gears { get; set; } = null;
        public List<DB_ItemUnit>? items { get; set; } = null;
    }

    public class ResetStorageReq
    {
        
    }

    public class ResetStorageRes : ErrorCodeDTO
    {
        public List<DB_GearUnit>? gears { get; set; } = null;
        public List<DB_ItemUnit>? items { get; set; } = null;
    }

}
