using Collision.Shapes;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class RootableObject : GameObject
    {
        protected bool destroyed = false;
        protected bool active = true;
        public Shape  _shape;
        public Inventory Inventory;

        public RootableObject()
        {
            ObjectType = GameObjectType.Item;
        }




      


        //시작이자 끝. FindSpaceForObject로 위치를 받았다면 그 위치에 아이템을 배치하고 없으면 아이템을 돌려서 다시한번 찾아보고 그래도 없으면 그냥 리턴하여 해당 아이템은 배치를 하지 않음
        public void FindPlaceableSlot(ItemObject item)
        {
            Vector2Int? posOnGrid = FindSpaceForObject(item);

            if (posOnGrid == null)
            {
                item.ItemRotate += 1;
                posOnGrid = FindSpaceForObject(item);
                if (posOnGrid == null)
                {
                    return;
                }
            }

            Inventory.instantGrid[0].PushItemIntoSlot(item.itemDataInfo, posOnGrid.Value.x, posOnGrid.Value.y);
            Console.WriteLine("Success Add Item!");
        }

        //



        //그리드의 크기를 아이템의 크기만큼 줄이고 해당 위치에 아이템이 배치 가능한지 검색
        public Vector2Int? FindSpaceForObject(ItemObject itemToInsert)
        {


            int width = Inventory.invenData.GridData[0].GridSizeX - (itemToInsert.Width - 1);
            int height = Inventory.invenData.GridData[0].GridSizeY - (itemToInsert.Height - 1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (CheckAvailableSpace(x, y, itemToInsert.Width, itemToInsert.Height) == true)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }
            return null;
        }

        //해당 아이템이 들어갈수 있는지 체크함'


        //선택된 위치에서 아이템의 크기만큼 좌표에 아이템이 비어있는지 체크
        public bool CheckAvailableSpace(int posX, int posY, int width, int height)
        {
            //아이템 슬롯 순회
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //해당 아이템 슬롯이 비어있지 않을 경우 
                    if (Inventory.instantGrid[0].gridSlot[posX + x, posY + y] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
