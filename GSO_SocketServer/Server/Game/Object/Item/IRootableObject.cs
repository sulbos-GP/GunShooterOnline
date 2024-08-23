using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object.Item
{
    public interface IRootableObject
    {
        public void MoveItem(Storage sourceStorage, Storage destinationStorage);
    }
}
