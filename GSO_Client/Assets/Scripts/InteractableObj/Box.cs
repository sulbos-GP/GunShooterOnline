using Google.Protobuf.Protocol;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : InteractableObject
{
    private OtherInventoryUI invenUI;

    public Vector2Int size;
    public double weight;

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
        size.x = x;
        size.y = y;
        this.weight = weight;
    }

    protected override void SetTriggerSize()
    {
        CircleCollider2D Collider = GetComponent<CircleCollider2D>();
        Collider.radius = interactRange;
    }

 
    [ContextMenu("box interact")]
    public override void Interact()
    {
        if (interactable)
        {
            InventoryPacket.SendLoadInvenPacket(0);
            InventoryPacket.SendLoadInvenPacket(objectId);
        }
        else
        {
            Debug.Log("불가");
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
