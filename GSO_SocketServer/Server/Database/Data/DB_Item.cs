using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Data
{


    public class DB_StorageUnit()
    {
        public int item_id;             // 아이템의 종류(해당 아이템을 DB에서 조회하기 위한 코드)
        public int grid_x;              // 아이템의 그리드 안 좌표상의 위치
        public int grid_y;              // 아이템의 그리드 안 좌표상의 위치
        public int rotation;            // 아이템의 회전코드(rotate * 90)
        public int stack_count;         // 아이템의 개수(소모품만 64개까지
        public int unit_attributes_id;  // 아이템의 속성 아이디
    }

    /// <summary>
    /// 아이템 속성
    /// </summary>
    public class DB_UnitAttributes
    {
        public int durability;
        public int storage_id;
    }

    public class DB_ItemBase
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
