
using System;
using System.Collections;
using System.Collections.Generic;

namespace WebCommonLibrary.Models.MasterDatabase
{
        
          
    public class FMasterItemWeapon
    {		
        public int item_id { get; set; } = 0;		
        public int attack_range { get; set; } = 0;		
        public int damage { get; set; } = 0;		
        public int distance { get; set; } = 0;		
        public int reload_round { get; set; } = 0;		
        public double attack_speed { get; set; } = 0;		
        public int reload_time { get; set; } = 0;		
        public string bullet { get; set; } = string.Empty;
    }

}