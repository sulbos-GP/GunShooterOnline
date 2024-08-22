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

    public class DB_InventoryUnit()
    {
        public int item_id;         //아이템의 종류(해당 아이템을 DB에서 조회하기 위한 코드)
        public int grid_x;          // 아이템의 그리드 안 좌표상의 위치
        public int grid_y;          // 아이템의 그리드 안 좌표상의 위치
        public int rotation;        // 아이템의 회전코드(rotate * 90)
        public int stack_count;     // 아이템의 개수(소모품만 64개까지)

    }

    public class DB_ItemData
    {
        public readonly int item_id;
        public readonly string code;
        public readonly string name;
        public readonly double weight;
        public readonly string type;
        public readonly int description;
        public readonly int scale_x;
        public readonly int scale_y;
        public readonly int purchase_price;
        public readonly double inquiry_time;
        public readonly int sell_price;
        public readonly int stack_count;
        public readonly string prefab;
        public readonly string icon;
    }
}
