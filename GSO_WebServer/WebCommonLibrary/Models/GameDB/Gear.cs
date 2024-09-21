using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommonLibrary.Models.GameDB
{
    public class DB_GearUnit
    {
        public DB_Gear gear                         { get; set; } = new DB_Gear();

        public DB_UnitAttributes attributes         { get; set; } = new DB_UnitAttributes();
    }

    public class DB_Gear
    {
        public string part { get; set; } = string.Empty;
        public int unit_attributes_id { get; set; } = 0;
    }
}
