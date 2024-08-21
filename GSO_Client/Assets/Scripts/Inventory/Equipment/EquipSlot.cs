using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EquipSlot : MonoBehaviour
{
    public ItemType allowedItemType; // �� ���Կ� ���Ǵ� ������ ����

    public ItemObject equippedItem; // ���� ������ ������

    //���� ���Կ� �������� ��ġ�Ǿ��� ���
    public void EquipItem(ItemObject item)
    {
        //��ġ�� ���� ���� ���
        equippedItem = item;

        if (equippedItem.curItemGrid != null)
        {
            equippedItem.curItemGrid.RemoveItemFromItemList(equippedItem);
            equippedItem.curItemGrid = null;
        }

        equippedItem.backUpEquipSlot = this;


        //�߾ӿ� ��ġ �� ������ ũ�⸸ŭ ũ�� ����
        equippedItem.transform.SetParent(transform);
        //equippedItem.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta; üũ�� �ʿ�
        equippedItem.GetComponent<RectTransform>().localPosition = Vector3.zero;

        ApplyItemEffects(equippedItem);


        //���� ��Ŷ ����?
    }



    // ������ ���� ����(������ ������ Ŭ��)
    public void UnequipItem()
    {
        if (equippedItem != null)
        {
            RemoveItemEffects(equippedItem);
            //���� ������ ������ ���

            //���� ��Ŷ ����?

            equippedItem = null;
        }
    }

    private void ApplyItemEffects(ItemObject item)
    {
        //���� ��� ���� ���� ������ �� ������ ����

        //�� �ϰ�� �÷��̾��� ���� ����

        //�����ϰ�� �׸����� ������ ����
        // ������ Ŀ����� -> ũ�� ����
        // ������ ��������� -> ũ�⸦ ���ϼ� �ִ���(������ĭ�� ��ġ����) �˻��� �����ϸ� ����

        //ȸ����ǰ�� ��� �����Կ� ����Ͽ� ������ ��ư Ŭ���� �ش� �������� ȿ�� �ο�
    }

    private void RemoveItemEffects(ItemObject item)
    {
        //���� ��� ���� ���� ������ �� ������ ����

        //�� �ϰ�� �÷��̾��� ���� ����ġ

        //�����ϰ�� ������ �⺻ũ�⺸�� �������� ���� ��������� ���� �Ұ�

        //ȸ����ǰ�� ��� �ش� �������� ������ ����
    }
}
