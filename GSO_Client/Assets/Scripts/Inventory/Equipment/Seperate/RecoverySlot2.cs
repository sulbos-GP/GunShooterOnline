using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot2 : RecoverySlot
{
    public override void Init()
    {
        base.Init();
        slotId = 6;
        equipType = ItemType.Recovery;
    }

}
