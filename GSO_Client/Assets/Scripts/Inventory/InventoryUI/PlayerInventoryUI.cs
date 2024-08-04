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
    private bool isInventorySet;
    
    protected override float InvenWeight
    {
        get { return invenWeight; }
        set 
        { 
            invenWeight = value;
            WeightTextSet();
        }
    }

    private void WeightTextSet()
    {
        weightText.text = $"WEIGHT \n {InvenWeight} / {invenData.limitWeight}";
    }

    protected override void Awake()
    {
        base.Awake();
        isInventorySet = false;
    }

    private void OnEnable()
    {
        if (!isInventorySet)
        {
            InventorySet();
            isInventorySet = true;
        }
    }

    public override void InventorySet()
    {
        invenData = Managers.Object.MyPlayer.GetComponent<PlayerInventory>().InputInvenData;
        base.InventorySet();
    }

    public void ChangeInventory()
    {
        //가방슬롯을 바꿨을때 인벤토리 내의 그리드의 크기와 아이템 슬롯의 크기를 증가

    }
}
