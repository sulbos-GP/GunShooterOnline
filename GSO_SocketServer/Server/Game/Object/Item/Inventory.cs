using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Inventory
    {
        public int InvenId;
        public InvenDataInfo invenData = new InvenDataInfo();

        public List<Grid> instantGrid = new List<Grid>();


        public Inventory(int id)
        {
            invenData.InventoryId = id;
            instantGrid.Add(new Grid());
        }


        public void GetItem(int id, int posX, int posY)
        {
            //아이템 가져오기
            ItemDataInfo info =  ObjectManager.Instance.Find2(id).GetItemData();
            instantGrid[0].PushItemIntoSlot(info, posX, posY);
        }

    }
}
