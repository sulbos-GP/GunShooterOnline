using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object.Gear
{
    public enum EGearPart
    {
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

    internal class GearDB
    {
    }
}
