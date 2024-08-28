namespace GSO_WebServerLibrary.Models.GameDB
{
    /// <summary>
    /// 저장소 안에 데이터 정보
    /// </summary>
    public class DB_StorageUnit()
    {        
        public int grid_x                   { get; set; }
        public int grid_y                   { get; set; }
        public int rotation                 { get; set; }
        public int? unit_attributes_id      { get; set; }

        public DB_UnitAttributes attributes { get; set; } = new DB_UnitAttributes();
    }

    /// <summary>
    /// 아이템 속성
    /// </summary>
    public class DB_UnitAttributes
    {
        public int item_id                  { get; set; }
        public int durability               { get; set; }
        public int? unit_storage_id         { get; set; } = null;
        public int amount                   { get; set; }
    }

    /// <summary>
    /// 아이템 베이스 정보
    /// </summary>
    public class DB_ItemBase
    {
        public readonly int item_id = 0;
        public readonly string code = string.Empty;
        public readonly string name = string.Empty;
        public readonly double weight = 0.0;
        public readonly string type = string.Empty;
        public readonly int description = 0;
        public readonly int scale_x = 0;
        public readonly int scale_y = 0;
        public readonly int purchase_price = 0;
        public readonly double inquiry_time = 0.0;
        public readonly int sell_price = 0;
        public readonly int stack_count = 0;
        public readonly string prefab = string.Empty;
        public readonly string icon = string.Empty;
    }
}
