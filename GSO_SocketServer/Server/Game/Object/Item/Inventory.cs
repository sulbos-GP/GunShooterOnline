using Server.Database.Game;
using Server.Database.Handler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDB;

namespace Server.Game
{

    public class Inventory : GameObject
    {
        public Player owner;                //소유자 (플레이어)
        public int storage_id = 0;          //가방 아이디

        public Storage storage = new Storage();

        /// <summary>
        /// 처음 접속한 이후 데이터베이스의 인벤토리 데이터 가져오기
        /// </summary>
        public Inventory(Player owner, int storage_id)
        {
            this.owner = owner;
            InitInventory(storage_id);
            LoadInventory().Wait();
        }

        /// <summary>
        /// 새로운 가방을 장착 시
        /// </summary>
        public void MakeInventory(int storage_id)
        {
            if(storage.ItemCount == 0)
            {
                //가방 처음 생성
                InitInventory(storage_id);
            }
            else
            {
                //기존에 아이템이 있을 경우
            }
        }

        /// <summary>
        /// 가방에 따른 인벤토리 초기화
        /// </summary>
        public void InitInventory(int storage_id)
        {
            try
            {
                //장비에 착용되어 있는 가방을 불러옴d
                ItemObject backpackItem = owner.gear.GetPartItem(EGearPart.Backpack);

                int scaleX = 0;
                int scaleY = 0;
                double weight = 0.0;
                if(backpackItem == null)
                {
                    //가방이 없다면 기본 제공
                    scaleX = 2;
                    scaleY = 3;
                    weight = 5.0;
                }
                else
                {
                    //마스터 테이블의 아이템 데이터 불러와서 가방의 정보 얻기
                    DB_ItemBackpack backpackData = DatabaseHandler.Context.ItemBackpack.Get(backpackItem.ItemId);
                    scaleX = backpackData.total_scale_x;
                    scaleY = backpackData.total_scale_y;
                    weight = backpackData.total_weight;
                }
                storage.Init(scaleX, scaleY, weight);

                this.storage_id = storage_id;
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
                IEnumerable<DB_ItemUnit> units = await DatabaseHandler.GameDB.LoadInventory(this.storage_id);

                if (units == null)
                {
                    return;
                }

                foreach (DB_ItemUnit unit in units)
                {
                    ItemObject newItem = new ItemObject(owner.Id, unit);
                    if (false == storage.InsertItem(newItem))
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

        public async Task<bool> InsertItem(ItemObject item, GameDB database, IDbTransaction transaction = null)
        {

            int ret = await database.InsertItem(storage_id, item, transaction);
            if (ret == 0)
            {
                throw new Exception("데이터베이스에서 아이템을 삽입하지 못함");
            }
            return true;
        }

        /// <summary>
        /// 자신의 인벤토리에 있는 아이템 삭제
        /// </summary>
        public async Task<bool> DeleteItem(ItemObject item, GameDB database, IDbTransaction transaction = null)
        {
            DB_ItemUnit unit = item.Unit;
            int ret = await database.DeleteItem(storage_id, unit, transaction);
            if (ret == 0)
            {
                throw new Exception("데이터베이스에서 아이템을 삭제하지 못함");
            }
            return true;
        }

        public async Task<bool> UpdateItem(ItemObject oldItem, ItemObject newItem, GameDB database, IDbTransaction transaction = null)
        {
            int ret = await database.UpdateItemAttributes(newItem, transaction);
            if (ret == 0)
            {
                throw new Exception("인벤토리에서 아이템 업데이트 안됨");
            }

            ret = await database.UpdateItem(storage_id, oldItem, newItem, transaction);
            if (ret == 0)
            {
                throw new Exception("인벤토리에서 아이템 업데이트 안됨");
            }
            return true;
        }

        public async Task<bool> UpdateItemAttributes(ItemObject item, GameDB database, IDbTransaction transaction = null)
        {
            int ret = await database.UpdateItemAttributes(item, transaction);
            if (ret == 0)
            {
                throw new Exception("인벤토리에서 아이템 속성 업데이트 안됨");
            }
            return true;
        }

        public void ClearInventory()
        {
            storage.ClearStorage();
        }
    }
}
