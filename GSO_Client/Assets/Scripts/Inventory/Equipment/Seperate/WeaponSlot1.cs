using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot1 : EquipSlotBase
{
    public override  void Init()
    {
        base.Init();
        slotId = 1;
        equipType = ItemType.Weapon;
    }

}
