using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenObj : InteractableObject
{
    public InvenData invenData;
    public InventoryUI inventoryUI;
    public OtherInventory otherInvenUI;

    private void Awake()
    {
        Init();
        Interact();
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

        //임시로 여기에서 데이터를 전달
        otherInvenUI.invenData = invenData;

        inventoryUI.invenUIControl();
    }
}
