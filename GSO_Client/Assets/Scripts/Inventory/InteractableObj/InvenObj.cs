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

    //��Ŷ���� ���� �ٲ� �κ������� �ݿ�

    //�÷��̾ ��ȣ�ۿ� Ű�� �������� ����� ����
    public override void Interact()
    {
        //������ �ش� ������Ʈ�� id�� ��Ŷ���� ����
        //�׷��� ���� �κ��丮�� �����ͷ� �κ��丮 ����

        //�ӽ÷� ���⿡�� �����͸� ����
        otherInvenUI.invenData = invenData;

        inventoryUI.invenUIControl();
    }
}
