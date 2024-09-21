using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot1 : EquipSlot
{
    private void Awake()
    {
        Init();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override  void Init()
    {
        slotId = 1;
        allowedItemType = ItemType.Weapon;
    }


    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        Debug.Log($"����1 : {item.item_name} ����");

        Data_master_item_weapon equipGun = new Data_master_item_weapon();
        equipGun = Data_master_item_weapon.GetData(item.itemId);
        if (equipGun == null)
        {
            Debug.Log("�ش� �������� ���̵� ���� ���� ����");
            return false;
        }
        Managers.Object.MyPlayer.GetComponent<Unit>().SetSlot1 = equipGun;

        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"����1 ������ ����");
        Managers.Object.MyPlayer.GetComponent<Unit>().SetSlot1 = null;
        return true;
    }
}
