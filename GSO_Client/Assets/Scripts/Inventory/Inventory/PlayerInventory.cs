using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : Inventory
{
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI moneyText; //돈에 대한 정보가 나올시 추가

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

    //슬롯이 나오면 그때 개발
    public int bagLv = 0;

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        C_LoadInventory packet = new C_LoadInventory();
        //packet.PlayerId = Managers.Object.MyPlayer.Id; //플레이어가 없는 상태라 플레이어 아이디를 못불러옴
        //packet.InventoryId = packet.PlayerId;
        packet.PlayerId = packet.InventoryId = 1;//Managers.Object.MyPlayer.Id;
        Managers.Network.Send(packet);
        Debug.Log("C_LoadInventory전송");
    }

    public void ChangeInventory()
    {
        //가방슬롯을 바꿨을때 인벤토리 내의 그리드의 크기와 아이템 슬롯의 크기를 증가

    }
}
