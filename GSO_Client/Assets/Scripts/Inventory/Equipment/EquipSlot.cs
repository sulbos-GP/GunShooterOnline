using Google.Protobuf.Protocol;
using Google.Protobuf.Reflection;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EquipSlotBase : MonoBehaviour
{
    protected const int ConsumeSlotStartId = 5;
    private const float slotOffsetRatio = 0.8f;

    [Header("Slot Variable")]
    public int slotId;
    public ItemType equipType; // 이 슬롯에 허용되는 아이템 유형
    public ItemObject equipItemObj; // 현재 장착된 아이템오브젝트 @@ 인벤토리가 열려있을때만 있음

    private Vector2 originalItemSize = Vector2.one;
    private bool isInit = false;

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
    public bool SetItemEquip(ItemObject item, bool isQuickInfo = false)
    {
        if (!isQuickInfo)
        {
            if (!ApplyItemEffects(item.itemData)) //플레이어의 기어 딕셔너리에 추가
            {
                Debug.Log("적용실패");
                return false;
            }
        }

        equipItemObj = item;
        equipItemObj.parentObjId = slotId;
        equipItemObj.backUpParentId = slotId;
        item.transform.SetParent(transform);

        //아이템 오브젝트 변화
        SetEquipItemObj(item);
        return true;
    }

    public void SetEquipItemObj(ItemObject item)
    {
        item.itemData.rotate = 0;
        item.Rotate(0);
        RectTransform itemRect = item.GetComponent<RectTransform>();
        SetItemObjSize(item);
        itemRect.localPosition = Vector3.zero;
    }

    private void SetItemObjSize(ItemObject item)
    {
        RectTransform slotRect = GetComponent<RectTransform>();
        RectTransform itemRect = item.GetComponent<RectTransform>();

        // 슬롯의 width와 height 가져오기
        float slotWidth = slotRect.rect.width * slotOffsetRatio;
        float slotHeight = slotRect.rect.height * slotOffsetRatio;

        // 아이템의 width와 height 가져오기
        float itemWidth = itemRect.rect.width;
        float itemHeight = itemRect.rect.height;

        // 너비와 높이 비율 계산
        float widthRatio = slotWidth / itemWidth;
        float heightRatio = slotHeight / itemHeight;

        // 최소 비율 선택 (슬롯 크기에 맞게 비율 조정)
        float scaleRatio = Mathf.Min(widthRatio, heightRatio);

        // 아이템의 scale 조정 (자식 객체에 적용)
        itemRect.localScale = new Vector3(scaleRatio, scaleRatio, 1f);

        // 위치 정렬 보정 (중앙 정렬)
        itemRect.anchoredPosition = Vector2.zero;

        Managers.SystemLog.Message($"SetItemObjSize: 완료 (Slot: {slotWidth}x{slotHeight}, Item: {itemWidth}x{itemHeight}, Scale: {scaleRatio})");
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
        equipItemObj.GetComponent<RectTransform>().localScale = originalItemSize;

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
        if(!InventoryController.Instance.SetEquipItem(slotId, item))
        {
            return false;
        }
        return true;
    }

    public virtual bool RemoveItemEffects()
    {
        Debug.Log("아이템 해제");
        if (!InventoryController.Instance.UnsetEquipItem(slotId))
        {
            return false;
        }

        return true;
    }
}
