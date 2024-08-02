using Google.Protobuf.Protocol;
using Server.Game.Object.Item;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Grid
    {
        public static int lastGridId = 0;
        
        public GridDataInfo gridData;
        /*int32 gridId = 1;
        int32 gridSizeX = 2;
        int32 gridSizeY = 3;
        float gridPosX = 4;
        float gridPosY = 5;
        repeated ItemDataInfo  itemList     = 6;*/

        public Inventory ownerInventory; //그리드를 소유한 인벤토리 (그냥 인벤토리 아이디로 바꿔도 괜춘)
        public List<ItemObject> itemObjectList = new List<ItemObject>(); //해당 그리드에 존재하는 아이템리스트
        public ItemDataInfo[,] gridSlot; //그리드에 채워진 아이템의 아이디(아이템이 그리드 어느위치에 있는지 보여주기 위함).

        public float GridWeight
        {
            get => gridWeight;
            set
            {
                gridWeight = value;
            }
        }
        private float gridWeight;
       
        //아이템 데이터 리스트의 아이템정보로 아이템을 슬롯에 넣고 어느 위치와 회전을 가지는지 업데이트(또한 아이템 오브젝트를 만들어 리스트에 추가해줌)
        public void SetGrid()
        {
            gridSlot = new ItemDataInfo[gridData.GridSizeX, gridData.GridSizeY];
            if (gridData.ItemList.Count != 0) //아이템의 데이터가 존재하면(아마 그럴일은 없겠지만 혹시나)
            {
                List<ItemDataInfo> sortDataList = new List<ItemDataInfo>(gridData.ItemList);
                sortDataList.OrderByDescending(item => item.Width * item.Height);
                foreach (ItemDataInfo item in sortDataList)
                {
                    //가장 큰것부터 넣음
                    RandomPushInstantObject(item);
                }
            }
        }

        public void RandomPushInstantObject(ItemDataInfo itemData)
        {
            //새로운 데이터 오브젝트 생성및 아이템 데이터 할당
            ItemObject newItemObj = new ItemObject();
            newItemObj.itemDataInfo = itemData;
            newItemObj.OwnerGrid = this;


            FindPlaceableSlot(newItemObj);
        }

        private void FindPlaceableSlot(ItemObject item)
        {
            Vector2Int? posOnGrid = FindSpaceForObject(item);

            if (posOnGrid == null)
            {
                item.ItemRotate+=1; //1회 회전
                posOnGrid = FindSpaceForObject(item);
                if (posOnGrid == null)
                {
                    return;
                }
            }

            //아이템오브젝트 데이터에서 현 위치를 업데이트
            PushItemIntoSlot(item.itemDataInfo, posOnGrid.Value.x, posOnGrid.Value.y);
        }

        public void PushItemIntoSlot(ItemDataInfo item, int posX, int posY)
        {

            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    //아이템의 위치에 아이템의 크기 만큼 슬롯에 아이템 코드로 채움 -> 어떤 슬롯에 들어있는 아이템을 검색하기 위해 아이디로 변경
                    gridSlot[item.ItemPosX + x, item.ItemPosX + y] = item;
                }
            }
        }

        public Vector2Int? FindSpaceForObject(ItemObject insertItem)
        {
            //검색할 너비와 높이를 아이템 크기에 따라 조정(아이템이 크면 그만큼 조회할 슬롯이 적어짐)
            int width = gridData.GridSizeX - (insertItem.Width - 1);
            int height = gridData.GridSizeY - (insertItem.Height - 1);

            if(width<0 || height < 0)
            {
                return null;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (ItemPushCheck(insertItem.itemDataInfo, x,y) == true)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }
            return null;
        }

        public bool ItemPushCheck(ItemDataInfo item, int posX, int posY) // -> itemobject
        {
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    //아이템이 해당 자리에 들어갈수 있는지 체크. 0(null)이면 삽입 가능
                    if (gridSlot[posX + x, posY + y] != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void PrintInvenContents()
        {
            string content = gridData.GridId + "의 슬롯 \n";

            for (int i = 0; i < gridSlot.GetLength(1); i++)
            {
                for (int j = 0; j < gridSlot.GetLength(0); j++)
                {
                    ItemDataInfo item = gridSlot[j, i];
                    if (item != null)
                    {
                        content += $"| {item.ItemId},{item.ItemCode} |";
                    }
                    else
                    {
                        content += $"| Null |";
                    }
                }
                content += "\n";
            }

            Debug.Print(content);
        }
    }
}
