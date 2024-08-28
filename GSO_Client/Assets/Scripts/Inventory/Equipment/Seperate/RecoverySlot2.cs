using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot2 : EquipSlot
{
    private void Awake()
    {
        slotId = 6;
        allowedItemType = ItemType.Recovery;
    }

    protected override void ApplyItemEffects(ItemObject item)
    {
        base.ApplyItemEffects(item);
    }

    protected override void RemoveItemEffects(ItemObject item)
    {
        base.RemoveItemEffects(item);
    }
}
