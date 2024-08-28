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

        //생성된 그리드를 초기세팅하고 들어있는 아이템
        instantGrid.InitializeGrid(new Vector2Int(5,5),15f); // 가방의 크기로 바꿀것
        
    }

    public void ChangeInventory()
    {
        //가방슬롯을 바꿨을때 인벤토리 내의 그리드의 크기와 아이템 슬롯의 크기를 증가

    }
}
