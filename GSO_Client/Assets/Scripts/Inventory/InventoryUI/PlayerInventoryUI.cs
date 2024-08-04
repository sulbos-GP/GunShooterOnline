using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryUI : InventoryUI
{
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI moneyText; //���� ���� ������ ���ý� �߰�
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
        //���潽���� �ٲ����� �κ��丮 ���� �׸����� ũ��� ������ ������ ũ�⸦ ����

    }
}
