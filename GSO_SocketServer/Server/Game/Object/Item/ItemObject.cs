using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ItemObject : GameObject
    {
        public static int lastItemId = 0;
        public const int maxItemMergeAmount = 64;
        /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/

        public Grid OwnerGrid;
        public ItemDataInfo itemDataInfo;
        /*소유한 데이터
        int32 itemId = 1;        // 해당 아이템의 고유한 아이디
        int32 itemCode = 2;        //아이템의 종류(해당 아이템을 DB에서 조회하기 위한 코드)
        int32 itemPosX = 3;        // 아이템의 그리드 안 좌표상의 위치
        int32 itemPosY = 4;        // 아이템의 그리드 안 좌표상의 위치
        int32 itemRotate = 5;        // 아이템의 회전코드(rotate * 90)
        int32 itemAmount = 6;      // 아이템의 개수(소모품만 64개까지)
        repeated int32 searchedPlayerId     = 7;            // 이 아이템을 조회한 플레이어의 아이디

         //임시
        string item_name = 8;
        float item_weight = 9;
        int32 item_type = 10;
        int32 item_string_value = 11;
        int32 item_purchase_price = 12;
        int32 item_sell_price = 13;
        float item_searchTime = 14;
        int32 width = 15;
        int32 height = 16;
        bool isItemConsumeable = 17;
        */


        public ItemObject()
        {
            ObjectType = GameObjectType.Item;
        }


        public int ItemRotate
        {
            get => itemDataInfo.ItemRotate;
            set
            {
                itemDataInfo.ItemRotate = value;
                itemDataInfo.Width = Width;
                itemDataInfo.Height = Height;
            }
        }
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

        //첫 생성시
        private void Init()
        {
            if (itemDataInfo == null)
            {
                //해당 오브젝트에는 반드시 아이템 데이터 info가 있어야함
                return;
            }
        }

        /// <summary>
        /// 같은 아이템코드의 아이템을 같은 위치에 두었을경우 
        /// targetItem = 아이템 배치에 놓은 위치에 존재하는 같은 코드의 아이템
        /// 현재 아이템과 타겟아이템의 개수를 합친것이 최대인 64개 보다 높으면 -> 64-타겟아이템의 양을 타겟에 더하고 현재아이템에 뺌 -> 현재 아이템을 원위치로
        /// 같거나 낮으면 타겟아이템의 개수에 현재 아이템의 개수를 더하고 현재 아이템은 없앰
        /// </summary>
        public void MergeItem(ItemObject itemObj)
        {
            //놓으려는 위치에 아이템이 있는데 배치에 성공했다면 이것은 아이템을 머지해야한다는 것(애초에 배치 실패였으면 패킷도 안옴)

            //두개의 아이템을 합친값이 최댓값보다 큰경우 -> 타겟의 아이템은 맥스가 되고 배치 아이템은 개수가 감소한뒤 원래 위치로 돌아감
            if (itemDataInfo.ItemAmount + itemObj.itemDataInfo.ItemAmount > maxItemMergeAmount)
            {
                //두개의 아이템을 합친 개수가 64개보다 클경우 타겟 아이템은 64개로 만들고 기존 아이템은 남은 개수를 가지고 원래 위치로 귀환
                //타깃 아이템의 개수가 최대가 되기위해 남은 개수
                int indexAmount = maxItemMergeAmount - itemObj.itemDataInfo.ItemAmount;

                itemObj.itemDataInfo.ItemAmount += indexAmount;
                itemDataInfo.ItemAmount -= indexAmount;

                OwnerGrid.PushItemIntoSlot(this, itemDataInfo.ItemPosX, itemDataInfo.ItemPosY);
                return;
            }
            else
            {
                itemObj.itemDataInfo.ItemAmount += itemDataInfo.ItemAmount;

                DestroyItem();
            }
        }

        public void DestroyItem()
        {
            //해당 아이템 오브젝트 삭제
        }


    }
}
