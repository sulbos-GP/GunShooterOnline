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
    public enum EStorageError
    {
        None,
        InsertFailed_DuplicateItem,
        InsertFailed_WeightLimit,
    }

    public class Storage
    {
        private List<ItemObject> items = new List<ItemObject>();     //저장소에 들어있는 아이템 오브젝트
        private List<List<int>> grid = new List<List<int>>();        //저장소의 그리드

        private int scale_x = 0;
        private int scale_y = 0;

        private double maxWeight = 0.0;                             //저장소 최대 무게
        private double curWeight = 0.0;                             //저장소 현재 무게

        public int Scale_X
        {
            get
            {
                return scale_x;
            }
        }

        public int Scale_Y
        {
            get
            {
                return scale_y;
            }
        }

        public double MaxWeight
        {
            get
            {
                return Math.Round(maxWeight, 1);
            }
            set
            {
                maxWeight = Math.Round(value, 1);
            }
        }

        public double CurWeight
        {
            get
            {
                return Math.Round(curWeight, 1);
            }
            set
            {
                curWeight = Math.Round(value, 1);
            }
        }

        public double LessWeight
        {
            get
            {
                return Math.Round(maxWeight - curWeight, 1);
            }
        }

        public List<ItemObject> Items
        {
            get
            {
                return items;
            }
        }

        public int ItemCount
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary>
        /// 그리드의 크기 및 무게 설정
        /// </summary>
        public void Init(int rows , int cols, double weight)
        {
            grid.Clear();
 
            this.items.Clear();

            scale_x = rows;
            scale_y = cols;

            this.MaxWeight = weight;
            this.CurWeight = 0.0;

            for (int i = 0; i < cols; i++)
            {
                List<int> row = new List<int>(new int[rows]);
                grid.Add(row);
            }

        }

        public void Fit()
        {
            int fitX = scale_x;
            int fitY = scale_y;

            for (int x = 0; x < scale_x; x++)
            {
                bool isEmpty = true;
                for (int y = 0; y < scale_y; y++)
                {
                    if (grid[y][x] == 1)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if(isEmpty == true)
                {
                    fitX = Math.Min(x, fitX);
                }
            }

            for (int y = 0; y < scale_y; y++)
            {
                bool isEmpty = true;
                for (int x = 0; x < scale_x; x++)
                {
                    if (grid[y][x] == 1)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty == true)
                {
                    fitY = Math.Min(y, fitY);
                }
            }

            double weight = 0.0f;
            foreach(ItemObject item in items)
            {
                weight += item.Weight * item.Amount;
            }

            scale_x = fitX;
            scale_y = fitY;
            maxWeight = weight;
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
            double weight = item.Weight * item.Amount;
            if (MaxWeight < CurWeight + weight)
            {
                return false;
            }

            if (false == this.StorageCheack(item, true))
            {
                return false;
            }

            CurWeight += weight;
            items.Add(item);

            return true;
        }

        /// <summary>
        /// 아이템 제거
        /// </summary>
        public bool DeleteItem(ItemObject item)
        {
            double weight = item.Weight * item.Amount;
            if (0 > CurWeight - weight)
            {
                return false;
            }

            if (false == this.StorageCheack(item, false))
            {
                return false;
            }
            CurWeight -= weight;
            items.Remove(item);

            return true;
        }

        public bool IsHaveAmount(ItemObject target, int amount)
        {
            int index = ScanItem(target);
            if(index == -1)
            {
                return false;
            }

            ItemObject item = items[index];
            return item.Amount >= amount;
        }

        /// <summary>
        ///아이템 무게에 따른 현재 아이템을 담을 수 있는 최대 수량
        /// </summary>
        public int CheackMaxAmount(ItemObject item, int amount)
        {
            
            if(LessWeight >= Math.Round(item.Weight * amount, 1))
            {
                return amount;
            }

            double weight = 0.0;
            int maxAmount = 0;
            while(LessWeight > weight)
            {
                weight = Math.Round((++maxAmount) * item.Weight, 1);
            }

            return maxAmount;
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
                CurWeight += Math.Round(item.Weight * amount, 1);
                item.Amount += amount;
                return item.Amount;
            }
        }


        /// <summary>
        /// 아이템 수량 감소 (return이  -1 일 경우 실패, 0이면 삭제 해야함)
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
                CurWeight -= Math.Round(item.Weight * amount, 1);
                item.Amount -= amount;
                return item.Amount;
            }
        }

        public void OverWriteToRollBackGrid(List<List<int>> rollback)
        {
            grid.Clear();
            foreach (var row in rollback)
            {
                grid.Add(new List<int>(row));
            }
        }


        public List<ItemObject> FindItemsByItemID(int itemID)
        {
            return items.FindAll(t => t.ItemId == itemID);
        }


        public int ScanItem(ItemObject target)
        {
            for (int index = 0; index < items.Count; ++index)
            {
                ItemObject item = items[index];
                if (item.Id == target.Id)
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

            List<List<int>> tempGrid = new List<List<int>>();
            foreach (var row in grid)
            {
                tempGrid.Add(new List<int>(row));
            }

            for (int i = y; i < y + height; ++i)
            {
                for (int j = x; j < x + width; ++j)
                {
                    int value = isPush ? 1 : -1;
                    tempGrid[i][j] += value;

                    if(tempGrid[i][j] == -1 || tempGrid[i][j] == 2)
                    {
                        return false;
                    }

                }
            }

            grid = tempGrid;
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

        public ItemObject GetItem(int index = 0)
        {
            return items[index];
        }

        public List<int> GetAllItemObjectIds()
        {
            return items.Select(item => item.Id).ToList();
        }
        public List<int> GetAllItemItemIds()
        {
            return items.Select(item => item.ItemId).ToList();
        }

        public void ClearStorage()
        {
            foreach (ItemObject item in items)
            {
                ObjectManager.Instance.Remove(item.Id);
            }
            items.Clear();
        }

        public void PrintInvenContents()
        {
            
            string content = $"[Storage : {items.Count}]\n";
            content += $"[size ( {scale_y} x {scale_x} )]\n";
            content += $"[weight ( {CurWeight} / {MaxWeight} )]\n";
            content += "{\n";

            for (int i = 0; i < scale_y; i++)
            {
                content += "\t";
                for (int j = 0; j < scale_x; j++)
                {
                    content += "[" + grid[i][j] + "]";
                }
                content += "\n";
            }
            content += "}\n";
            Console.WriteLine(content);
        }
    }
}
