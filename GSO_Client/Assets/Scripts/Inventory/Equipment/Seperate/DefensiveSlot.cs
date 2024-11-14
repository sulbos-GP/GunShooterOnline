using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveSlot : EquipSlotBase
{
    public override void Init()
    {
        slotId = 3;
        equipType = ItemType.Defensive;
    }
}
