using Google.Protobuf.Protocol;
using Server.Game;
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
        public ItemObject[,] gridSlot; //그리드에 채워진 아이템의 아이디(아이템이 그리드 어느위치에 있는지 보여주기 위함).

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

            gridSlot = new ItemObject[gridData.GridSizeX, gridData.GridSizeY];

            if (gridData.ItemList.Count != 0) //아이템의 데이터가 존재하면(아마 그럴일은 없겠지만 혹시나)
            {
                List<ItemDataInfo> sortDataList = new List<ItemDataInfo>(gridData.ItemList);
                sortDataList.OrderByDescending(item => item.Width * item.Height);
                foreach (ItemDataInfo item in sortDataList)
                {
                    //가장 큰것부터 넣음
                    PushInstantObject(item);
                    Console.WriteLine($"item  : {item.ItemName} \nItemId : {item.ItemId} \nItemCode : {item.ItemCode}\nItemConsume? : {item.IsItemConsumeable}\n" +
                    $"ItemAmount ={item.ItemAmount}\nitemPos = {item.ItemPosX},{item.ItemPosY}\n");
                }
            }
        }

        public void PushInstantObject(ItemDataInfo itemData)
        {
            //새로운 데이터 오브젝트 생성및 아이템 데이터 할당
            ItemObject newItemObj = ObjectManager.Instance.Find<ItemObject>(itemData.ItemId);

            newItemObj.itemDataInfo = itemData;
            newItemObj.ownerGrid = this;

            FindPlaceableSlot(newItemObj);

            //아이템 오브젝트의 아이템 데이터가 모두 설정이 완료됨.
            itemObjectList.Add(newItemObj);
        }

        private void FindPlaceableSlot(ItemObject item)
        {
            Vector2Int? posOnGrid = FindSpaceForObject(item);

            if (posOnGrid == null) //안들어가면 회전시켜서 한번더 체크해보고 그것도 안되면 그 아이템은 패스. 없애거나 다른 아이템으로 재설정하거나(미정)
            {
                item.ItemRotate+=1; //1회 회전
                posOnGrid = FindSpaceForObject(item);
                if (posOnGrid == null)
                {
                    return;
                }
            }

            //아이템오브젝트 데이터에서 현 위치를 업데이트
            PushItemIntoSlot(item, posOnGrid.Value.x, posOnGrid.Value.y);
        }

        /// <summary>
        /// 해당 위치에 아이템의 크기만큼의 공간을 해당 아이템 오브젝트를 넣음
        /// </summary>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public void PushItemIntoSlot(ItemObject item, int posX, int posY)
        {
            // 아이템의 위치를 갱신하고 슬롯에 배치
            item.itemDataInfo.ItemPosX = posX;
            item.itemDataInfo.ItemPosY = posY;

            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {

                    int targetX = posX + x;
                    int targetY = posY + y;
                    if (gridSlot[targetX, targetY] != null)
                    {
                        // 현재 슬롯에 다른 아이템이 있는 경우 병합 시도
                        item.MergeItem(gridSlot[targetX, targetY]);
                        return;
                    }
                    gridSlot[targetX, targetY] = item;
                }
            }
        }

        /// <summary>
        /// 아이템의 앵커위치(왼쪽 상단)부터 아이템의 크기 만큼의 공간을 널값으로 만듬
        /// </summary>
        /// <param name="deleteItem"></param>
        public void DeleteItemFromSlot(ItemObject deleteItem)
        {
            Console.WriteLine($"width : {deleteItem.Width}, height : {deleteItem.Height}");
            for (int x = 0; x < deleteItem.itemDataInfo.Width; x++)
            {
                for (int y = 0; y < deleteItem.itemDataInfo.Height; y++)
                {
                    gridSlot[deleteItem.itemDataInfo.ItemPosX + x, deleteItem.itemDataInfo.ItemPosY + y] = null;
                    Console.WriteLine($"X  : {deleteItem.itemDataInfo.ItemPosX + x}, Y : {deleteItem.itemDataInfo.ItemPosY + y}");
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
                        //해당 위치를 반환
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
                    //아이템이 해당 자리에 들어갈수 있는지 체크. 비어있지 않으면 배치 불가니 즉시 false 반환
                    if (gridSlot[posX + x, posY + y] != null)
                    {
                        if (gridSlot[posX + x, posY + y].itemDataInfo.IsItemConsumeable && gridSlot[posX + x, posY + y].itemDataInfo.ItemCode == item.ItemCode)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            //아이템 크기만큼의 슬롯이 모두 비어있다면 배치 가능
            return true;
        }

        public void PrintInvenContents()
        {
            string content = gridData.GridId + "의 슬롯 \n";

            for (int i = 0; i < gridSlot.GetLength(1); i++)
            {
                for (int j = 0; j < gridSlot.GetLength(0); j++)
                {
                    ItemObject item = gridSlot[j, i];
                    if (item != null)
                    {
                        content += $"| {item.itemDataInfo.ItemId},{item.itemDataInfo.ItemCode} |";

                    }
                    else
                    {
                        content += $"| Null |";
                    }
                }
                content += "\n";
            }

            Console.WriteLine(content);
        }
    }
}
