using Google.Protobuf.Protocol;
using Pipelines.Sockets.Unofficial.Buffers;
using Server.Database.Data;
using Server.Database.Game;
using Server.Database.Handler;
using Server.Game;
using Server.Game.Object;
using Server.Game.Object.Item;
using System;
using System.Collections.Generic;
using System.Data;
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
                IEnumerable<DB_Unit> units = await DatabaseHandler.GameDB.LoadInventory(this.storage_id);

                if (units == null)
                {
                    return;
                }

                foreach (DB_Unit unit in units)
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
            DB_Unit unit = item.Unit;
            int ret = await database.DeleteItem(storage_id, unit, transaction);
            if (ret == 0)
            {
                throw new Exception("데이터베이스에서 아이템을 삭제하지 못함");
            }
            return true;
        }

        public async Task<bool> MergeItem(DB_Unit oldInvenUnit1, DB_Unit newInvenUnit1, DB_Unit oldInvenUnit2, DB_Unit newInvenUnit2)
        {
            using (var database = DatabaseHandler.GameDB)
            {

                using (var transaction = database.GetConnection().BeginTransaction())
                {
                    try
                    {

                        if (newInvenUnit1.attributes.amount > 0)
                        {
                            //인벤토리 아이템이 증가
                            //인벤토리 아이템이 감소
                            if (0 == await database.UpdateItem(storage_id, oldInvenUnit1, newInvenUnit1, transaction))
                            {
                                throw new Exception("인벤토리에서 아이템 업데이트 안됨");
                            }
                        }
                        else
                        {
                            //인벤토리 아이템이 삭제
                            if (0 == await database.DeleteItem(storage_id, oldInvenUnit1, transaction))
                            {
                                throw new Exception("인벤토리에서 아이템 삭제가 안됨");
                            }
                        }

                        if (newInvenUnit2.attributes.amount > 0)
                        {
                            //인벤토리 아이템이 증가
                            //인벤토리 아이템이 감소
                            if (0 == await database.UpdateItem(storage_id, oldInvenUnit2, newInvenUnit2, transaction))
                            {
                                throw new Exception("인벤토리에서 아이템 업데이트 안됨");
                            }
                        }
                        else
                        {
                            //인벤토리 아이템이 삭제
                            if (0 == await database.DeleteItem(storage_id, oldInvenUnit2, transaction))
                            {
                                throw new Exception("인벤토리에서 아이템 삭제가 안됨");
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[MergeItem] : {e.Message.ToString()}");
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public async Task<bool> MergeItem(DB_Unit oldInvenUnit, DB_Unit newInvenUnit)
        {
            using (var database = DatabaseHandler.GameDB)
            {

                using (var transaction = database.GetConnection().BeginTransaction())
                {
                    try
                    {

                        if (newInvenUnit.attributes.amount > 0)
                        {
                            //인벤토리 아이템이 증가
                            //인벤토리 아이템이 감소
                            if (0 == await database.UpdateItem(storage_id, oldInvenUnit, newInvenUnit, transaction))
                            {
                                throw new Exception("인벤토리에서 아이템 업데이트 안됨");
                            }
                        }
                        else
                        {
                            //인벤토리 아이템이 삭제
                            if (0 == await database.DeleteItem(storage_id, oldInvenUnit, transaction))
                            {
                                throw new Exception("인벤토리에서 아이템 삭제가 안됨");
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[MergeItem] : {e.Message.ToString()}");
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public async Task<bool> DevideItem(DB_Unit oldInvenUnit, DB_Unit newInvenUnit, DB_Unit newDevideUnit)
        {
            using (var database = DatabaseHandler.GameDB)
            {

                using (var transaction = database.GetConnection().BeginTransaction())
                {
                    try
                    {

                        if (newInvenUnit.attributes.amount > 0)
                        {
                            //인벤토리 아이템이 감소
                            if (0 == await database.UpdateItem(storage_id, oldInvenUnit, newInvenUnit, transaction))
                            {
                                throw new Exception("인벤토리에서 아이템 업데이트 안됨");
                            }
                        }
                        else
                        {
                            //인벤토리 아이템이 삭제
                            if (0 == await database.DeleteItem(storage_id, oldInvenUnit, transaction))
                            {
                                throw new Exception("인벤토리에서 아이템 삭제가 안됨");
                            }
                        }

                        //인벤토리에 삽입
                        //if (0 == await database.InsertItem(storage_id, newDevideUnit, transaction))
                        //{
                        //    throw new Exception("인벤토리에서 아이템 삭제가 안됨");
                        //}

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[DevideItem] : {e.Message.ToString()}");
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public async Task<bool> DevideItem(bool isSource, DB_Unit oldInvenUnit, DB_Unit newInvenUnit)
        {
            using (var database = DatabaseHandler.GameDB)
            {

                using (var transaction = database.GetConnection().BeginTransaction())
                {
                    try
                    {

                        if(isSource)
                        {
                            if (newInvenUnit.attributes.amount > 0)
                            {
                                //인벤토리 아이템이 증가
                                //인벤토리 아이템이 감소
                                if (0 == await database.UpdateItem(storage_id, oldInvenUnit, newInvenUnit, transaction))
                                {
                                    throw new Exception("인벤토리에서 아이템 업데이트 안됨");
                                }
                            }
                            else
                            {
                                //인벤토리 아이템이 삭제
                                if (0 == await database.DeleteItem(storage_id, oldInvenUnit, transaction))
                                {
                                    throw new Exception("인벤토리에서 아이템 삭제가 안됨");
                                }
                            }
                        }
                        else
                        {
                            //if (0 == await database.InsertItem(storage_id, oldInvenUnit, transaction))
                            //{
                            //    throw new Exception("인벤토리에서 아이템 삽입이 안됨");
                            //}
                        }


                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[DevideItem] : {e.Message.ToString()}");
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public void ClearInventory()
        {
            storage.ClearStorage();
        }
    }
}
