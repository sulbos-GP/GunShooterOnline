using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot3 : RecoverySlot
{
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Init()
    {
        base.Init();
        slotId = 7;
        allowedItemType = ItemType.Recovery;
        if (targetSlot != null)
        {
            targetSlot = ItemQuickSlotsParent.GetChild(2).GetComponent<IQuickSlot>();
            targetSlot.SlotId = 3;
        }

    }
    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        Debug.Log($"�Ҹ�ǰ3 : {item.item_name} ����");   
        if (targetSlot != null)
        {
            targetSlot.SetSlot(item);
        }
        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"�Ҹ�ǰ3 ������ ����");
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
