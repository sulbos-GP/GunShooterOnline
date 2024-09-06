using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot3 : EquipSlot
{
    public QuickSlot targetSlot;

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
        slotId = 7;
        allowedItemType = ItemType.Recovery;
    }
    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        Debug.Log($"�Ҹ�ǰ3 : {item.item_name} ����");
        targetSlot.SetSlot(item);
        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"�Ҹ�ǰ3 ������ ����");
        targetSlot.ResetSlot();
        return true;
    }
}
