using Google.Protobuf.Protocol;
using Google.Protobuf.Reflection;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


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

        if (equippedItem.curItemGrid != null)
        {
            equippedItem.curItemGrid.RemoveItemFromItemList(equippedItem);
            equippedItem.curItemGrid = null;
            equippedItem.backUpItemGrid = null;
        }

        equippedItem.backUpEquipSlot = this;


        //�߾ӿ� ��ġ �� ������ ũ�⸸ŭ ũ�� ����
        equippedItem.transform.SetParent(transform);

        item.itemData.rotate = 0;
        item.Rotate(0);
        originalItemSize = item.GetComponent<RectTransform>().rect.size;
        AdjustRectTransform(item);
        equippedItem.GetComponent<RectTransform>().localPosition = Vector3.zero;

        ApplyItemEffects(equippedItem);


        //���� ��Ŷ ����?
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
    public void UnequipItem()
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
}
