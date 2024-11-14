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
    public ItemData gearData
    {
        get => Managers.Object.MyPlayer.gearDict[slotId];
        set
        {
            Managers.Object.MyPlayer.gearDict[slotId] = value;
        }
    }
    public ItemObject equippedItem; // ���� ������ ������

    private Vector2 originalItemSize = Vector2.zero;

    protected virtual void OnDisable()
    {
        foreach (Transform child in transform)
        {
            // �ڽ� ��ü�� ����
            Managers.Resource.Destroy(child.gameObject);
        }
    }

    public virtual void Init()
    {

    }

    //�κ��丮�� ���� ����: �÷��̾� �� �߰� + ������ ������Ʈ ����
    public bool EquipItem(ItemObject item)
    {
        //todo �ε带 ���� �ҷ��� ���� ������ ���� �����Ǿ� �ִ� �����۰� ���Ͽ� ȿ�� ����
        if (!ApplyItemEffects(item.itemData))
        {
            Debug.Log("�������");
            return false;
        }

        //��ġ�� ���� ���� ���
        equippedItem = item;
        equippedItem.parentObjId = slotId;
        equippedItem.backUpParentId = slotId;


        //�߾ӿ� ��ġ �� ������ ũ�⸸ŭ ũ�� ����
        equippedItem.transform.SetParent(transform);
        SetEquipItemObj(item);
        
        return true;
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
        Vector2 slotSize = GetComponent<RectTransform>().rect.size * 0.9f;
        Vector2 itemSize = item.GetComponent<RectTransform>().rect.size;

        float widthRatio = slotSize.x / itemSize.x;
        float heightRatio = slotSize.y / itemSize.y;

        float minRatio = Mathf.Min(widthRatio, heightRatio);

        if (minRatio != 1f)
        {
            item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize.x * minRatio);
            item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize.y * minRatio);
        }
    }



    // ������ ���� ����
    public bool UnEquipItem()
    {
        if (equippedItem == null)
        {
            Debug.Log("�̹� ������ �������� ����");
            return false;
        }

        if (!RemoveItemEffects(equippedItem.itemData))
        {
            Debug.Log("���Ž���");
            return false;
        }
        //���� ������ ������ ���
        equippedItem.GetComponent<RectTransform>().sizeDelta = originalItemSize;
        originalItemSize = Vector2.zero;

        equippedItem = null;
        
        return true;
    }

    //�κ��丮�� ������ ���� ���� : �÷��̾��� gear�� ���� ���� + ���� Ȥ�� ������ ���� �Ķ���� ��ȭ ����
    public virtual bool ApplyItemEffects(ItemData item)
    {
        gearData = item;
        Debug.Log("������ ����");
        return true;
    }

    public virtual bool RemoveItemEffects(ItemData item)
    {
        gearData = null;
        Debug.Log("������ ����");
        return true;
    }
}
