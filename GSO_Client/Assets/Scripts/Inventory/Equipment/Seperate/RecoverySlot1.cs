using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot1 : RecoverySlot
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
        slotId = 5;
        allowedItemType = ItemType.Recovery;
    }

    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        Debug.Log($"�Ҹ�ǰ1 : {item.item_name} ����");

        targetSlot.SetSlot(item);

        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"�Ҹ�ǰ1 ������ ����");
        targetSlot.ResetSlot();
        return true;
    }

    public override void UpdateQuickSlotAmount(int amount)
    {
        base.UpdateQuickSlotAmount(amount);
        targetSlot.UpdateItemAmount(amount);
    }
}
