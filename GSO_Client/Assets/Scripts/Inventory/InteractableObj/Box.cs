using Google.Protobuf.Protocol;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : InteractableObject
{
    private InventoryController invenController;
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
    [ContextMenu("box interact")]
    public override void Interact()
    {
        //서버에 해당 오브젝트의 id를 패킷으로 전송
        //그래서 받은 인벤토리의 데이터로 인벤토리 형성
        if (GetComponent<OtherInventory>().InputInvenData.inventoryId == 0)
        {
            C_LoadInventory packet = new C_LoadInventory();
            packet.PlayerId = Managers.Object.MyPlayer.Id;
            packet.InventoryId = objectId;
            Managers.Network.Send(packet);
            Debug.Log($"C_LoadInventory, player : {packet.PlayerId}, inventory: {packet.InventoryId} ");
        }

        

        InventoryController.invenInstance.invenUIControl();
    }
}
