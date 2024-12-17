using UnityEngine;

public partial class InventoryController
{
    /// <summary>
    /// 아이템을 집거나 놓을때의 상호작용
    /// </summary>
    private void ItemEvent()
    {
        if (isItemSelected) 
        {
            if (SelectedEquip != null)
            {
                ItemReleaseInEquip(selectedItem, selectedEquip);
                ResetSelection();
                return;
            }
            if (isOnDelete)
            {
                ItemReleaseInDelete();
                ResetSelection();
                return;
            }
            if (isGridSelected)
            {
                gridPosition = WorldToGridPos();
                ItemReleaseInGrid(selectedItem, gridPosition);
                
                ResetSelection();
                return;
            }
            UndoSlot(SelectedItem);
            UndoItem(SelectedItem);
            ResetSelection();
        }
        else
        {
            if (isEquipSelected)
            {
                if (selectedEquip.equipItemObj != null)
                {
                    SelectedItem = selectedEquip.equipItemObj;
                    if (!selectedEquip.UnsetItemEquip())
                    {
                        if(selectedItem == null)
                        {
                            return;
                        }
                        selectedEquip.SetEquipItemObj(SelectedItem); //실패시 원위치로
                        ResetSelection();
                        return;
                    }
                    if (selectedItem != null)
                    {
                        SetSelectedObjectToLastSibling(selectedItem.transform);
                    }
                    
                }
                return;
            }

            if (isGridSelected)
            {
                gridPosition = WorldToGridPos();
                
                ItemObject clickedItem = selectedGrid.GetItem(gridPosition.x, gridPosition.y);
                if (clickedItem == null) { Debug.Log("해당 위치에 아이템이없음");  return; }


                if (clickedItem.isHide == true)
                {
                    if (!clickedItem.isOnSearching)
                    {
                        InventoryPacket.SendSearchItemPacket(clickedItem.backUpParentId, clickedItem);
                        clickedItem.SearchItemHandler();
                    }
                }
                else
                {
                    ItemGet(gridPosition);
                }
                return;
            }
        }
    }

    /// <summary>
    /// pos에 해당하는 위치에 아이템이 존재할경우 그리드의 아이템 제거 및 selectedItem으로 지정
    /// </summary>
    /// <param name="pos"></param>
    private void ItemGet(Vector2Int pos)
    {
        if (!isGridSelected) { return; }
        SelectedItem = selectedGrid.PickUpItem(pos.x, pos.y);

        SetSelectedObjectToLastSibling(selectedRect);
    }

    /// <summary>
    /// 아이템을 나눌 경우
    /// </summary>
    private void ItemDivide(ItemObject item, Vector2Int gridPos)
    {
        DivideInterface divideInterface = Managers.Resource.Instantiate("UI/InvenUI/DivideItemInterface", item.transform.parent).GetComponent<DivideInterface>();
        divideInterface.SetInterfacePos();

        //일단 divide모드에서 삭제로 갈경우에는 디바이드 적용없이 그냥 전체 아이템 삭제임

        if (IsEquipSlot(item.parentObjId))
        {
            EquipSlotBase target =equipSlotDic[item.parentObjId];
            //옮긴 위치가 장착칸
            if (item.backUpParentId == selectedEquip.slotId)
            {
                UndoSlot(item);
                UndoItem(item);
                return;
            }
            divideInterface.SetAmountIndex(item, Vector2Int.zero, target.equipItemObj);
        }
        else
        {
            //옮긴 위치가 그리드
            if (item.backUpParentId == selectedGrid.objectId && item.backUpItemPos == gridPos)
            {
                UndoSlot(item);
                UndoItem(item);
                return;
            }

            divideInterface.SetAmountIndex(item, gridPos, overlapItem);
        }

        
    }

    /// <summary>
    /// 아이템을 장착칸에 배치할 경우
    /// </summary>
    private void ItemReleaseInEquip(ItemObject item, EquipSlotBase slot)
    {
        if (slot.equipType == item.itemData.item_type)
        {

            if (isDivideMode)
            {
                ItemDivide(selectedItem, Vector2Int.zero);
            }
            else
            {
                if (slot.equipItemObj != null) //장착칸에 아이템이 존재함
                {
                    if (CheckAbleToMerge(item, slot.equipItemObj))
                    {
                        //합칠 수 있을 경우
                        int needAmount = selectedItem.ItemAmount + slot.equipItemObj.ItemAmount <= ItemObject.maxItemMergeAmount
                            ? selectedItem.ItemAmount : ItemObject.maxItemMergeAmount - slot.equipItemObj.ItemAmount;

                        InventoryPacket.SendMergeItemPacket(item, slot.equipItemObj, needAmount);
                    }
                    else if (item.itemData.item_type == slot.equipType) //교체가 가능할 경우
                    {
                        //교체 패킷 생성?
                        //들고 있는 아이템 장착 및 원래 있던 아이템을 들고있는 아이템이 있던 위치로 이동
                        //원래 있던 위치가 장착칸이라면 똑같이 장착하고 인벤토리라면 배치 가능여부 확인후 배치. 배치 불가능시 바닥에 버리기(상자생성)
                        

                        //임시로 교체가 가능한 경우를 막기
                        UndoSlot(SelectedItem);
                        UndoItem(SelectedItem);
                    }
                    else 
                    {
                        //배치가 불가능
                        UndoSlot(SelectedItem);
                        UndoItem(SelectedItem);
                    }
                }
                else //장착칸이 비어있으니 그냥 배치
                {
                    if(slot.slotId == item.backUpParentId) 
                    {
                        //장착칸에 방금뺀 아이템을 다시 배치할 경우 예외처리
                        UndoSlot(SelectedItem);
                        UndoItem(SelectedItem);
                        return;
                    }

                    //문제가 없다면 move
                    InventoryPacket.SendMoveItemPacket(item);
                }
            } 
        }
        else
        {
            UndoSlot(item);
            UndoItem(item);
        }
    }

    /// <summary>
    /// 아이템을 삭제할경우
    /// </summary>
    private void ItemReleaseInDelete()
    {
        InventoryPacket.SendDeleteItemPacket(selectedItem);
    }

    /// <summary>
    /// 아이템을 인벤토리 그리드에 배치할경우
    /// </summary>
    private void ItemReleaseInGrid(ItemObject item, Vector2Int pos)
    {
        if (itemPlaceableInGrid)
        {
            HandleItemPlacementInGrid(item, pos);
        }
        else
        {
            UndoSlot(item);
            UndoItem(item);
        }

       
    }

    private void HandleItemPlacementInGrid(ItemObject item, Vector2Int pos)
    {
        if (isDivideMode)
        {
            ItemDivide(item, pos);
        }
        else
        {
            if (overlapItem != null)
            {
                if (CheckAbleToMerge(item, overlapItem))
                {
                    int needAmount = item.ItemAmount + overlapItem.ItemAmount <= ItemObject.maxItemMergeAmount
                        ? item.ItemAmount 
                        : ItemObject.maxItemMergeAmount - overlapItem.ItemAmount;

                    InventoryPacket.SendMergeItemPacket(item, overlapItem, needAmount);
                }
                else
                {
                    UndoSlot(SelectedItem);
                    UndoItem(SelectedItem);
                }
            }
            else
            {
                InventoryPacket.SendMoveItemPacket(item, pos);
            }
        }
    }


    public bool CheckAbleToMerge(ItemObject item, ItemObject _overlapItem)
    {
        return item.itemData.isItemConsumeable &&
               item.itemData.itemId == _overlapItem.itemData.itemId &&
               _overlapItem.ItemAmount < ItemObject.maxItemMergeAmount &&
               !_overlapItem.isHide;
    }

    public void DestroyItem(ItemObject targetItem)
    {
        //딕셔너리에서 삭제 및 아이템 오브젝트 삭제
        instantItemDic.Remove(targetItem.itemData.objectId);
        targetItem.DestroyItem();
    }

}


