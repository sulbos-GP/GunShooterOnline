using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    public static List<ItemData> items = new List<ItemData>();
    static ItemDB()
    {
        Init();
    }

    private static void Init()
    {

        //아이템 코드 임시. 나중에 정상화 해야함
        ItemData pistol = new ItemData();
        pistol.itemId = 1;
        pistol.isItemConsumeable = false;
        pistol.item_name = "Colt45";
        pistol.item_weight = 2.0f;
        pistol.item_type = ItemType.Weapon;
        pistol.item_string_value = 101;
        pistol.item_purchase_price = 400;
        pistol.item_sell_price = 100;
        pistol.width = 2;
        pistol.height = 2;
        pistol.item_searchTime = 2;

        ItemData armor = new ItemData();
        armor.itemId = 2;
        armor.isItemConsumeable = true;
        armor.item_name = "ak47";
        armor.item_weight = 7.0f;
        armor.item_type = ItemType.Weapon;
        armor.item_string_value = 201;
        armor.item_purchase_price = 1100;
        armor.item_sell_price = 550;
        armor.width = 4;
        armor.height = 2;
        armor.item_searchTime = 1;

        ItemData band = new ItemData();
        band.itemId = 11;
        band.isItemConsumeable = true;
        band.item_name = "Band";
        band.item_weight = 0.2f;
        band.item_type = ItemType.Recovery;
        band.item_string_value = 401;
        band.item_purchase_price = 100;
        band.item_sell_price = 200;
        band.width = 1;
        band.height = 1;
        band.item_searchTime = 0.5f;

        items.Add(pistol);
        items.Add(armor);
        items.Add(band);









    }
}
