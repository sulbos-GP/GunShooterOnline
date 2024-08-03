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

    //��Ŷ���� ���� �ٲ� �κ������� �ݿ�

    //�÷��̾ ��ȣ�ۿ� Ű�� �������� ����� ����
    public override void Interact()
    {
        //������ �ش� ������Ʈ�� id�� ��Ŷ���� ����
        //�׷��� ���� �κ��丮�� �����ͷ� �κ��丮 ����
        C_LoadInventory packet = new C_LoadInventory();
        packet.PlayerId = 1; //�ӽ� 1�� ����, ���߿� ������Ʈ �Ŵ����� ���� �÷��̾��� id�� �޾ƿ���
        packet.InventoryId = 1;
        Managers.Network.Send(packet);
        Debug.Log("C_LoadInventory");

        InventoryController.invenInstance.invenUIControl();
    }
}
