using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot2 : RecoverySlot
{

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Init()
    {
        base.Init();
        slotId = 6;
        allowedItemType = ItemType.Recovery;
        if (targetSlot != null)
        {
            targetSlot = ItemQuickSlotsParent.GetChild(1).GetComponent<QuickSlot>();
        }

    }
    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        Debug.Log($"소모품2 : {item.item_name} 장착");
        if (targetSlot != null)
        {
            targetSlot.SetSlot(item);
        }
        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"소모품2 아이템 해제");
        if (targetSlot != null)
        {
            targetSlot.ResetSlot();
        }
        return true;
    }
    public override void UpdateQuickSlotAmount(int amount)
    {
        base.UpdateQuickSlotAmount(amount);
        if (targetSlot != null)
        {
            targetSlot.UpdateItemAmount(amount);
        }
    }
}
