using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot3 : RecoverySlot
{
    public override void Init()
    {
        base.Init();
        slotId = 7;
        equipType = ItemType.Recovery;
    }
}
