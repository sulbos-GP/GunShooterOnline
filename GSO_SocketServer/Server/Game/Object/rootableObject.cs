using Collision.Shapes;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class rootableObject : GameObject
    {
        protected bool destroyed = false;
        protected bool active = true;
        public Shape _shape;
        Inventory Inventory;

        public rootableObject()
        {
            ObjectType = GameObjectType.Item;
        }


       /* private void FindPlaceableSlot(ItemObject item)
        {
            Vector2Int? posOnGrid = FindSpaceForObject(item);

            if (posOnGrid == null)
            {
                //item.RotateRight();



                posOnGrid = FindSpaceForObject(item);
                if (posOnGrid == null)
                {
                    return;
                }
            }
           

            Inventory.instantGrid[0].PushItemIntoSlot(item.GetItemData(), posOnGrid.Value.x, posOnGrid.Value.y);
        }

        /// <summary>
        /// 아이템의 크기만큼 들어갈 장소를 찾음
        /// ?를 쓴 이유는 마지막 리턴값에 널값을 허용하기 위함
        /// </summary>
        public Vector2Int? FindSpaceForObject(ItemObject itemToInsert)
        {
            int width = Inventory.instantGrid[0].gridData.GridSizeX - (itemToInsert.Width - 1);
            int height = gridSize.y - (itemToInsert.Height - 1);

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
        }*/
    }
}
