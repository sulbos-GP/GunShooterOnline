using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommonLibrary.Models.GameDB
{
    public class DB_Gear
    {
        public string part                          { get; set; } = string.Empty;
        public int unit_attributes_id               { get; set; } = 0;

        public DB_UnitAttributes attributes         { get; set; } = new DB_UnitAttributes();
    }
}
