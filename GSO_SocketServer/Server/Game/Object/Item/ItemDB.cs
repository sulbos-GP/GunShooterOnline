using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Gear()
    {
        public int item_id;         //아이템의 종류(해당 아이템을 DB에서 조회하기 위한 코드)
        public int gear_type;       //장비의 종류
    }

    //임시 가방 데이터
    public class Backpack()
    {
        public int scale_x = 6;
        public int scale_y = 7;
        public double limit_weight = 20.0;
    }





    public class DB_BackpackData
    {
        public readonly string code;
        public readonly int total_x;
        public readonly int total_y;
        public readonly double weight;
    }
}
