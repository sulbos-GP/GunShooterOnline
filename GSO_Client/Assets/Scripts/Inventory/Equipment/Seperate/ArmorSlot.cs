using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSlot : EquipSlot
{
    private void Awake()
    {
        slotId = 3;
        allowedItemType = ItemType.Defensive;
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
