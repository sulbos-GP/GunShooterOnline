using Google.Protobuf.Protocol;
using Google.Protobuf.Reflection;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSlot : MonoBehaviour
{
    public int slotId;
    public ItemType allowedItemType; // �� ���Կ� ���Ǵ� ������ ����
    public ItemObject equippedItem; // ���� ������ ������
    private Vector2 originalItemSize = Vector2.zero;

    //���� ���Կ� �������� ��ġ�Ǿ��� ���
    public void EquipItem(ItemObject item)
    {
        //��ġ�� ���� ���� ���
        equippedItem = item;
        equippedItem.backUpParentId = slotId;


        //�߾ӿ� ��ġ �� ������ ũ�⸸ŭ ũ�� ����
        equippedItem.transform.SetParent(transform);
        SetEquipItemObj(item);

        ApplyItemEffects(equippedItem);


        //���� ��Ŷ ����?
    }

    public void SetEquipItemObj(ItemObject item)
    {
        item.itemData.rotate = 0;
        item.Rotate(0);
        originalItemSize = item.GetComponent<RectTransform>().rect.size;
        AdjustRectTransform(item);
        item.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    private void AdjustRectTransform(ItemObject item)
    {
        // 1�� RectTransform�� ũ�⸦ �������� 0.9�� �� ũ��
        Vector2 slotSize = GetComponent<RectTransform>().rect.size * 0.9f;

        // 2�� RectTransform�� ���� ũ��
        Vector2 itemSize = item.GetComponent<RectTransform>().rect.size;

        // ������ �°� 2�� RectTransform�� ũ�⸦ ����
        float widthRatio = slotSize.x / itemSize.x;
        float heightRatio = slotSize.y / itemSize.y;

        // ���� ���� ������ ũ�⸦ ����
        float minRatio = Mathf.Min(widthRatio, heightRatio);

        // minRatio�� 1���� ������ ũ�⸦ ���̰�, 1���� ũ�� ũ�⸦ �ø�
        if (minRatio != 1f)
        {
            item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize.x * minRatio);
            item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize.y * minRatio);
        }
    }



    // ������ ���� ����(������ ������ Ŭ��)
    public void UnEquipItem()
    {
        if (equippedItem != null)
        {
            RemoveItemEffects(equippedItem);
            //���� ������ ������ ���

            equippedItem.GetComponent<RectTransform>().sizeDelta = originalItemSize;
            originalItemSize = Vector2.zero;

            //���� ��Ŷ ����?

            equippedItem = null;
        }
    }

    //�̰� �ڵ鷯���� �ٷ����?
    protected virtual void ApplyItemEffects(ItemObject item)
    {
        //���� ��� ���� ���� ������ �� ������ ����

        //�� �ϰ�� �÷��̾��� ���� ����
        //���������� 0 : 0%, 1 : 15%, 2: 25%, 3 : 35%  , ����Ƚ�� 20ȸ ����

        //�����ϰ�� �׸����� ������ ����
        // ������ Ŀ����� -> ũ�� ����
        // ������ ��������� -> ũ�⸦ ���ϼ� �ִ���(������ĭ�� ��ġ����) �˻��� �����ϸ� ����
        // ���� ũ�� 0 : 2*3 , 1 : 3*4 , 2 : 5*5 , 3 : 7*6

        //ȸ����ǰ�� ��� �����Կ� �������� ������ ������ ����
    }

    protected virtual void RemoveItemEffects(ItemObject item)
    {
        //���� ��� ���� ���� ������ �� ������ ����

        //�� �ϰ�� �÷��̾��� ���� ����ġ

        //�����ϰ�� ������ �⺻ũ�⺸�� �������� ���� ��������� ���� �Ұ�

        //ȸ����ǰ�� ��� �ش� �������� ������ ����
    }

    public static EquipSlot GetEquipSlot(int objectId)
    {
        switch (objectId)
        {
            case 1: return InventoryController.invenInstance.Weapon1;
            case 2: return InventoryController.invenInstance.Weapon2;
            case 3: return InventoryController.invenInstance.Armor;
            case 4: return InventoryController.invenInstance.Bag;
            case 5: return InventoryController.invenInstance.Consume1;
            case 6: return InventoryController.invenInstance.Consume2;
            case 7: return InventoryController.invenInstance.Consume3;
            default: return null; // objectId�� ��ȿ���� ���� ��� null ��ȯ
        }
    }
}
