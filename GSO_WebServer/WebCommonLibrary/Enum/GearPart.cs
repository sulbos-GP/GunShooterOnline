using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WebCommonLibrary.Enum
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
}
