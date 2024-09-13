using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveSlot : EquipSlot
{
    private void Awake()
    {
        Init();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Init()
    {
        slotId = 3;
        allowedItemType = ItemType.Defensive;
    }

    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        Debug.Log($"아머 : {item.item_name} 장착");

        //얘는 따로 수정할 요소 없음 기껏해야 피격시 내구도만 업데이트 하면 됨

        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"아머 아이템 해제");

        return true;
    }
}
