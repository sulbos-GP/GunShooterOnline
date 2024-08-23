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

    //��Ŷ���� ���� �ٲ� �κ������� �ݿ�

    //�÷��̾ ��ȣ�ۿ� Ű�� �������� ����� ����
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
            Debug.Log("�Ұ���");
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
