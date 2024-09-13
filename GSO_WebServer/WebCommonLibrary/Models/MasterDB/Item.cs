using System;
using System.Collections.Generic;
using System.Text;

namespace WebCommonLibrary.Models.MasterDB
{
    /// <summary>
    /// 아이템 베이스 정보
    /// </summary>
    public class DB_ItemBase
    {
        public int item_id { get; set; } = 0;
        public string code { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public double weight { get; set; } = 0.0;
        public string type { get; set; } = string.Empty;
        public int description { get; set; } = 0;
        public int scale_x { get; set; } = 0;
        public int scale_y { get; set; } = 0;
        public int purchase_price { get; set; } = 0;
        public double inquiry_time { get; set; } = 0;
        public int sell_price { get; set; } = 0;
        public int amount { get; set; } = 0;
        public string icon { get; set; } = string.Empty;
    }

    /// <summary>
    /// 가방 아이템 정보
    /// </summary>
    public class DB_ItemBackpack
    {
        public int item_id { get; set; } = 0;
        public int total_scale_x { get; set; } = 0;
        public int total_scale_y { get; set; } = 0;
        public double total_weight { get; set; } = 0.0;
    }

    /// <summary>
    /// 사용 아이템 정보
    /// </summary>
    public class DB_ItemUse
    {
        public int item_id { get; set; } = 0;
        public int energy { get; set; } = 0;
        public double active_time { get; set; } = 0.0;
        public double duration { get; set; } = 0.0;
        public string effect { get; set; } = string.Empty;
        public double cool_time { get; set; } = 0.0;
    }

    /// <summary>
    /// 무기 아이템 정보
    /// </summary>
    public class DB_ItemWeapon
    {
        public int item_id { get; set; } = 0;
        public int attack_range { get; set; } = 0;
        public int damage { get; set; } = 0;
        public int distance { get; set; } = 0;
        public int reload_round { get; set; } = 0;
        public double attack_speed { get; set; } = 0.0;
        public int reload_time { get; set; } = 0;
        public string bullet { get; set; } = string.Empty;

    }
}
