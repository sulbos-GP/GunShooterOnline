using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ItemObject
    {
        public const int maxItemMergeAmount = 64;
        public ItemDataInfo itemDataInfo; //데이터(아이템 코드, 이름, 조회시간, 크기 , 이미지)

        //회전상태에 따른 너비와 높이를 보려면 이걸로
        public int Width
        {
            get
            {
                if (itemDataInfo.ItemRotate % 2 == 0)
                {
                    return itemDataInfo.Width;
                }
                return itemDataInfo.Height;
            }
        }
        public int Height
        {
            get
            {
                if (itemDataInfo.ItemRotate % 2 == 0)
                {
                    return itemDataInfo.Height;
                }
                return itemDataInfo.Width;
            }
        }

        private void Init()
        {
            if (itemDataInfo == null)
            {
                //해당 오브젝트에는 아이템 데이터 info가 있어야함
                return;
            }
        }

        /// <summary>
        /// 같은 아이템코드의 아이템을 같은 위치에 두었을경우 
        /// targetItem = 아이템 배치에 놓은 위치에 존재하는 같은 코드의 아이템
        /// 현재 아이템과 타겟아이템의 개수를 합친것이 최대인 64개 보다 높으면 -> 64-타겟아이템의 양을 타겟에 더하고 현재아이템에 뺌 -> 현재 아이템을 원위치로
        /// 같거나 낮으면 타겟아이템의 개수에 현재 아이템의 개수를 더하고 현재 아이템은 없앰
        /// </summary>
        public void MergeItem(ItemObject targetItem)
        {
            //현재 오브젝트와 합치려는 오브젝트와 같으면
            if (itemDataInfo.ItemCode != targetItem.itemDataInfo.ItemCode)
            {
                return;
            }

            if (itemDataInfo.ItemAmount + targetItem.itemDataInfo.ItemAmount > maxItemMergeAmount)
            {
                //두개의 아이템을 합친 개수가 64개보다 클경우 타겟 아이템은 64개로 만들고 기존 아이템은 남은 개수를 가지고 원래 위치로 귀환

                //타깃 아이템의 개수가 최대가 되기위해 남은 개수
                int indexAmount = maxItemMergeAmount - targetItem.itemDataInfo.ItemAmount;

                targetItem.itemDataInfo.ItemAmount += indexAmount;
                itemDataInfo.ItemAmount -= indexAmount;
                return;
            }
            else
            {
                targetItem.itemDataInfo.ItemAmount += itemDataInfo.ItemAmount;

                DestroyItem();
            }
        }

        public void DestroyItem()
        {
            //해당 아이템 오브젝트 삭제

        }










       /* public void SetItemData(ItemDataInfo itemDataInfo)
        {
            itemId = itemDataInfo.ItemId;
            itemCode = itemDataInfo.ItemCode;
            itemPos = new Vector2Int(itemDataInfo.ItemPosX, itemDataInfo.ItemPosY);
            itemRotate = itemDataInfo.ItemRotate;
            itemAmount = itemDataInfo.ItemAmount;

            foreach (int id in itemDataInfo.SearchedPlayerId)
            {
                searchedPlayerId.Add(id);
            }

            //임시
            item_name = itemDataInfo.ItemName;
            item_weight = itemDataInfo.ItemWeight; //아이템의 무게
            item_type = itemDataInfo.ItemType;
            item_string_value = itemDataInfo.ItemStringValue;
            item_purchase_price = itemDataInfo.ItemPurchasePrice;
            item_sell_price = itemDataInfo.ItemSellPrice;
            item_searchTime = itemDataInfo.ItemSearchTime;
            width = itemDataInfo.Width;
            height = itemDataInfo.Height;
            isItemConsumeable = itemDataInfo.IsItemConsumeable; //임시(아이템 타입으로 유추가능, 아이템 머지에 소모품인지 판단함. 이후 코드를 통해 조회로 변경)
                                                                //itemSprite = itemDataInfo.ItemSprite;
        }

        /// <summary>
        /// 현재 스크립트의 변수를 ItemDataInfo로 변환
        /// </summary>
        public ItemDataInfo GetItemData()
        {
            ItemDataInfo itemDataInfo = new ItemDataInfo();
            itemDataInfo.ItemId = itemId;
            itemDataInfo.ItemCode = itemCode;
            itemDataInfo.ItemPosX = itemPos.x;
            itemDataInfo.ItemPosY = itemPos.y;
            itemDataInfo.ItemRotate = itemRotate;
            itemDataInfo.ItemAmount = itemAmount;

            foreach (int id in searchedPlayerId)
            {
                itemDataInfo.SearchedPlayerId.Add(id);
            }

            //임시

            itemDataInfo.ItemName = item_name;
            itemDataInfo.ItemWeight = item_weight; //아이템의 무게
            itemDataInfo.ItemType = item_type;
            itemDataInfo.ItemStringValue = item_string_value;
            itemDataInfo.ItemPurchasePrice = item_purchase_price;
            itemDataInfo.ItemSellPrice = item_sell_price;
            itemDataInfo.ItemSearchTime = item_searchTime;
            itemDataInfo.Width = width;
            itemDataInfo.Height = height;
            itemDataInfo.IsItemConsumeable = isItemConsumeable;  //임시(아이템 타입으로 유추가능, 아이템 머지에 소모품인지 판단함. 이후 코드를 통해 조회로 변경)
                                                                 //itemSprite = itemDataInfo.ItemSprite;

            return itemDataInfo;
        }
*/
    }
}
