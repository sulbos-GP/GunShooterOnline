using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Eval;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController
{
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
                if (selectedEquip.equippedItem != null)
                {

                    SelectedItem = selectedEquip.equippedItem;
                    if (!selectedEquip.UnEquipItem())
                    {
                        selectedEquip.SetEquipItemObj(SelectedItem); //���н� ����ġ��
                        ResetSelection();
                        return;
                    }
                    SetSelectedObjectToLastSibling(selectedItem.transform);
                }
                return;
            }

            if (isGridSelected)
            {
                
                gridPosition = WorldToGridPos();
                
                ItemObject clickedItem = selectedGrid.GetItem(gridPosition.x, gridPosition.y);
                if (clickedItem == null) { Debug.Log("�ش� ��ġ�� �������̾���");  return; }


                if (clickedItem.isHide == true)
                {
                    if (!clickedItem.isOnSearching)
                    {
                        SendSearchItemPacket(clickedItem.backUpParentId, clickedItem);
                        clickedItem.UnhideItem();
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

    
    private void ItemGet(Vector2Int pos)
    {
        if (!isGridSelected) { return; }
        SelectedItem = selectedGrid.PickUpItem(pos.x, pos.y);

        SetSelectedObjectToLastSibling(selectedRect);
    }

    /// <summary>
    /// �������� ���� ���
    /// </summary>
    private void ItemDivide(ItemObject item, Vector2Int gridPos)
    {
        DivideInterface divideInterface = Managers.Resource.Instantiate("UI/DivideItemInterface", item.transform.parent).GetComponent<DivideInterface>();
        divideInterface.SetInterfacePos();

        //�ϴ� divide��忡�� ������ ����쿡�� ����̵� ������� �׳� ��ü ������ ������

        if (IsEquipSlot(item.parentObjId))
        {
            EquipSlot target =equipSlotDic[item.parentObjId];
            //�ű� ��ġ�� ����ĭ
            if (item.backUpParentId == selectedEquip.slotId)
            {
                UndoSlot(item);
                UndoItem(item);
                return;
            }
            divideInterface.SetAmountIndex(item, Vector2Int.zero, target.equippedItem);
        }
        else
        {
            //�ű� ��ġ�� �׸���
            if (item.backUpParentId == selectedGrid.objectId && item.backUpItemPos == gridPos)
            {
                UndoSlot(item);
                UndoItem(item);
                return;
            }

            divideInterface.SetAmountIndex(item, gridPos, overlapItem);
        }

        
    }

    private void ItemReleaseInEquip(ItemObject item, EquipSlot slot)
    {
        if (slot.allowedItemType == item.itemData.item_type)
        {

            if (isDivideMode)
            {
                ItemDivide(selectedItem, Vector2Int.zero);
            }
            else
            {
                if (slot.equippedItem != null)
                {
                    if (CheckAbleToMerge(item, slot.equippedItem))
                    {

                        int needAmount = selectedItem.ItemAmount + slot.equippedItem.ItemAmount <= ItemObject.maxItemMergeAmount
                            ? selectedItem.ItemAmount : ItemObject.maxItemMergeAmount - slot.equippedItem.ItemAmount;

                        SendMergeItemPacket(item, slot.equippedItem, needAmount);
                    }
                    else
                    {
                        UndoSlot(SelectedItem);
                        UndoItem(SelectedItem);
                    }
                }
                else
                {
                    SendMoveItemPacket(item);
                }
            } 
        }
        else
        {
            UndoSlot(item);
            UndoItem(item);
        }
    }

    private void ItemReleaseInDelete()
    {
        SendDeleteItemPacket(selectedItem);
    }

    
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pos"></param>
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

                    SendMergeItemPacket(item, overlapItem, needAmount);
                }
                else
                {
                    UndoSlot(SelectedItem);
                    UndoItem(SelectedItem);
                }
            }
            else
            {
                SendMoveItemPacket(item, pos);
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
        instantItemDic.Remove(targetItem.itemData.objectId);
        targetItem.DestroyItem();
    }

}


