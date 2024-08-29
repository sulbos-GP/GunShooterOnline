using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Data
{
    public enum EGearPart
    {
        [Description("none")]
        None,

        [Description("main_weapon")]
        MainWeapon,

        [Description("sub_weapon")]
        SubWeapon,

        [Description("armor")]
        Armor,

        [Description("backpack")]
        Backpack,

        [Description("pocket_first")]
        PocketFirst,

        [Description("pocket_second")]
        PocketSecond,

        [Description("pocket_third")]
        PocketThird
    }

    public class DB_Gear
    {
        public string part;
        public int unit_attributes_id;

        public DB_UnitAttributes attributes = new DB_UnitAttributes();
    }
}
