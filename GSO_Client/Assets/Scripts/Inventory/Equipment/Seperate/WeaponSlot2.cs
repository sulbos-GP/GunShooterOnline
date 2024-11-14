using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot2 : EquipSlotBase
{
    public override void Init()
    {
        base.Init();
        slotId = 2;
        equipType = ItemType.Weapon;
    }
}
