using Google.Protobuf.WellKnownTypes;
using Server.Database.Data;
using Server.Database.Handler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object.Gear
{
    public class Gear : GameObject
    {
        private Dictionary<string, ItemObject> parts = new Dictionary<string, ItemObject>();
        private Player owner;

        public Gear(Player owner) 
        {
            this.owner = owner;
            LoadGear().Wait();
        }

        public ItemObject GetPart(EGearPart part)
        {
            ItemObject item = new ItemObject();
            switch (part)
            {
                case EGearPart.MainWeapon:      parts.TryGetValue("main_weapon", out item);     break;
                case EGearPart.SubWeapon:       parts.TryGetValue("sub_weapon", out item);      break;
                case EGearPart.Armor:           parts.TryGetValue("armor", out item);           break;
                case EGearPart.Backpack:        parts.TryGetValue("backpack", out item);        break;
                case EGearPart.PocketFirst:     parts.TryGetValue("pocket_first", out item);    break;
                case EGearPart.PocketSecond:    parts.TryGetValue("pocket_second", out item);   break;
                case EGearPart.PocketThird:     parts.TryGetValue("pocket_third", out item);    break;
            }

            return item;
        }

        public async Task LoadGear()
        {
            try
            {
                IEnumerable<DB_Gear> gears = await DatabaseHandler.GameDB.LoadGear(owner.uid);

                if (gears == null)
                {
                    return;
                }

                foreach (DB_Gear gear in gears)
                {
                    ItemObject item = new ItemObject(owner.Id, 0, 0, 0, gear.attributes);
                    if(false == parts.TryAdd(gear.part, item))
                    {
                        throw new Exception($"장비의 파트({gear.part})가 중복되어 있음");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"[LoadGear] : {e.Message.ToString()}");
            }
        }

    }
}
