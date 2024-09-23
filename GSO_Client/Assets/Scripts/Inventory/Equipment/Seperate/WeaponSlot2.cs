using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot2 : EquipSlot
{
    private void Awake()
    {
        Init();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Init()
    {
        slotId = 2;
        allowedItemType = ItemType.Weapon;
    }

    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        Debug.Log($"무기2 : {item.item_name} 장착");

        Data_master_item_weapon equipGun = new Data_master_item_weapon();
        equipGun = Data_master_item_weapon.GetData(item.itemId);
        if (equipGun == null)
        {
            Debug.Log("해당 아이템의 아이디를 가진 총이 없음");
            return false;
        }

        Managers.Object.MyPlayer.GetComponent<Unit>().SetSlot2 = equipGun;

        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"무기2 아이템 해제");
        Managers.Object.MyPlayer.GetComponent<Unit>().SetSlot2 = null;
        return true;
    }
}
