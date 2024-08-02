using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : Inventory
{
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI moneyText; //���� ���� ������ ���ý� �߰�

    protected override float InvenWeight
    {
        get { return invenWeight; }
        set { 
            invenWeight = value;
            WeightTextSet();
        }
    }

    private void WeightTextSet()
    {
        weightText.text = $"WEIGHT \n {InvenWeight} / {limitWeight}";
    }

    //������ ������ �׶� ����
    public int bagLv = 0;

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        C_LoadInventory packet = new C_LoadInventory();
        //packet.PlayerId = Managers.Object.MyPlayer.Id; //�÷��̾ ���� ���¶� �÷��̾� ���̵� ���ҷ���
        //packet.InventoryId = packet.PlayerId;
        packet.PlayerId = packet.InventoryId = 1;//Managers.Object.MyPlayer.Id;
        Managers.Network.Send(packet);
        Debug.Log("C_LoadInventory����");
    }

    public void ChangeInventory()
    {
        //���潽���� �ٲ����� �κ��丮 ���� �׸����� ũ��� ������ ������ ũ�⸦ ����

    }
}
