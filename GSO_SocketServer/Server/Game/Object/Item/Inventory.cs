using Google.Protobuf.Protocol;
using Pipelines.Sockets.Unofficial.Buffers;
using Server.Database.Handler;
using Server.Game;
using Server.Game.Object;
using Server.Game.Object.Item;
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
    public class Inventory : GameObject
    {
        public Player owner;                //소유자 (플레이어)
        public int storage_id = 0;          //가방 아이디
        public int storage_item_id = 0;     //가방의 아이템 아이디

        public Storage storage = new Storage();

        /// <summary>
        /// 처음 접속한 이후 데이터베이스의 인벤토리 데이터 가져오기
        /// </summary>
        public Inventory(Player owner, int storage_id)
        {
            this.owner = owner;
            InitInventory(storage_id).Wait();
            LoadInventory().Wait();
        }

        /// <summary>
        /// 새로운 가방을 장착 시
        /// </summary>
        public void MakeInventory(int storage_id)
        {
            if(storage.items.Count == 0)
            {
                //가방 처음 생성
                InitInventory(storage_id).Wait();
            }
            else
            {
                //기존에 아이템이 있을 경우
            }
        }

        /// <summary>
        /// 가방에 따른 인벤토리 초기화
        /// </summary>
        public async Task InitInventory(int storage_id)
        {
            try
            {
                //가방(아이템)에 대한 정보 불러오기
                int storage_item_id = await DatabaseHandler.GameDB.GetStorageItemId(storage_id);

                //마스터 테이블의 아이템 데이터 불러와서 가방의 정보 얻기 (지금은 임시)
                //Backpack backpack = await DatabaseHandler.MasterDB.GetBackpackInfo(storage_item_id);
                Backpack backpack = new Backpack();

                storage.Init(backpack.scale_x, backpack.scale_y, backpack.limit_weight);

                this.storage_id = storage_id;
                this.storage_item_id = storage_item_id;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[InitInventory] : {e.Message.ToString()}");
            }
        }

        /// <summary>
        /// 저장소 아이디에 있는 모든 아이템 불러오기
        /// </summary>
        public async Task LoadInventory()
        {
            try
            {
                IEnumerable<DB_InventoryUnit> units = await DatabaseHandler.GameDB.LoadInventory(this.storage_id);

                if (units == null)
                {
                    return;
                }

                foreach (DB_InventoryUnit unit in units)
                {
                    ItemObject newItem = new ItemObject(unit.item_id);
                    ObjectManager.Instance.Add(newItem);

                    if (false == storage.PushItem(newItem))
                    {
                        throw new Exception("인벤토리 DB로드 실패");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"[LoadInventory] : {e.Message.ToString()}");
            }
        }

        /// <summary>
        /// 자신의 인벤토리에 있는 아이템 삭제
        /// </summary>
        public async Task<bool> DeleteItem(ItemObject deleteItem)
        {
            try
            {

                if (deleteItem == null)
                {
                    throw new Exception("삭제하려는 아이템이 존재하지 않음");
                }

                if(-1 == storage.ScanItem(deleteItem))
                {
                    throw new Exception("삭제하는 아이템과 정보가 일치하지 않음");
                }

                if(false == storage.PopItem(deleteItem))
                {
                    throw new Exception("인벤토리에서 아이템을 삭제하지 못함");
                }

                DB_InventoryUnit deleteUnit = deleteItem.ConvertInventoryUnit();
                int ret = await DatabaseHandler.GameDB.DeleteItem(storage_id, deleteUnit);
                if (ret == 0)
                {
                    throw new Exception("데이터베이스에서 아이템을 삭제하지 못함");
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[DeleteItem] : {e.Message.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 박스 인벤토리에서 이동할 경우
        /// </summary>
        public async Task<bool> MoveItem(int objectId, PS_ItemInfo moveInfo, int gridX, int gridY)
        {
            try
            {
                BoxObject box = ObjectManager.Instance.Find<BoxObject>(objectId);
                ItemObject moveItem = ObjectManager.Instance.Find<ItemObject>(moveInfo.ObjectId);

                if (box == null)
                {
                    throw new Exception("박스가 존재하지 않음");
                }

                if (false == box.storage.PopItem(moveItem))
                {
                    throw new Exception("박스 아이템을 제거하지 못함");
                }

                if (false == this.storage.PushItem(moveItem))
                {
                    throw new Exception("인벤토리에서 아이템을 삽입하지 못함");
                }

                DB_InventoryUnit moveUnit = moveItem.ConvertInventoryUnit();
                int ret = await DatabaseHandler.GameDB.InsertItem(storage_id, moveUnit);
                if (ret == 0)
                {
                    throw new Exception("데이터베이스에서 아이템을 삭제하지 못함");
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[MoveItem] : {e.Message.ToString()}");
                return false;
            }
        }

        public async Task<bool> MergeItem(ItemObject mergedItem, ItemObject combinedItem, int mergeNumber)
        {
            try
            {

                DB_InventoryUnit oldMergedUnit = mergedItem.ConvertInventoryUnit();
                DB_InventoryUnit oldCombinedUnit = combinedItem.ConvertInventoryUnit();

                if(false == storage.MergeItem(mergedItem, combinedItem, mergeNumber))
                {
                    throw new Exception("인벤토리에서 아이템 머지 실패");
                }

                bool combinedIsDelete = (storage.ScanItem(combinedItem) == -1) ? true : false;
                await DatabaseHandler.GameDB.MergeItem(storage_id, oldMergedUnit, mergedItem.ConvertInventoryUnit(), false, oldCombinedUnit, combinedItem.ConvertInventoryUnit());


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[MergeItem] : {e.Message.ToString()}");
                return false;
            }
        }

        public async Task<ItemObject> DevideItem(ItemObject totalItem, int gridX, int gridY, int rotation, int devideNumber)
        {
            try
            {

                DB_InventoryUnit totalMergedUnit = totalItem.ConvertInventoryUnit();

                ItemObject newItem = storage.DevideItem(totalItem, gridX, gridY, rotation, devideNumber);
                if (newItem == null)
                {
                    throw new Exception("인벤토리에서 아이템 나누기 실패");
                }

                await DatabaseHandler.GameDB.DevideItem(storage_id, totalMergedUnit, totalItem.ConvertInventoryUnit(), newItem.ConvertInventoryUnit());

                return newItem;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[MergeItem] : {e.Message.ToString()}");
                return null;
            }
        }

        public IEnumerable<PS_ItemInfo> GetInventoryItems()
        {
            List<PS_ItemInfo> infos = new List<PS_ItemInfo>();
            foreach(ItemObject item in storage.items)
            {
                infos.Add(item.Info);
            }
            return infos;
        }

        public void ClearInventory()
        {
            foreach (ItemObject item in storage.items)
            {
                item.DestroyItem();
            }
        }
    }
}
