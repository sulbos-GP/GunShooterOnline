using Google.Protobuf.Protocol;
using Pipelines.Sockets.Unofficial.Buffers;
using Server.Database.Handler;
using Server.Game;
using Server.Game.Object;
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
    public class Inventory
    {

        //인벤토리를 생성하기위한 데이터
        //인벤토리 아이디 == 소유자 (인벤토리를 검색하기 위해)
        //무게 같은 그리드 크기이지만 무게가 다를 수 있다
        //GridData = 서버에서 오브젝트가 만들어진다

        public InvenDataInfo invenData = new InvenDataInfo();
        /*int32 inventoryId = 1;
        float limitWeight = 2;
        repeated GridDataInfo GridData = 3;*/
        public Dictionary<int,Grid> instantGrid = new Dictionary<int,Grid>(); //해당 인벤토리가 소유한 그리드

        public Player owner;                //소유자 (플레이어)
        public int storage_id = 0;          //가방 아이디
        public int storage_item_id = 0;     //가방의 아이템 아이디
        public double weight = 0.0;         

        public Grid InventoryGrid = new Grid();

        /// <summary>
        /// 인벤토리 생성 (지금은 storage_id가 반드시 있다고 가정)
        /// </summary>
        public Inventory()
        {
        }

        /// <summary>
        /// 인벤토리 처음 생성시 또는 가방을 장착시
        /// </summary>
        public void MakeInventory(Player owner, int storage_id)
        {
            this.owner = owner;

            InitInventory(storage_id).Wait();

            LoadInventory().Wait();
        }

        /// <summary>
        /// 가방에 따른 인벤토리 초기화
        /// </summary>
        public async Task InitInventory(int storage_id)
        {
            try
            {
                this.storage_id = storage_id;

                //가방(아이템)에 대한 정보 불러오기
                int storage_item_id = await DatabaseHandler.GameDB.GetStorageItemId(storage_id);
                this.storage_item_id = storage_item_id;

                //마스터 테이블의 아이템 데이터 불러와서 가방의 정보 얻기 (지금은 임시)
                //Backpack backpack = await DatabaseHandler.MasterDB.GetBackpackInfo(storage_item_id);
                Backpack backpack = new Backpack();

                //가방의 데이터를 채움?
                invenData.InventoryId = owner.Id;
                invenData.LimitWeight = backpack.limit_weight;

                //가방의 크기를 저장하는 곳은?

                InventoryGrid.gridData = new GridDataInfo()
                {
                    
                };

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

                IEnumerable<InventoryUnit> units = await DatabaseHandler.GameDB.LoadInventory(this.storage_id);

                if (units == null)
                {
                    return;
                }

                //인벤토리 그리드
                Grid newItem = new Grid();
                newItem.gridData = new GridDataInfo
                {
                    GridId = ++Grid.lastGridId,
                    GridSizeX = info.scale_x,
                    GridSizeY = info.scale_y,
                    GridPosX = unit.grid_x,
                    GridPosY = unit.grid_y,
                };

                foreach (InventoryUnit unit in units)
                {
                    
                    //마스터테이블에서 아이템 아이디를 이용하여 아이템 정보 가져오기
                    //TODO : 나중에는 마스터테이블을 미리 로드하여 메모리에 저장된 값을 불러와야함
                    ItemInfo info = await DatabaseHandler.MasterDB.GetItemInfo(unit.item_id);

                    //데이터베이스에서 여러개의 아이템
                    //아이디, 위치, 회전, 몇개(카운터)

                    //인벤토리 안에 넣고 싶은데 (위치랑 회전에 맞게)

                    ItemInfo

                    newItem.InsertItemDataInGridData();

                    newItem.ownerInventory = this;
                    newItem.SetGrid();

                    instantGrid.Add(newGrid.gridData.GridId, newGrid);
                    invenData.GridData.Add(newGrid.gridData);

                }
                

            }
            catch (Exception e)
            {
                Console.WriteLine($"[LoadInventory] : {e.Message.ToString()}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void InsertItem()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> DeleteItem(int objectId, InventoryUnit delete)
        {
            try
            {
                ItemObject item = ObjectManager.Instance.Find<ItemObject>(objectId);

                if (item == null)
                {
                    throw new Exception("삭제하려는 아이템이 존재하지 않음");
                }

                if(false == item.Equals(delete))
                {
                    throw new Exception("삭제하는 아이템과 정보가 일치하지 않음");
                }

                int ret = await DatabaseHandler.GameDB.DeleteItem(owner.uid, delete);
                if (ret == 0)
                {
                    throw new Exception("데이터베이스에서 삭제하지 못함");
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
        /// 
        /// </summary>
        public void MoveItem()
        {

        }
        

        public void MoveItem(ItemObject target, ItemDataInfo packetData, Grid targetGrid)
        {
            //아이템의 이름,아이디
            //전위치 -> 옮겨질 현위치
            //전회전 -> 현재 회전도
            //이전 그리드의 아이디. 현재 그리드의 아이디
            Console.WriteLine($"MoveItem Method\n" +
                $"Item = {target.itemDataInfo.ItemName}, id = {target.itemDataInfo.ItemId}\n" +
                $"pos : ({target.itemDataInfo.ItemPosX},{target.itemDataInfo.ItemPosY}) -> ({packetData.ItemPosX},{packetData.ItemPosY})\n" +
                $"rotate : {target.itemDataInfo.ItemRotate} -> {packetData.ItemRotate}\n" +
                $"grid : {target.ownerGrid.gridData.GridId} -> {targetGrid.gridData.GridId}");

            //그리드에서 아이템 및 아이템 데이터 삭제
            target.ownerGrid.DeleteItemFromSlot(target);
            target.ownerGrid.RemoveItemDataInGridData(target);
            //아이템의 소유그리드와 회전도 업데이트
            target.ownerGrid = targetGrid;
            target.ItemRotate = packetData.ItemRotate;
            
            //그리드에 아이템 및 아이템 데이터 삽입
            target.ownerGrid.PushItemIntoSlot(target, packetData.ItemPosX,packetData.ItemPosY);
            target.ownerGrid.InsertItemDataInGridData(target);
            target.ownerGrid.PrintInvenContents();
        }

        public void DeleteItem(int id)
        {
            //아이템 가져오기
            ItemObject target = ObjectManager.Instance.Find<ItemObject>(id);

            if (target == null)
            {
                return;
            }

            Console.WriteLine($"DeleteItem Method\n" +
                $"Item = {target.itemDataInfo.ItemName}, id = {target.itemDataInfo.ItemId}\n" +
                $"pos : ({target.itemDataInfo.ItemPosX},{target.itemDataInfo.ItemPosX} -> X)\n" +
                $"rotate : {target.itemDataInfo.ItemRotate} -> X\n" +
                $"grid : {target.ownerGrid.gridData.GridId} -> X");
            target.ownerGrid.DeleteItemFromSlot(target);

            target.ownerGrid.PrintInvenContents();
        }

        public ItemDataInfo DuplicateItemData(ItemDataInfo targetData)
        {
            ItemDataInfo newItemData = new ItemDataInfo();
            newItemData.ItemId = targetData.ItemId;
            newItemData.ItemPosX = targetData.ItemPosX;
            newItemData.ItemPosY = targetData.ItemPosY;
            newItemData.ItemRotate = targetData.ItemRotate;
            newItemData.ItemAmount = targetData.ItemAmount;
            newItemData.ItemCode = targetData.ItemCode;
            newItemData.IsItemConsumeable = targetData.IsItemConsumeable;
            newItemData.ItemName = targetData.ItemName;
            newItemData.ItemWeight = targetData.ItemWeight;
            newItemData.ItemType = targetData.ItemType;
            newItemData.ItemStringValue = targetData.ItemStringValue;
            newItemData.ItemPurchasePrice = targetData.ItemPurchasePrice;
            newItemData.ItemSellPrice = targetData.ItemSellPrice;
            newItemData.Width = targetData.Width;
            newItemData.Height = targetData.Height;
            newItemData.ItemSearchTime = targetData.ItemSearchTime;
            return newItemData;
        }
    }
}
