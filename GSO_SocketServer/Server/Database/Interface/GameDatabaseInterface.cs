using Server.Database.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Data;
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
        public Task<IEnumerable<DB_Gear>> LoadGear(int uid);

        /// <summary>
        /// 가방 인벤토리 로드
        /// </summary>
        public Task<IEnumerable<DB_StorageUnit>> LoadInventory(int storage_id);

        /// <summary>
        /// 인벤토리에 아이템 삽입
        /// </summary>
        public Task<int> InsertItem(int storage_id, DB_StorageUnit unit, IDbTransaction transaction = null);

        /// <summary>
        /// 인벤토리에서 아이템 제거
        /// </summary>
        public Task<int> DeleteItem(int storage_id, DB_StorageUnit unit, IDbTransaction transaction = null);

    }
}
