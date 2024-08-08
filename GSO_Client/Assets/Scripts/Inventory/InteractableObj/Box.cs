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

    //��Ŷ���� ���� �ٲ� �κ������� �ݿ�

    //�÷��̾ ��ȣ�ۿ� Ű�� �������� ����� ����
    [ContextMenu("box interact")]
    public override void Interact()
    {
        InventoryController.invenInstance.invenUIControl();
        invenUI.invenData = GetComponent<OtherInventory>().InputInvenData;
        invenUI.InventorySet();
        
    }
}
