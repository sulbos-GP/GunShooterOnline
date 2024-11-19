using Google.Protobuf.Protocol;
using Google.Protobuf.Reflection;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
    public bool SetItemEquip(ItemObject item)
    {
        if (!ApplyItemEffects(item.itemData)) //�÷��̾��� ��� ��ųʸ��� �߰�
        {
            Debug.Log("�������");
            return false;
        }

        //������ ������Ʈ ��ȭ
        equipItemObj = item;
        equipItemObj.parentObjId = slotId;
        equipItemObj.backUpParentId = slotId;
        equipItemObj.transform.SetParent(transform);

        SetEquipItemObj(item);
        
        return true;
    }

    public void SetEquipItemObj(ItemObject item)
    {
        item.itemData.rotate = 0;
        item.Rotate(0);
        originalItemSize = item.GetComponent<RectTransform>().rect.size;
        SetItemObjSize(item);
        item.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    private void SetItemObjSize(ItemObject item)
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
    public bool UnsetItemEquip()
    {
        if (equipItemObj == null)
        {
            Debug.Log("�̹� ������ �������� ����");
            return false;
        }

        //���� ������ ������ ���
        equipItemObj.GetComponent<RectTransform>().sizeDelta = originalItemSize;
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
        InventoryController.Instance.SetEquipItem(slotId, item);
        return true;
    }

    public virtual bool RemoveItemEffects()
    {
        Debug.Log("������ ����");
        InventoryController.Instance.UnsetEquipItem(slotId);
        return true;
    }
}
