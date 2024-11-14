using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverySlot1 : RecoverySlot
{
    public override void Init()
    {
        base.Init();
        slotId = 5;
        equipType = ItemType.Recovery;
    }
}
