using Google.Protobuf.Protocol;
using Pipelines.Sockets.Unofficial.Buffers;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Game
{
    public class Inventory
    {
        public InvenDataInfo invenData = new InvenDataInfo();
        /*int32 inventoryId = 1;
        float limitWeight = 2;
        repeated GridDataInfo GridData = 3;*/
        public List<Grid> instantGrid = new List<Grid>(); //해당 인벤토리가 소유한 그리드


        //인벤토리가 처음 생성될때?
        public Inventory(int ownerId, int x = 0, int y = 0)
        {
            invenData.InventoryId = ownerId;
            invenData.LimitWeight = 20; //임시
            Grid newGrid = new Grid();
            if (x == 0)
            {
                x = 4;
            }
            if (y == 0)
            {
                y = 5;
            }
            newGrid.gridData = MakeNewGridData(x,y);
            newGrid.ownerInventory = this;
            newGrid.SetGrid();
            instantGrid.Add(newGrid);
            invenData.GridData.Add(newGrid.gridData);
            Console.WriteLine($"ownerId : {ownerId} \n instantGridAmt : {instantGrid.Count} \nitemAmount : {newGrid.itemObjectList.Count} \n");
        }

        private GridDataInfo MakeNewGridData(int x, int y)
        {
            //그리드의 데이터를 생성함과 동시에 그리드 데이터의 아이템데이터 리스트에 넣을 아이템의 데이터 또한 생성
            GridDataInfo newData = new GridDataInfo
            {
                GridId = ++Grid.lastGridId,
                GridSizeX = x, //임시
                GridSizeY = y, //임시
                GridPosX = 0,
                GridPosY = 0, //지금은 1개뿐이라 0,0 나중에 인벤토리가 어떻게 생겼는지에 대한 데이터를 추가해야할듯
                RandomItemAmount = 3 //임시. 소유자의 조건에 따라 달라짐
            };

            Console.WriteLine($"GridId : {newData.GridId} \nGridSize : {newData.GridSizeX},{newData.GridSizeY}\n" +
                $"gridPos ={newData.GridPosX},{newData.GridPosY}\n");

            CreateRandomItemDataIntoGridData(newData);

            return newData;
        }

        private void CreateRandomItemDataIntoGridData(GridDataInfo gridData)
        {
            int restSize = gridData.GridSizeX * gridData.GridSizeY;
            List<ItemDataInfo> canInsertlist = new List<ItemDataInfo>();
            foreach (ItemDataInfo data in ItemDB.Instance.items.Values)
            {
                if (data.Width * data.Height < restSize)
                {
                    canInsertlist.Add(data);
                }
            }

            //생성해야하는 아이템의 수만큼 반복
            for (int i = 0; i < gridData.RandomItemAmount; i++)
            {
                if (restSize <= 0)
                {
                    //남은 공간이 없다면 반복 중지
                    break;
                }
                //두번째 순서부터 
                if (i != 0)
                {
                    for (int j = canInsertlist.Count - 1; j >= 0; j--)
                    {
                        ItemDataInfo data = canInsertlist[j];
                        if (data.Width * data.Height > restSize)
                        {
                            canInsertlist.RemoveAt(j);
                        }
                    }
                }

                if (canInsertlist.Count <= 0)
                {
                    //넣을수 있는 아이템이 없다면 브레이크
                    break;
                }

                System.Random rnd = new System.Random();
                int random = rnd.Next(0, canInsertlist.Count);
                ItemDataInfo newItemData = DuplicateItemData(canInsertlist[random]);

                bool itemExists = false;
                //아이템의 위치, 회전을 제외한 아이디+데이터베이스 데이터를 그리드에 넣어줌
                //그리드에서 이 아이템 리스트를 기반으로 그리드 슬롯에 넣고 아이템 데이터 업데이트예정
                if (newItemData.IsItemConsumeable)
                {
                    foreach (ItemDataInfo data in gridData.ItemList)
                    {
                        if (data.ItemCode == newItemData.ItemCode)
                        {
                            data.ItemAmount += 1;
                            itemExists = true;
                            break;
                        }
                    }
                }

                // 중복이 아닌 경우에만 새 아이템 추가
                if (!itemExists)
                {
                    newItemData.ItemId = ObjectManager.Instance.Add<ItemObject>().Id;
                    gridData.ItemList.Add(newItemData);

                    restSize -= canInsertlist[random].Width * canInsertlist[random].Height;
                }
            }
        }

        

        public void MoveItem(int id, int posX, int posY, int rotate)
        {
            //아이템 가져오기
            ItemObject target = ObjectManager.Instance.Find<ItemObject>(id);

            if(target == null)
            {
                return;
            }
            Console.WriteLine($"MoveItem Method\n Item = {target.itemDataInfo.ItemName}, id = {target.itemDataInfo.ItemId}\n" +
                $"pos : ({target.itemDataInfo.ItemPosX},{target.itemDataInfo.ItemPosX}) -> ({posX},{posY})\n" +
                $"rotate : {target.itemDataInfo.ItemRotate} -> {rotate}");
            target.ownerGrid.DeleteItemFromSlot(target);
            target.ItemRotate = rotate;
            target.ownerGrid.PushItemIntoSlot(target,posX,posY);
            target.ownerGrid.PrintInvenContents();
        }

        public void DeleteItem(int id)
        {
            //아이템 가져오기
            ItemObject target = ObjectManager.Instance.Find<ItemObject>(id);

            if (target == null)
            {
                return;
            }

            target.ownerGrid.DeleteItemFromSlot(target);

            target.ownerGrid.PrintInvenContents();
        }

        public ItemDataInfo DuplicateItemData(ItemDataInfo targetData)
        {
            ItemDataInfo newItemData = new ItemDataInfo();
            newItemData.ItemId = targetData.ItemId;
            newItemData.ItemPosX = targetData.ItemPosX;
            newItemData.ItemPosY = targetData.ItemPosY;
            newItemData.ItemRotate = targetData.ItemRotate;
            newItemData.ItemAmount = targetData.ItemAmount;
            newItemData.ItemCode = targetData.ItemCode;
            newItemData.IsItemConsumeable = targetData.IsItemConsumeable;
            newItemData.ItemName = targetData.ItemName;
            newItemData.ItemWeight = targetData.ItemWeight;
            newItemData.ItemType = targetData.ItemType;
            newItemData.ItemStringValue = targetData.ItemStringValue;
            newItemData.ItemPurchasePrice = targetData.ItemPurchasePrice;
            newItemData.ItemSellPrice = targetData.ItemSellPrice;
            newItemData.Width = targetData.Width;
            newItemData.Height = targetData.Height;
            newItemData.ItemSearchTime = targetData.ItemSearchTime;
            return newItemData;
        }
    }
}
