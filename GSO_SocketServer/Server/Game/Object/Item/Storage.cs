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
using static Humanizer.In;

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
        public double maxWeight = 0.0;                              //저장소 최대 무게
        public double curWeight = 0.0;                              //저장소 현재 무게

        /// <summary>
        /// 그리드의 크기 및 무게 설정
        /// </summary>
        public void Init(int rows , int cols, double weight)
        {
            grid.Clear();
 
            this.items.Clear();
            this.maxWeight = weight;
            this.curWeight = 0.0;

            for (int i = 0; i < cols; i++)
            {
                List<int> row = new List<int>(new int[rows]);
                grid.Add(row);
            }

        }

        /// <summary>
        /// Storage간 결합
        /// </summary>
        public void ChangeStorge(Storage newStorge)
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

            double weight = item.Weight * item.Amount;
            if(maxWeight < curWeight + weight)
            {
                OverWriteToRollBackGrid(rollback);
                return false;
            }

            curWeight += weight;
            items.Add(item);
            item.CreateItem();

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

            double weight = item.Weight * item.Amount;
            if (0 > curWeight - weight)
            {
                OverWriteToRollBackGrid(rollback);
                return false;
            }

            if (false == items.Remove(item))
            {
                OverWriteToRollBackGrid(rollback);
                return false;
            }

            curWeight -= weight;
            item.DestroyItem();

            PrintInvenContents();
            return true;
        }

        /// <summary>
        /// 아이템 수량 증가
        /// </summary>
        public int IncreaseAmount(ItemObject item, int amount)
        {
            int value = item.Amount + amount;
            if (value > item.LimitAmount)
            {
                return -1;
            }
            else
            {
                this.curWeight += item.Weight * amount;
                item.Amount += amount;
                return item.Amount;
            }
        }


        /// <summary>
        /// 아이템 수량 감소 (0일 경우 삭제)
        /// </summary>
        public int DecreaseAmount(ItemObject item, int amount)
        {
            int value = item.Amount - amount;
            if(value < 0)
            {
                return -1;
            }
            else
            {
                this.curWeight -= item.Weight * amount;
                item.Amount -= amount;
                return item.Amount;
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
                if (item.ItemId == target.ItemId && item.Rotate == target.Rotate && item.X == target.X && item.Y == target.Y)
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

            if(y + height > grid.Count() || y + height < 0 || x + width > grid[0].Count() || x + width < 0)
            {
                return false;
            }

            for (int i = y; i < y + height; ++i)
            {
                for (int j = x; j < x + width; ++j)
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

        public IEnumerable<PS_ItemInfo> GetItems(int viewerId)
        {
            List<PS_ItemInfo> infos = new List<PS_ItemInfo>();
            foreach (ItemObject item in items)
            {
                infos.Add(item.ConvertItemInfo(viewerId));
            }
            return infos;
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
