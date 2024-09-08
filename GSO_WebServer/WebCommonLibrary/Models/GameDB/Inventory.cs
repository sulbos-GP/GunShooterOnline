namespace WebCommonLibrary.Models.GameDB
{
    /// <summary>
    /// 저장소 안에 있는 아이템 데이터와 속성
    /// </summary>
    public class DB_ItemUnit
    {
        public DB_StorageUnit? storage { get; set; } = null;
        public DB_UnitAttributes? attributes { get; set; } = null;
    }

    /// <summary>
    /// 저장소 안에 데이터 정보
    /// </summary>
    public class DB_StorageUnit
    {
        public int grid_x { get; set; } = 0;
        public int grid_y { get; set; } = 0;
        public int rotation { get; set; } = 0;
        public int unit_attributes_id { get; set; } = 0;
    }

    /// <summary>
    /// 아이템 속성
    /// </summary>
    public class DB_UnitAttributes
    {
        public int item_id { get; set; } = 0;
        public int durability { get; set; } = 0;
        public int? unit_storage_id { get; set; } = null;
        public int amount { get; set; } = 0;
    }
}
