using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Interface
{
    public interface IGameDatabase
    {

        /// <summary>
        /// 플레이어 장비 로드
        /// </summary>
        public Task<int> GetStorageItemId(int storage_id);

        /// <summary>
        /// 플레이어 장비 로드
        /// </summary>
        public Task<IEnumerable<Gear>> LoadGear(int uid);

        /// <summary>
        /// 가방 인벤토리 로드
        /// </summary>
        public Task<IEnumerable<InventoryUnit>> LoadInventory(int storage_id);

        /// <summary>
        /// 인벤토리에 아이템 삽입
        /// </summary>
        public Task InsertItem();

        /// <summary>
        /// 인벤토리에서 아이템 제거
        /// </summary>
        public Task DeleteItem();

        /// <summary>
        /// 아이템 이동
        /// </summary>
        public Task MoveItem();

    }
}
