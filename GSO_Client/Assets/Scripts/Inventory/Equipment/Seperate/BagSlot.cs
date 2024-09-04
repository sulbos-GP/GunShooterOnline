using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagSlot : EquipSlot
{
    private void Awake()
    {
        slotId = 4;
        allowedItemType = ItemType.Bag;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
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
