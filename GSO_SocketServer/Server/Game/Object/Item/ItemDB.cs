using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    internal class ItemDB
    {
        private static ItemDB instance = new ItemDB();
        public static ItemDB Instance
        {
            get
            {
                return instance;

            }
            set
            {
                instance = value;
            }
        }
        public Dictionary<int, ItemDataInfo> items = new Dictionary<int, ItemDataInfo>();

        public ItemDB()
        {
            Init();
        }

        private void Init()
        {

            //아이템 코드 임시. 나중에 정상화 해야함
            ItemDataInfo pistol = new ItemDataInfo();
            pistol.ItemId = 0;
            pistol.ItemPosX = 0;
            pistol.ItemPosY = 0;
            pistol.ItemRotate = 0;
            pistol.ItemAmount = 1;
            pistol.ItemCode = 1;
            pistol.IsItemConsumeable = false;
            pistol.ItemName = "Pistol01";
            pistol.ItemWeight = 2.0f;
            pistol.ItemType = ItemType.Weapon;
            pistol.ItemStringValue = 101;
            pistol.ItemPurchasePrice = 400;
            pistol.ItemSellPrice = 100;
            pistol.Width = 2;
            pistol.Height = 2;
            pistol.ItemSearchTime = 2;

            ItemDataInfo ak47 = new ItemDataInfo();
            ak47.ItemId = 0;
            ak47.ItemPosX = 0;
            ak47.ItemPosY = 0;
            ak47.ItemRotate = 0;
            ak47.ItemAmount = 1;
            ak47.ItemCode = 2;
            ak47.IsItemConsumeable = false;
            ak47.ItemName = "Ak47";
            ak47.ItemWeight = 7.0f;
            ak47.ItemType = ItemType.Weapon; ;
            ak47.ItemStringValue = 102;
            ak47.ItemPurchasePrice = 2200;
            ak47.ItemSellPrice = 500;
            ak47.Width = 4;
            ak47.Height = 3;
            ak47.ItemSearchTime = 3;

            ItemDataInfo recoveryKit = new ItemDataInfo();
            recoveryKit.ItemId = 0;
            recoveryKit.ItemPosX = 0;
            recoveryKit.ItemPosY = 0;
            recoveryKit.ItemRotate = 0;
            recoveryKit.ItemAmount = 1;
            recoveryKit.ItemCode = 3;
            recoveryKit.IsItemConsumeable = true;
            recoveryKit.ItemName = "Recovery kit";
            recoveryKit.ItemWeight = 1.0f;
            recoveryKit.ItemType = ItemType.Recovery ;
            recoveryKit.ItemStringValue = 301;
            recoveryKit.ItemPurchasePrice = 500;
            recoveryKit.ItemSellPrice = 120;
            recoveryKit.Width = 2;
            recoveryKit.Height = 2;
            recoveryKit.ItemSearchTime = 1;

            ItemDataInfo bandage = new ItemDataInfo();
            bandage.ItemId = 0;
            bandage.ItemPosX = 0;
            bandage.ItemPosY = 0;
            bandage.ItemRotate = 0;
            bandage.ItemAmount = 1;
            bandage.ItemCode = 4;
            bandage.IsItemConsumeable = true;
            bandage.ItemName = "Bandage";
            bandage.ItemWeight = 0.0f;
            bandage.ItemType = ItemType.Recovery;
            bandage.ItemStringValue = 302;
            bandage.ItemPurchasePrice = 100;
            bandage.ItemSellPrice = 20;
            bandage.Width = 1;
            bandage.Height = 1;
            bandage.ItemSearchTime = 1;


            ItemDataInfo pill = new ItemDataInfo();
            pill.ItemId = 0;
            pill.ItemPosX = 0;
            pill.ItemPosY = 0;
            pill.ItemRotate = 0;
            pill.ItemAmount = 1;
            pill.ItemCode = 5;
            pill.IsItemConsumeable = true;
            pill.ItemName = "Pill";
            pill.ItemWeight = 0.0f;
            pill.ItemType = ItemType.Recovery;
            pill.ItemStringValue = 402;
            pill.ItemPurchasePrice = 150;
            pill.ItemSellPrice = 40;
            pill.Width = 1;
            pill.Height = 1;
            pill.ItemSearchTime = 1;

            items.Add(pistol.ItemCode, pistol);
            items.Add(ak47.ItemCode, ak47);
            items.Add(recoveryKit.ItemCode, recoveryKit);
            items.Add(bandage.ItemCode, bandage);
            items.Add(pill.ItemCode, pill);









        }
    }
}
