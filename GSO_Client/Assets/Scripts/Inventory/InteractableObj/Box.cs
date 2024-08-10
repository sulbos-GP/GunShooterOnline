using Google.Protobuf.Protocol;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : InteractableObject
{
    private InventoryController invenController;
    private OtherInventoryUI invenUI;

    private void Awake()
    {
        Init();

    }
    protected override void Init()
    {
        base.Init();
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
        InventoryController.invenInstance.invenUIControl();
        invenUI.invenData = GetComponent<OtherInventory>().InputInvenData;
        invenUI.InventorySet();
        
    }
}
