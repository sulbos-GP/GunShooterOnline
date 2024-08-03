using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : InteractableObject
{
    public InvenData invenData;

    private void Awake()
    {
        Init();

    }
    protected override void Init()
    {
        base.Init();
        interactRange = 2;

        SetTriggerSize();
    }

    protected override void SetTriggerSize()
    {
        CircleCollider2D Collider = transform.GetComponent<CircleCollider2D>();
        Collider.radius = interactRange;
    }

    //패킷으로 받은 바뀐 인벤데이터 반영

    //플레이어가 상호작용 키를 눌렀을때 실행될 내용
    public override void Interact()
    {
        //서버에 해당 오브젝트의 id를 패킷으로 전송
        //그래서 받은 인벤토리의 데이터로 인벤토리 형성
        C_LoadInventory packet = new C_LoadInventory();
        packet.PlayerId = 1; //임시 1로 고정, 나중에 오브젝트 매니저를 통해 플레이어의 id를 받아오기
        packet.InventoryId = 1;
        Managers.Network.Send(packet);
        Debug.Log("C_LoadInventory");

        InventoryController.invenInstance.invenUIControl();
    }
}
