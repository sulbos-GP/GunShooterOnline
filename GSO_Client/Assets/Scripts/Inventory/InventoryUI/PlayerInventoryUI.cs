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
                                      
    public int bagLv = 0;

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

    private void Awake()
    {
        InventorySet();
    }

    public override void InventorySet()
    {
        Debug.Log(Managers.Object.MyPlayer.gameObject.name);
        invenData = Managers.Object.MyPlayer.GetComponent<PlayerInventory>().InputInvenData;
        base.InventorySet();
    }

    public void ChangeInventory()
    {
        //���潽���� �ٲ����� �κ��丮 ���� �׸����� ũ��� ������ ������ ũ�⸦ ����

    }
}
