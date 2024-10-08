using System;
using System.Collections.Generic;
using System.Text;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace WebCommonLibrary.DTO.User
{
    public class LoadMetadataReq
    {

    }

    public class LoadMetadataRes : ErrorCodeDTO
    {
        public UserMetadataInfo? metadata { get; set; } = null;
    }
}
