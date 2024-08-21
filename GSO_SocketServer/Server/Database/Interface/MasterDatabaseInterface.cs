using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Interface
{
    public interface IMasterDatabase
    {
        public Task<ItemInfo> GetItemInfo(int item_id);
    }
}
