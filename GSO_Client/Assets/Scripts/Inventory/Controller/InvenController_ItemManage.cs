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
using static UnityEditor.Progress;
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
                ItemReleaseInEquip();
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
                if (isDivideMode)
                {
                    ItemDivideInGrid(selectedItem, gridPosition);
                }
                else
                {
                    ItemReleaseInGrid(selectedItem, gridPosition);
                }
                ResetSelection();
                return;
            }

            //� ��쵵 �������� ���Ұ�� ��ġ ����
            UndoGridSlot(SelectedItem);
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
                    selectedEquip.UnequipItem();
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
                        SendSearchItemPacket(clickedItem.backUpItemGrid.objectId, clickedItem);
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

    private void ItemDivideInGrid(ItemObject item, Vector2Int gridPosition)
    {
        if (item.curItemGrid == selectedGrid && item.itemData.pos == gridPosition || !itemPlaceableInGrid)
        {
            UndoGridSlot(item);
            UndoItem(item);
            return;
        }

        DivideInterface divideInterface = Managers.Resource.Instantiate("UI/DivideItemInterface",item.transform.parent).GetComponent<DivideInterface>(); //�׸��� ������Ʈ�� �ڽ����� ����
        divideInterface.SetInterfacePos(item);
        divideInterface.SetAmountIndex(item, gridPosition);
        
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
            Debug.Log("������ ��ġ ����");
            item.curItemGrid.PrintInvenContents();
            item.backUpItemGrid.PrintInvenContents();

            UndoGridSlot(item);
            UndoItem(item);
        }

    }

    
    
    /// <summary>
    /// ���ĭ�� �������� ��ġ�� ���
    /// </summary>
    private void ItemReleaseInEquip()
    {
        if (SelectedEquip.allowedItemType == SelectedItem.itemData.item_type)
        {
            if (SelectedEquip.equippedItem == null)//Ÿ���� ��ġ�ϰ� ����ĭ�� �������� ������� -> �ش� ����ĭ�� ������ ����
            {
                EquipSelectedItem();
            }
            else //Ÿ���� ��ġ�ϳ� ����ĭ�� �������� ����
            {
                if (SelectedItem.backUpItemGrid != null)
                { //�׸��� -> ����ĭ
                    SwapWithGrid();
                }
                else
                { //����ĭ -> ����ĭ
                    SwapBetweenEquipSlots();
                }
            }
        }
        else //Ÿ���� ��ġ���� ���� ��� -> ���� �ź�. �������� �������� ���� �ִ� �ڸ��� ��ȯ��Ŵ
        {
            RejectEquipItem(selectedItem);
        }
    }

    /// <summary>
    /// ���ĭ�� ����ֱ⿡ ���õ� �������� �ٷ� ��ġ
    /// </summary>
    private void EquipSelectedItem()
    {
        selectedItem.backUpEquipSlot = selectedEquip;
        if (selectedItem.backUpItemGrid != null)
        {
            selectedItem.backUpItemGrid = null;
        }

        selectedEquip.EquipItem(selectedItem);

        SelectedItem = null;
    }

    /// <summary>
    /// ���â�� �������� �׸��忡 �ְ� ������ �������� ���â�� ��ġ Ȥ�� ���õ� ������ undo 
    /// </summary>
    private void SwapWithGrid()
    {
        ItemObject toSlotItem = SelectedItem;
        ItemObject toGridItem = SelectedEquip.equippedItem;
        EquipSlot targetEquip = SelectedEquip;
        GridObject playerGrid = playerInvenUI.instantGrid; //���� �κ� �ϳ��� �׸��尡 �������� �ȴٸ� �����ؾ���

        Vector2Int? findSpacePos = playerGrid.FindSpaceForObject(toGridItem);
        if(findSpacePos == null)
        {
            toGridItem.RotateRight();
            findSpacePos = playerGrid.FindSpaceForObject(toGridItem);
        }

        if (findSpacePos != null)
        {
            toGridItem.curItemGrid = playerGrid;
            toGridItem.curEquipSlot = null;
            toGridItem.backUpEquipSlot = null;
            //�κ��丮�� ������ �������� ������ ���� -> ��ȯ
            toGridItem.backUpItemGrid = playerGrid;
            targetEquip.UnequipItem();

            toSlotItem.backUpEquipSlot = targetEquip;
            targetEquip.EquipItem(toSlotItem);

            if (toSlotItem.backUpItemGrid != null)
            {
                toSlotItem.backUpItemGrid = null;
            }
        }
        else
        {
            if(toGridItem.itemData.rotate != 0)
            {
                toGridItem.itemData.rotate = 0;
                toGridItem.Rotate(toGridItem.itemData.rotate);
            }
            //�ڸ� ���� ��ġ ����-> ���� ��ġ��
            UndoGridSlot(SelectedItem);
            UndoItem(SelectedItem);
        }
    }

    /// <summary>
    /// ���â���� ���â���� ��ȯ �Ұ��
    /// </summary>
    private void SwapBetweenEquipSlots()
    {
        if (selectedItem.backUpEquipSlot == null) { Debug.Log("Something Wrong In Equip to Equip"); return; }
        ItemObject targetItem = SelectedEquip.equippedItem;
        EquipSlot targetSlot = targetItem.backUpEquipSlot;

        SelectedItem.backUpEquipSlot.UnequipItem(); //�׽�Ʈ�غ���
        targetSlot.UnequipItem();

        SelectedItem.backUpEquipSlot.EquipItem(targetItem); //������ �������� �ִ� ����ĭ�� �����Ϸ��� ĭ�� �ִ� ������ ��ġ
        targetSlot.EquipItem(selectedItem);
        SelectedItem = null;
    }

    /// <summary>
    /// ���ĭ�� ��ġ�� �Ұ����Ұ�� Undo
    /// </summary>
    public void RejectEquipItem(ItemObject item)
    {
        //�׸��忡�� �õ��Ұ��
        if (item.backUpItemGrid != null)
        {
            UndoGridSlot(item);
            UndoItem(item);
        }
        else if (item.backUpEquipSlot != null)
        {//����ĭ���� �õ��Ұ��
            item.curEquipSlot = item.backUpEquipSlot;
            item.backUpEquipSlot.EquipItem(item);
            item = null;
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
    /// ������ ��ġ ����. ���� �� ������ ��ġ ����
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pos"></param>
    private void HandleItemPlacementInGrid(ItemObject item, Vector2Int pos)
    {
        if (overlapItem != null)
        {
            if (CheckAbleToMerge(item , overlapItem))
            {
                //�������� ������ ������
                int needAmount = selectedItem.ItemAmount + overlapItem.ItemAmount <= ItemObject.maxItemMergeAmount 
                    ? selectedItem.ItemAmount : ItemObject.maxItemMergeAmount - overlapItem.ItemAmount;

                SendMergeItemPacket(item, overlapItem, needAmount);
            }
            else
            {
                UndoGridSlot(SelectedItem);
                UndoItem(SelectedItem);
            }
        }
        else
        {
            SendMoveItemPacket(item, pos);
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

    /// <summary>
    /// �׸��忡 ��ġ�� ������ 
    /// </summary>
    public void CompleteItemPlacement(ItemObject item, Vector2Int pos)
    {
        

        if (item.backUpEquipSlot != null) //����ĭ -> �׸���
        {
            //item.backUpEquipSlot.equippedItem = null; //�̺κ��� �ּ��̾ �� ���ư����� üũ�� �����Ұ�
            item.backUpEquipSlot = null;
        }

        item.curItemGrid.PlaceItem(item, pos.x, pos.y);


        ResetSelection();

    }
}


