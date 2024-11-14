using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot : EquipSlotBase
{
    public IQuickSlot targetSlot;
    public Transform ItemQuickSlotsParent => UIManager.Instance.IQuickSlot;

    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        return true;
    }

    public override bool RemoveItemEffects()
    {
        base.RemoveItemEffects();
        return true;
    }

    public virtual void UpdateQuickSlotAmount(int amount)
    {

    }
}
