using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class BulletStat
    {
        public float speed;
        public GunType bulletType;

        public BulletStat(float speed, GameObject bulletObj, GunType bulletType)
        {
            this.speed = speed;
            this.bulletType = bulletType;
        }
    }
}
