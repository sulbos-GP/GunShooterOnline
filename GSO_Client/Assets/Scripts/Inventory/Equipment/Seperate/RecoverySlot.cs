using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot : EquipSlot
{
    public QuickSlot targetSlot;


    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Init()
    {

    }

    public override bool ApplyItemEffects(ItemData item)
    {
        
        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {

        return true;
    }

    public virtual void UpdateQuickSlotAmount(int amount)
    {

    }
}
