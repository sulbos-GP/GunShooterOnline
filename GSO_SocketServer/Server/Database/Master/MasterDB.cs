using Server.Database.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Master
{
    public partial class MasterDB : MySQL, IMasterDatabase
    {

        public MasterDB() : base()
        {
            
        }

    }
}
