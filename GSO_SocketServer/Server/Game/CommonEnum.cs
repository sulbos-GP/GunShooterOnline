using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public enum eTABLE_TYPE
    {
        None,
        TestItem
    }

    public enum eStat
    {
        None,
        Health,
        Armor
    }

    public enum GunState
    {
        Shootable,
        Empty,
        Reloading
    }

    public enum GunType
    {
        Pistol,
        AssultRifle,
        ShotGun,
        Sniper
    }
}
