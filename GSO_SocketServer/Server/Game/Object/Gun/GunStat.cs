using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GunStat
    {
        public float range;       // 사거리.
        public float fireRate;    // 연사 속도. 값과 속도가 반비례
        public int ammo;          // 장탄수(보유장탄)
        public float accuracy;    // 발사 각도(정확도)
        public int damage;        // 데미지
        public GameObject bulletObj;
        public GunType gunType;
        public int reloadTime;
    }
}
