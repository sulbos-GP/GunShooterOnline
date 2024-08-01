using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GridData
    {
        public Vector2Int gridSize;
        public Vector2 gridPos;
        public List<ItemObject> itemList;

        public bool createRandomItem;
        public int randomItemAmount;
    }

    public class Inventory
    {
        public float limitWeight; //해당 인벤토리의 한계무게
        public List<GridData> gridList; //이 인벤토리에서 생성될 그리드의 데이터

    }
}
