using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagSlot : EquipSlotBase
{
    public override void Init()
    {
        slotId = 4;
        equipType = ItemType.Bag;
    }
}
