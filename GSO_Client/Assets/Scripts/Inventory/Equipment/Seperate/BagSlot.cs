using Google.Protobuf.Protocol;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagSlot : EquipSlot
{
    private void Awake()
    {
        Init();
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Init()
    {
        slotId = 4;
        allowedItemType = ItemType.Bag;
    }

    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);

        // ������ Ŀ����� -> ũ�� ����
        // ������ ��������� -> ũ�⸦ ���ϼ� �ִ���(������ĭ�� ��ġ����) �˻��� �����ϸ� ����

        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;
        Debug.Log($"���� : {item.item_name} ����");

        //�������� ������ ���̵� ���� ������ �� �����͸� �ҷ���
        BagData targetBag = new BagData();
        bool isSuccess = BagDB.bagDB.TryGetValue(item.itemId, out targetBag);
        if (!isSuccess)
        {
            Debug.Log("�ش� ���̵��� ������ �������� ����");
            return false;
        }

        

        if (playerUI.instantGrid != null) //�κ��丮�� �����ִ� ���¿��� ��ȭ
        {
            if (playerUI.equipBag.item_id == targetBag.item_id) //���� ������ �����̶�� ��ȭ ���� ���� ó��
            {
                return true;
            }

            Vector2Int newSize = new Vector2Int(targetBag.total_scale_x, targetBag.total_scale_y);

            if (playerUI.equipBag.total_scale_x > newSize.x || playerUI.equipBag.total_scale_y > newSize.y)
            {
                isSuccess = playerUI.instantGrid.CheckAvailableToChange(newSize);
                if (!isSuccess || targetBag.total_weight < playerUI.instantGrid.GridWeight)
                {
                    Debug.Log("�ش� ���濡 ������ ����");
                    return false;
                }
            }

            playerUI.equipBag = targetBag;
            //���� ������ ũ�ų� �׽�Ʈ ����� �׸����� ������ ������Ʈ
            playerUI.instantGrid.UpdateGridObject(newSize, targetBag.total_weight);
        }
        else //���ʿ� �����Ǿ� ��� �ҷ��ö�(�κ��丮�� �������� ����)
        {
            playerUI.equipBag = targetBag;
        }

        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item); //������ �κ��丮�� ���� ���¿����� ȣ���

        Debug.Log($"���� ������ ����");
        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;

        BagData basicBag = new BagData();
        bool isSuccess = BagDB.bagDB.TryGetValue(-1,out basicBag);
        if (!isSuccess )
        {
            Debug.Log("�⺻ ���� �����͸� ã�� ����");
            return false;
        }

        if (playerUI.instantGrid.GridWeight > 5)
        {
            Debug.Log("�ش� ������ ���� �Ҽ� ����");
            return false;
        }

        playerUI.equipBag = basicBag;
        Vector2Int basicSize = new Vector2Int(basicBag.total_scale_x, basicBag.total_scale_y);
        playerUI.instantGrid.UpdateGridObject(basicSize, basicBag.total_weight);
        return true;
    }
}
