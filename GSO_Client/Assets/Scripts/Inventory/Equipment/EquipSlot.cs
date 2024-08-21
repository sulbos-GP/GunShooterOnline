using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EquipSlot : MonoBehaviour
{
    public ItemType allowedItemType; // 이 슬롯에 허용되는 아이템 유형

    public ItemObject equippedItem; // 현재 장착된 아이템

    //장착 슬롯에 아이템이 배치되었을 경우
    public void EquipItem(ItemObject item)
    {
        //배치에 성공 했을 경우
        equippedItem = item;

        if (equippedItem.curItemGrid != null)
        {
            equippedItem.curItemGrid.RemoveItemFromItemList(equippedItem);
            equippedItem.curItemGrid = null;
        }

        equippedItem.backUpEquipSlot = this;


        //중앙에 배치 및 슬롯의 크기만큼 크기 세팅
        equippedItem.transform.SetParent(transform);
        //equippedItem.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta; 체크가 필요
        equippedItem.GetComponent<RectTransform>().localPosition = Vector3.zero;

        ApplyItemEffects(equippedItem);


        //슬롯 패킷 전송?
    }



    // 아이템 장착 해제(장착된 슬롯을 클릭)
    public void UnequipItem()
    {
        if (equippedItem != null)
        {
            RemoveItemEffects(equippedItem);
            //장착 해제에 성공한 경우

            //슬롯 패킷 전송?

            equippedItem = null;
        }
    }

    private void ApplyItemEffects(ItemObject item)
    {
        //총일 경우 현재 장착 슬롯의 총 데이터 변경

        //방어구 일경우 플레이어의 방어력 증가

        //가방일경우 그리드의 데이터 변경
        // 레벨이 커질경우 -> 크기 증가
        // 레벨이 낮아질경우 -> 크기를 줄일수 있는지(아이템칸에 겹치는지) 검사후 가능하면 감소

        //회복용품일 경우 퀵슬롯에 등록하여 퀵슬롯 버튼 클릭시 해당 아이템의 효과 부여
    }

    private void RemoveItemEffects(ItemObject item)
    {
        //총일 경우 현재 장착 슬롯의 총 데이터 제거

        //방어구 일경우 플레이어의 방어력 원위치

        //가방일경우 가방의 기본크기보다 아이템이 많이 들어있으면 제거 불가

        //회복용품일 경우 해당 퀵슬롯의 아이템 제거
    }
}
