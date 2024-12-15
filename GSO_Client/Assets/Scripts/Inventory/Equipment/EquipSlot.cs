using Google.Protobuf.Protocol;
using Google.Protobuf.Reflection;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EquipSlotBase : MonoBehaviour
{
    public int slotId;
    public ItemType equipType; // �� ���Կ� ���Ǵ� ������ ����
    public ItemObject equipItemObj; // ���� ������ �����ۿ�����Ʈ @@ �κ��丮�� ������������ ����
    private Vector2 originalItemSize = Vector2.zero;

    bool isInit = false;

    private void Awake()
    {
        if (!isInit) 
        {
            Init();
        }
    }
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
        isInit = true;
    }

    //�κ��丮�� ���� ����: �÷��̾� �� �߰� + ������ ������Ʈ ����
    public bool SetItemEquip(ItemObject item, bool isQuickInfo = false)
    {
        if (!isQuickInfo)
        {
            if (!ApplyItemEffects(item.itemData)) //�÷��̾��� ��� ��ųʸ��� �߰�
            {
                Debug.Log("�������");
                return false;
            }
        }

        equipItemObj = item;
        equipItemObj.parentObjId = slotId;
        equipItemObj.backUpParentId = slotId;
        item.transform.SetParent(transform);

        //������ ������Ʈ ��ȭ
        SetEquipItemObj(item);
        return true;
    }

    public void SetEquipItemObj(ItemObject item)
    {
        Managers.SystemLog.Message($"SetEquipItemObj");
        
        item.itemData.rotate = 0;
        item.Rotate(0);
        RectTransform itemRect = item.GetComponent<RectTransform>();
        originalItemSize = itemRect.localScale;
        SetItemObjSize(item);
        itemRect.localPosition = Vector3.zero;
        Managers.SystemLog.Message($"SetEquipItemObj �Ϸ�");
    }

    private void SetItemObjSize(ItemObject item)
    {
        Managers.SystemLog.Message($"SetItemObjSize");

        RectTransform slotRect = GetComponent<RectTransform>();
        RectTransform itemRect = item.GetComponent<RectTransform>();

        // ������ width�� height ��������
        float slotWidth = slotRect.rect.width;
        float slotHeight = slotRect.rect.height;

        // �������� width�� height ��������
        float itemWidth = itemRect.rect.width;
        float itemHeight = itemRect.rect.height;

        // �ʺ�� ���� ���� ���
        float widthRatio = slotWidth / itemWidth;
        float heightRatio = slotHeight / itemHeight;

        // �ּ� ���� ���� (���� ũ�⿡ �°� ���� ����)
        float scaleRatio = Mathf.Min(widthRatio, heightRatio);

        // �������� scale ���� (�ڽ� ��ü�� ����)
        itemRect.localScale = new Vector3(scaleRatio, scaleRatio, 1f);

        // ��ġ ���� ���� (�߾� ����)
        itemRect.anchoredPosition = Vector2.zero;

        Managers.SystemLog.Message($"SetItemObjSize: �Ϸ� (Slot: {slotWidth}x{slotHeight}, Item: {itemWidth}x{itemHeight}, Scale: {scaleRatio})");
    }



    // ������ ���� ����
    public bool UnsetItemEquip()
    {
        if (equipItemObj == null)
        {
            Debug.Log("�̹� ������ �������� ����");
            return false;
        }

        //���� ������ ������ ���
        equipItemObj.GetComponent<RectTransform>().localScale = originalItemSize;
        originalItemSize = Vector2.zero;

        if (!RemoveItemEffects()) //�÷��̾��� ��� ��ųʸ����� ����
        {
            Debug.Log("���Ž���");
            return false;
        }

        equipItemObj =null;
        return true;
    }

    //�κ��丮�� ������ ���� ���� : �÷��̾��� gear�� ���� ���� + ���� Ȥ�� ������ ���� �Ķ���� ��ȭ ����
    public virtual bool ApplyItemEffects(ItemData item)
    {
        Debug.Log("������ ����");
        if(!InventoryController.Instance.SetEquipItem(slotId, item))
        {
            return false;
        }
        return true;
    }

    public virtual bool RemoveItemEffects()
    {
        Debug.Log("������ ����");
        if (!InventoryController.Instance.UnsetEquipItem(slotId))
        {
            return false;
        }
        return true;
    }
}
