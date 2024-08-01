using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    internal class Inventory
    {
        public int InvenId;
        public InvenDataInfo invenData;

        public List<Grid> instantGrid;
    }
}
