using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.User
{
    public class UpdateTicketReq
    {

    }

    public class UpdateTicketRes : ErrorCodeDTO
    {
        public FUser? User { get; set; } = null;
        public int RemainingTime { get; set; } = 0;
    }
}
