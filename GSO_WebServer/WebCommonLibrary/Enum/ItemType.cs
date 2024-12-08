using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WebCommonLibrary.Enum
{
    public enum EItemType
    {
        [Description("none")]
        None,

        [Description("Weapone")]
        Weapone,

        [Description("Defensive")]
        Defensive,

        [Description("Bag")]
        Bag,

        [Description("Recovery")]
        Recovery,

        [Description("Bullet")]
        Bullet,

        [Description("Spoil")]
        Spoil
    }
}
