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

    //��Ŷ���� ���� �ٲ� �κ������� �ݿ�

    //�÷��̾ ��ȣ�ۿ� Ű�� �������� ����� ����
    [ContextMenu("box interact")]
    public override void Interact()
    {
        //������ �ش� ������Ʈ�� id�� ��Ŷ���� ����
        //�׷��� ���� �κ��丮�� �����ͷ� �κ��丮 ����
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
