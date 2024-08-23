using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
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
    
    public List<ItemData> items = new List<ItemData>();

    public ItemDB()
    {
        Init();
    }

    private void Init()
    {

        //아이템 코드 임시. 나중에 정상화 해야함
        ItemData pistol = new ItemData();
        pistol.objectId = 0;
        pistol.itemId = 1;
        pistol.pos.x = 0;
        pistol.pos.x = 0;
        pistol.rotate = 0;
        pistol.amount = 1;
        
        pistol.isItemConsumeable = false;
        pistol.item_name = "Pistol01";
        pistol.item_weight = 2.0f;
        pistol.item_type = ItemType.Weapon;
        pistol.item_string_value = 101;
        pistol.item_purchase_price = 400;
        pistol.item_sell_price = 100;
        pistol.width = 2;
        pistol.height = 2;
        pistol.item_searchTime = 2;

        ItemData ak47 = new ItemData();
        ak47.objectId = 0;
        ak47.pos.x = 0;
        ak47.pos.y = 0;
        ak47.rotate = 0;
        ak47.amount = 1;
        ak47.itemId = 2;
        ak47.isItemConsumeable = false;
        ak47.item_name = "Ak47";
        ak47.item_weight = 7.0f;
        ak47.item_type = ItemType.Weapon;
        ak47.item_string_value = 102;
        ak47.item_purchase_price = 2200;
        ak47.item_sell_price = 500;
        ak47.width = 4;
        ak47.height = 3;
        ak47.item_searchTime = 3;

        ItemData recoveryKit = new ItemData();
        recoveryKit.objectId = 0;
        recoveryKit.pos.x = 0;
        recoveryKit.pos.y = 0;
        recoveryKit.rotate = 0;
        recoveryKit.amount = 1;
        recoveryKit.itemId = 3;
        recoveryKit.isItemConsumeable = true;
        recoveryKit.item_name = "Recovery kit";
        recoveryKit.item_weight = 1.0f;
        recoveryKit.item_type = ItemType.Recovery;
        recoveryKit.item_string_value = 301;
        recoveryKit.item_purchase_price = 500;
        recoveryKit.item_sell_price = 120;
        recoveryKit.width = 2;
        recoveryKit.height = 2;
        recoveryKit.item_searchTime = 1;

        ItemData bandage = new ItemData();
        bandage.objectId = 0;
        bandage.pos.x = 0;
        bandage.pos.y = 0;
        bandage.rotate = 0;
        bandage.amount = 1;
        bandage.itemId = 4;
        bandage.isItemConsumeable = true;
        bandage.item_name = "Bandage";
        bandage.item_weight = 0.0f;
        bandage.item_type = ItemType.Recovery;
        bandage.item_string_value = 302;
        bandage.item_purchase_price = 100;
        bandage.item_sell_price = 20;
        bandage.width = 1;
        bandage.height = 1;
        bandage.item_searchTime = 1;


        ItemData pill = new ItemData();
        pill.objectId = 0;
        pill.pos.x = 0;
        pill.pos.y = 0;
        pill.rotate = 0;
        pill.amount = 1;
        pill.itemId = 5;
        pill.isItemConsumeable = true;
        pill.item_name = "Pill";
        pill.item_weight = 0.0f;
        pill.item_type = ItemType.Recovery;
        pill.item_string_value = 402;
        pill.item_purchase_price = 150;
        pill.item_sell_price = 40;
        pill.width = 1;
        pill.height = 1;
        pill.item_searchTime = 1;

        items.Add(pistol);
        items.Add(ak47);
        items.Add(recoveryKit);
        items.Add(bandage);
        items.Add( pill);









    }
}
