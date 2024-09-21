
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterItemBase
    {		
        public int item_id { get; set; } = 0;		
        public string code { get; set; } = string.Empty;		
        public string name { get; set; } = string.Empty;		
        public double weight { get; set; } = 0;		
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

}