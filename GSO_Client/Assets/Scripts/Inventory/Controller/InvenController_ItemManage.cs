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
    /// <summary>
    /// ��Ŭ���� �������� ���ų� ���� ����
    /// </summary>
    private void ItemEvent()
    {
        if (isItemSelected) //�������� ��ġ�ؾ��ϴ°��
        {
            //�����Ͱ� ���� ���Կ� �������
            if (SelectedEquip != null)
            {
                ItemReleaseInEquip(selectedItem, selectedEquip);
                ResetSelection(); //�������� ���� ���¶�� selection ����
                return;
            }

            //�����Ͱ� ������ ĭ�� �������
            if (isOnDelete)
            {
                ItemReleaseInDelete();
                ResetSelection();
                return;
            }

            //�����Ͱ� �κ��丮 �׸��忡 ��ġ
            if (isGridSelected)
            {
                gridPosition = WorldToGridPos();
                ItemReleaseInGrid(selectedItem, gridPosition);
                ResetSelection();
                return;
            }

            //� ��쵵 �������� ���Ұ�� ��ġ ����
            UndoSlot(SelectedItem);
            UndoItem(SelectedItem);
            ResetSelection();
            Debug.Log("������ ��ġ ����");

        }
        else//�������� �Ⱦ� �ؾ��ϴ� ���
        {
            if (isEquipSelected)
            {
                if (selectedEquip.equippedItem != null)
                {
                    //������ �������� ������ �ش� ������ ���� ����
                    SelectedItem = selectedEquip.equippedItem;
                    selectedEquip.UnEquipItem();
                    SetSelectedObjectToLastSibling(selectedItem.transform);
                }
                return;
            }

            if (isGridSelected)
            {
                
                gridPosition = WorldToGridPos();
                
                ItemObject clickedItem = selectedGrid.GetItem(gridPosition.x, gridPosition.y);
                if (clickedItem == null) { Debug.Log("�ش� ��ġ�� �������� ����");  return; }

                //Ŭ���� �������� ������ ��쿡�� ������ �����ϰ� �ƴϸ� �������� ��
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

    


    /// <summary>
    /// �������� ���� �õ�.
    /// </summary>
    private void ItemGet(Vector2Int pos)
    {
        if (!isGridSelected) { return; }
        SelectedItem = selectedGrid.PickUpItem(pos.x, pos.y);

        //�������� �׸��忡 �������°��� ����
        SetSelectedObjectToLastSibling(selectedRect);
    }

    private void ItemDivide(ItemObject item,Vector2Int gridPosition)
    {
        //��ġ�� �Ұ����ϰų� ���� �ڸ���� Undo
        if (item.parentObjId == selectedGrid.objectId && item.itemData.pos == gridPosition || !itemPlaceableInGrid)
        {
            UndoSlot(item);
            UndoItem(item);
            return;
        }

        DivideInterface divideInterface = Managers.Resource.Instantiate("UI/DivideItemInterface",item.transform.parent).GetComponent<DivideInterface>(); //�׸��� ������Ʈ�� �ڽ����� ����
        divideInterface.SetInterfacePos(item);
        divideInterface.SetAmountIndex(item, gridPosition, overlapItem);
        
    }

    /// <summary>
    /// ���ĭ�� �������� ��ġ�� ���
    /// </summary>
    private void ItemReleaseInEquip(ItemObject item, EquipSlot slot)
    {
        if (slot.allowedItemType == item.itemData.item_type)
        {

            if (isDivideMode)
            {
                ItemDivide(selectedItem, gridPosition);
            }
            else
            {
                if (slot.equippedItem != null)
                {
                    if (CheckAbleToMerge(item, slot.equippedItem))
                    {
                        //�������� ������ ������
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

    /// <summary>
    /// ����ĭ�� ��ġ�Ұ�� -> ������ ����ó��
    /// </summary>
    private void ItemReleaseInDelete()
    {
        SendDeleteItemPacket(selectedItem);
    }

    
    /// <summary>
    /// �������� ���� �õ�.
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

    /// <summary>
    /// ������ ��ġ ����. ���� �� ������ ��ġ ����
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
                    //�������� ������ ������
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

    /// <summary>
    /// ������ ������ �����Ҷ� ������ �������� üũ
    /// </summary>
    public bool CheckAbleToMerge(ItemObject item, ItemObject _overlapItem)
    {
        return item.itemData.isItemConsumeable &&
               item.itemData.itemId == _overlapItem.itemData.itemId &&
               _overlapItem.ItemAmount < ItemObject.maxItemMergeAmount &&
               !_overlapItem.isHide;
    }

    /// <summary>
    /// ��Ʈ�ѷ� �󿡼� ���� ó��
    /// </summary>
    public void DestroyItem(ItemObject targetItem)
    {
        instantItemDic.Remove(targetItem.itemData.objectId);
        targetItem.DestroyItem();
    }

}


