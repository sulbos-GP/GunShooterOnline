using Google.Protobuf.Protocol;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : InteractableObject
{
    private OtherInventoryUI invenUI;
    public bool interactable;
    private void Awake()
    {
        Init();

    }
    protected override void Init()
    {
        base.Init();
        interactable = true; 
        interactRange = 2;
        invenUI = InventoryController.invenInstance.otherInvenUI;
        SetTriggerSize();
    }

    protected override void SetTriggerSize()
    {
        CircleCollider2D Collider = GetComponent<CircleCollider2D>();
        Collider.radius = interactRange;
    }

    //패킷으로 받은 바뀐 인벤데이터 반영

    //플레이어가 상호작용 키를 눌렀을때 실행될 내용
    [ContextMenu("box interact")]
    public override void Interact()
    {
        if (interactable)
        {
            InventoryController.invenInstance.SendLoadInvenPacket(0);
            InventoryController.invenInstance.SendLoadInvenPacket(objectId);
        }
        else
        {
            Debug.Log("불가능");
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
