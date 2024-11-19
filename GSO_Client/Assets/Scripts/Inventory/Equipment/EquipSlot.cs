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
    public ItemType equipType; // 이 슬롯에 허용되는 아이템 유형
    public ItemObject equipItemObj; // 현재 장착된 아이템오브젝트 @@ 인벤토리가 열려있을때만 있음
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
            // 자식 객체를 삭제
            Managers.Resource.Destroy(child.gameObject);
        }
    }

    public virtual void Init()
    {
        isInit = true;
    }

    //인벤토리를 통한 장착: 플레이어 기어에 추가 + 아이템 오브젝트 변형
    public bool SetItemEquip(ItemObject item)
    {
        if (!ApplyItemEffects(item.itemData)) //플레이어의 기어 딕셔너리에 추가
        {
            Debug.Log("적용실패");
            return false;
        }

        //아이템 오브젝트 변화
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



    // 아이템 장착 해제
    public bool UnsetItemEquip()
    {
        if (equipItemObj == null)
        {
            Debug.Log("이미 장착된 아이템이 없음");
            return false;
        }

        //장착 해제에 성공한 경우
        equipItemObj.GetComponent<RectTransform>().sizeDelta = originalItemSize;
        originalItemSize = Vector2.zero;

        if (!RemoveItemEffects()) //플레이어의 기어 딕셔너리에서 제거
        {
            Debug.Log("제거실패");
            return false;
        }

        equipItemObj =null;
        return true;
    }

    //인벤토리를 통하지 않은 장착 : 플레이어의 gear에 의한 장착 + 장착 혹은 해제에 따른 파라미터 변화 적용
    public virtual bool ApplyItemEffects(ItemData item)
    {
        Debug.Log("아이템 장착");
        InventoryController.Instance.SetEquipItem(slotId, item);
        return true;
    }

    public virtual bool RemoveItemEffects()
    {
        Debug.Log("아이템 해제");
        InventoryController.Instance.UnsetEquipItem(slotId);
        return true;
    }
}
