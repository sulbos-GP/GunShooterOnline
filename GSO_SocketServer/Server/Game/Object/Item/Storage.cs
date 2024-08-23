using Google.Protobuf.Protocol;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Server.Game
{

    public enum EStorageResult
    {
        Successed,
        Failed,
        ZeroAmount,
    }

    public class Storage
    {
        public List<ItemObject> items = new List<ItemObject>();     //저장소에 들어있는 아이템 오브젝트
        public List<List<int>> grid = new List<List<int>>();        //저장소의 그리드
        public double weight = 0.0;                                 //저장소 무게

        /// <summary>
        /// 그리드의 크기 및 무게 설정
        /// </summary>
        public void Init(int rows , int cols, double weight)
        {
            grid.Clear();
 
            this.items.Clear();
            this.weight = weight;

            for (int i = 0; i < cols; i++)
            {
                List<int> row = new List<int>(new int[rows]);
                grid.Add(row);
            }

        }

        /// <summary>
        /// Storage간 결합
        /// </summary>
        public void Merge(Storage other)
        {

        }

        /// <summary>
        /// 아이템 삽입
        /// </summary>
        public bool InsertItem(ItemObject item)
        {
            //자리가 겹치는지 확인
            List<List<int>> rollback = InitRollBack();
            if (false == StorageCheack(item, true))
            {
                OverWriteToRollBackGrid(rollback);
                return false;
            }
            PrintInvenContents();
            items.Add(item);
            return true;
        }

        /// <summary>
        /// 아이템 합치기
        /// </summary>
        public bool MergeItem(ItemObject mergedItem, ItemObject combinedItem, int mergeNumber)
        {
            int index = ScanItem(mergedItem);
            if (index == -1)
            {
                return false;
            }

            index = ScanItem(combinedItem);
            if (index == -1)
            {
                return false;
            }

            if (mergedItem.Info.ItemId != combinedItem.Info.ItemId)
            {
                return false;
            }

            int limit = mergedItem.LimitAmount;
            if (mergedItem.Amount + mergeNumber > limit)
            {
                return false;
            }

            if (combinedItem.Amount - mergeNumber < 0)
            {
                return false;
            }

            mergedItem.Amount += mergeNumber;
            combinedItem.Amount -= mergeNumber;

            if (combinedItem.Amount == 0)
            {
                items.Remove(combinedItem);
                combinedItem.DestroyItem();
            }

            PrintInvenContents();
            return true;
        }

        /// <summary>
        /// 아이템 제거
        /// </summary>
        public bool DeleteItem(ItemObject item)
        {
            //자리가 겹치는지 확인
            List<List<int>> rollback = InitRollBack();
            if (false == StorageCheack(item, false))
            {
                OverWriteToRollBackGrid(rollback);
                return false;
            }

            if(false == items.Remove(item))
            {
                OverWriteToRollBackGrid(rollback);
                return false;
            }

            PrintInvenContents();
            return true;
        }

        /// <summary>
        /// 아이템 나누기
        /// </summary>
        public ItemObject DevideItem(ItemObject totalItem, int gridX, int gridY, int rotation, int devideNumber)
        {
            int index = ScanItem(totalItem);
            if (index == -1)
            {
                return null;
            }

            if (totalItem.Amount - devideNumber < 0)
            {
                return null;
            }

            ItemObject newItem = new ItemObject(totalItem.Data.item_id);
            newItem.X = gridX;
            newItem.Y = gridY;
            newItem.Rotate = rotation;
            newItem.Amount = devideNumber;

            if(false == InsertItem(newItem))
            {
                return null;
            }
            totalItem.Amount -= devideNumber;

            items.Add(newItem);
            ObjectManager.Instance.Add(newItem);

            PrintInvenContents();
            return newItem;
        }

        /// <summary>
        /// 아이템 수량 증가
        /// </summary>
        public EStorageResult IncreaseAmount(ItemObject item, int amount)
        {
            int value = item.Amount + amount;
            if (value > item.LimitAmount)
            {
                return EStorageResult.Failed;
            }
            else
            {
                item.Amount += amount;
                return EStorageResult.Successed;
            }
        }


        /// <summary>
        /// 아이템 수량 감소 (0일 경우 삭제)
        /// </summary>
        public EStorageResult DecreaseAmount(ItemObject item, int amount)
        {
            int value = item.Amount - amount;
            if(value < 0)
            {
                return EStorageResult.Failed;
            }
            else if(value == 0)
            {
                item.Amount = 0;
                items.Remove(item);
                return EStorageResult.ZeroAmount;
            }
            else
            {
                item.Amount -= amount;
                return EStorageResult.Successed;
            }
        }

        public List<List<int>> InitRollBack()
        {
            List<List<int>> rollback = new List<List<int>>();
            foreach (var row in grid)
            {
                rollback.Add(new List<int>(row));
            }
            return rollback;
        }

        public void OverWriteToRollBackGrid(List<List<int>> rollback)
        {
            grid.Clear();
            foreach (var row in rollback)
            {
                grid.Add(new List<int>(row));
            }
        }

        public int ScanItem(ItemObject target)
        {
            for (int index = 0; index < items.Count; ++index)
            {
                ItemObject item = items[index];
                if (item.Info.ItemId == target.Info.ItemId && item.Rotate == target.Rotate && item.X == target.X && item.Y == target.Y)
                {
                    return index;
                }
            }
            return -1;
        }

        public bool StorageCheack(ItemObject item, bool isPush)
        {
            int x = item.X;
            int y = item.Y;
            int width = item.Width;
            int height = item.Height;

            for (int i = y; i < height; ++i)
            {
                for (int j = x; j < width; ++j)
                {
                    int value = isPush ? 1 : -1;
                    grid[i][j] += value;

                    if(grid[i][j] == -1 || grid[i][j] == 2)
                    {

                        return false;
                    }

                }
            }
            return true;
        }

        public void PrintInvenContents()
        {
            
            string content = "[PrintInvenContents]\n";

            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < grid[i].Count; j++)
                {
                    content += "[" + grid[i][j] + "]";
                }
                content += "\n";
            }

            Console.WriteLine(content);

        }
    }
}
