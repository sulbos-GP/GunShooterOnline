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
    public ItemType allowedItemType; // 이 슬롯에 허용되는 아이템 유형
    public ItemObject equippedItem; // 현재 장착된 아이템
    private Vector2 originalItemSize = Vector2.zero;

    //장착 슬롯에 아이템이 배치되었을 경우
    public void EquipItem(ItemObject item)
    {
        //배치에 성공 했을 경우
        equippedItem = item;

        if (equippedItem.curItemGrid != null)
        {
            equippedItem.curItemGrid.RemoveItemFromItemList(equippedItem);
            equippedItem.curItemGrid = null;
            equippedItem.backUpItemGrid = null;
        }

        equippedItem.backUpEquipSlot = this;


        //중앙에 배치 및 슬롯의 크기만큼 크기 세팅
        equippedItem.transform.SetParent(transform);

        item.itemData.rotate = 0;
        item.Rotate(0);
        originalItemSize = item.GetComponent<RectTransform>().rect.size;
        AdjustRectTransform(item);
        equippedItem.GetComponent<RectTransform>().localPosition = Vector3.zero;

        ApplyItemEffects(equippedItem);


        //슬롯 패킷 전송?
    }

    private void AdjustRectTransform(ItemObject item)
    {
        // 1번 RectTransform의 크기를 기준으로 0.9배 한 크기
        Vector2 slotSize = GetComponent<RectTransform>().rect.size * 0.9f;

        // 2번 RectTransform의 현재 크기
        Vector2 itemSize = item.GetComponent<RectTransform>().rect.size;

        // 비율에 맞게 2번 RectTransform의 크기를 조절
        float widthRatio = slotSize.x / itemSize.x;
        float heightRatio = slotSize.y / itemSize.y;

        // 가장 작은 비율로 크기를 맞춤
        float minRatio = Mathf.Min(widthRatio, heightRatio);

        // minRatio가 1보다 작으면 크기를 줄이고, 1보다 크면 크기를 늘림
        if (minRatio != 1f)
        {
            item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize.x * minRatio);
            item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize.y * minRatio);
        }
    }



    // 아이템 장착 해제(장착된 슬롯을 클릭)
    public void UnequipItem()
    {
        if (equippedItem != null)
        {
            RemoveItemEffects(equippedItem);
            //장착 해제에 성공한 경우

            equippedItem.GetComponent<RectTransform>().sizeDelta = originalItemSize;
            originalItemSize = Vector2.zero;

            //슬롯 패킷 전송?

            equippedItem = null;
        }
    }

    //이건 핸들러에서 다루려나?
    protected virtual void ApplyItemEffects(ItemObject item)
    {
        //총일 경우 현재 장착 슬롯의 총 데이터 변경

        //방어구 일경우 플레이어의 방어력 증가
        //데미지감소 0 : 0%, 1 : 15%, 2: 25%, 3 : 35%  , 감소횟수 20회 고정

        //가방일경우 그리드의 데이터 변경
        // 레벨이 커질경우 -> 크기 증가
        // 레벨이 낮아질경우 -> 크기를 줄일수 있는지(아이템칸에 겹치는지) 검사후 가능하면 감소
        // 가방 크기 0 : 2*3 , 1 : 3*4 , 2 : 5*5 , 3 : 7*6

        //회복용품일 경우 퀵슬롯에 퀵슬롯의 아이템 데이터 변경
    }

    protected virtual void RemoveItemEffects(ItemObject item)
    {
        //총일 경우 현재 장착 슬롯의 총 데이터 제거

        //방어구 일경우 플레이어의 방어력 원위치

        //가방일경우 가방의 기본크기보다 아이템이 많이 들어있으면 제거 불가

        //회복용품일 경우 해당 퀵슬롯의 아이템 제거
    }
}
