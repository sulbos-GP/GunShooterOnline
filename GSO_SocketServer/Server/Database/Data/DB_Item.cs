using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Data
{

    public class DB_StorageUnit()
    {
        public int grid_x;                  
        public int grid_y;                  
        public int rotation;                
        public int unit_attributes_id;     

        public DB_UnitAttributes attributes = new DB_UnitAttributes();
    }

    /// <summary>
    /// 아이템 속성
    /// </summary>
    public class DB_UnitAttributes
    {
        public int item_id;
        public int durability;
        public int? unit_storage_id = null;
        public int amount;
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
        public readonly string icon;
    }

    public class DB_ItemBackpack
    {
        public readonly int item_id;
        public readonly int total_scale_x;
        public readonly int total_scale_y;
        public readonly double total_weight;
    }
}
