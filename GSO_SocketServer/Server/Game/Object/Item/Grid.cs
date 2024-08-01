using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    internal class Grid
    {
        public static int CreateItemId = 1;

        public GridDataInfo gridData;
        public Inventory ownerInventory; //그리드를 소유한 인벤토리 (그냥 인벤토리 아이디로 바꿔도 괜춘)
        public int gridId;
        public int[,] gridSlot; //그리드에 채워진 아이템의 코드

        public float GridWeight
        {
            get => gridWeight;
            set
            {
                gridWeight = value;
            }
        }
        private float gridWeight;

        private void Init()
        {
            gridSlot = new int[gridData.GridSizeX, gridData.GridSizeY];

            if (gridData.ItemList.Count == 0)
            {
                //아이템이 비어있다면 랜덤한 아이템을 생성해야하는지 체크
                if (gridData.CreateRandomItem)
                {
                    for (int i = 0; i < gridData.RandomItemAmount; i++)
                    {
                        gridData.ItemList.Add(CreateItemInfo());
                    }
                    gridData.CreateRandomItem = false;
                }
            }

            foreach (ItemDataInfo items in gridData.ItemList)
            {
                PushItemIntoSlot(items);
            }

        }

        //랜덤한 아이템의 정보를 생성. 
        private ItemDataInfo CreateItemInfo()
        {
            //새로운 아이템을 생성하여 아이템 리스트에 추가
            ItemDataInfo newItemInfo = new ItemDataInfo();

            //랜덤한 아이템의 코드를 뽑아서 그 코드로 데이터베이스 조회하여 아이템의 Info 설정해주고 반환
            System.Random rnd = new System.Random();
            int randomCode = rnd.Next(1, 6); //1~5까지의 랜덤한 코드를 반환
            newItemInfo.ItemId = CreateItemId;
            CreateItemId++;
            newItemInfo.ItemCode = randomCode;



            newItemInfo.ItemAmount = 1;
            //newItemInfo.SearchedPlayerId = 이거 new로 할당해줘야하나?;



            return newItemInfo;

        }

        private bool ItemPushCheck(ItemDataInfo item, int posX, int posY)
        {
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    //아이템이 해당 자리에 들어갈수 있는지 체크. 0(null)이면 삽입 가능
                    if (gridSlot[posX + x, posY + y] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void PushItemIntoSlot(ItemDataInfo item)
        {
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    //아이템의 위치에 아이템의 크기 만큼 슬롯에 아이템 코드로 채움
                    gridSlot[item.ItemPosX + x, item.ItemPosX + y] = item.ItemCode;
                }
            }
        }

        public void PrintInvenContents(int[,] gridSlot)
        {
            string content = gridId + "\n";

            for (int i = 0; i < gridSlot.GetLength(1); i++)
            {
                for (int j = 0; j < gridSlot.GetLength(0); j++)
                {
                    int itemCode = gridSlot[j, i];
                    if (itemCode != 0)
                    {
                        content += $"| {itemCode} |";
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
