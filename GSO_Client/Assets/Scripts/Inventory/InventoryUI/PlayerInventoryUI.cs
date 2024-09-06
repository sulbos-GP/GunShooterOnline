using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryUI : InventoryUI
{
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI moneyText; //돈에 대한 정보가 나올시 추가
                                      //
    public int bagLv = 0;

    public void WeightTextSet(double GridWeigt, double limitWeight)
    {
        weightText.text = $"WEIGHT \n {GridWeigt} / {limitWeight}"; 
    }

    private void Awake()
    {

    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    /// <summary>
    /// 핸들러에서 불려질 함수
    /// </summary>
    public override void InventorySet()
    {
        base.InventorySet();

        Vector2Int curSize = Vector2Int.zero;
        double limitWeight = 0;
        switch (bagLv)
        {
            case 0: curSize = BagSize.level0; limitWeight = 5f; break;
            case 1: curSize = BagSize.level1; limitWeight = 15f; break;
            case 2: curSize = BagSize.level2; limitWeight = 20f; break;
            case 3: curSize = BagSize.level3; limitWeight = 40f; break;
        }

        //생성된 그리드를 초기세팅하고 들어있는 아이템
        instantGrid.InitializeGrid(curSize, limitWeight); // 가방의 크기로 바꿀것
    }

}
