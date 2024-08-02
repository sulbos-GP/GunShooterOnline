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


        public void MoveItem(int id, int posX, int posY)
        {
            //아이템 가져오기
            ItemObject target =  ObjectManager.Instance.Find<ItemObject>(id);
            //TODO : instantGrid[0].ItemPushCheck


            // 지승현 -> 박성훈 
            /*instantGrid[0].ItemPushCheck(target, posX, posY);

            if(target != null )
            {
                if(target.itemDataInfo.ItemCode == instantGrid[0].gridSlot[posX,posY])
                {
                    Console.WriteLine("Merge");


                    instantGrid[0].
                }
            }
*/
           

            /*

             C_MoveItemHandler에서 패킷으로 받은 아이템 처리할때 아이템의 위치만 옮기는게 아니라 만약 해당 위치에 
            이미 아이템이 있다면 ItemDataInfo.isItemConsumable이 true고 아이템코드가 서로 같은지 확인하고  
            amount의 갯수를 늘리게 해야되  이게 내가 준 itemObject의 MergeItem이에

             */
        }




    }
}
