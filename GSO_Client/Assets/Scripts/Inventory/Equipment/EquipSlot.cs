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
    public ItemType allowedItemType; // 이 슬롯에 허용되는 아이템 유형
    public ItemData gearData
    {
        get => Managers.Object.MyPlayer.gearDict[slotId];
        set
        {
            Managers.Object.MyPlayer.gearDict[slotId] = value;
        }
    }
    public ItemObject equippedItem; // 현재 장착된 아이템

    private Vector2 originalItemSize = Vector2.zero;

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

    }

    //인벤토리를 통한 장착: 플레이어 기어에 추가 + 아이템 오브젝트 변형
    public bool EquipItem(ItemObject item)
    {
        //todo 로드를 통해 불려와 질수 있으니 원래 장착되어 있던 아이템과 비교하여 효과 적용
        if (!ApplyItemEffects(item.itemData))
        {
            Debug.Log("적용실패");
            return false;
        }

        //배치에 성공 했을 경우
        equippedItem = item;
        equippedItem.parentObjId = slotId;
        equippedItem.backUpParentId = slotId;


        //중앙에 배치 및 슬롯의 크기만큼 크기 세팅
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



    // 아이템 장착 해제
    public bool UnEquipItem()
    {
        if (equippedItem == null)
        {
            Debug.Log("이미 장착된 아이템이 없음");
            return false;
        }

        if (!RemoveItemEffects(equippedItem.itemData))
        {
            Debug.Log("제거실패");
            return false;
        }
        //장착 해제에 성공한 경우
        equippedItem.GetComponent<RectTransform>().sizeDelta = originalItemSize;
        originalItemSize = Vector2.zero;

        equippedItem = null;
        
        return true;
    }

    //인벤토리를 통하지 않은 장착 : 플레이어의 gear에 의한 장착 + 장착 혹은 해제에 따른 파라미터 변화 적용
    public virtual bool ApplyItemEffects(ItemData item)
    {
        gearData = item;
        Debug.Log("아이템 장착");
        return true;
    }

    public virtual bool RemoveItemEffects(ItemData item)
    {
        gearData = null;
        Debug.Log("아이템 해제");
        return true;
    }
}
