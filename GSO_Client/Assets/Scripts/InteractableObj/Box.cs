using Google.Protobuf.Protocol;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : InteractableObject
{
    private OtherInventoryUI invenUI;
    private Vector2Int size;
    private double weight;

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

    public void SetBox(int x, int y, double weight)
    {
        this.size.x = x;
        this.size.y = y;
        this.weight = weight;
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
            InventoryController.invenInstance.SendLoadInvenPacket(0);  //플레이어는 실패 가능성이 없기에 플레이어 인벤 먼저
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
