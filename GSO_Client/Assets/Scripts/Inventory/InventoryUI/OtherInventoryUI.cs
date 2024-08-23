using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OtherInventoryUI : InventoryUI
{

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void InventorySet()
    {
        base.InventorySet();

        //생성된 그리드를 초기세팅하고 들어있는 아이템
        instantGrid.InitializeGrid(new Vector2Int(6, 7)); // 가방의 크기로 바꿀것
    }
}
