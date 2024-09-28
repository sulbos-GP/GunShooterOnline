using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot : EquipSlot
{
    public QuickSlot targetSlot;
    public Transform ItemQuickSlotsParent;

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Init()
    {
        base.Init();
        if(GameObject.Find("IQuickSlot") != null)
        {
            ItemQuickSlotsParent = GameObject.Find("IQuickSlot").transform;
        }
    }

    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        return true;
    }

    public virtual void UpdateQuickSlotAmount(int amount)
    {

    }
}
