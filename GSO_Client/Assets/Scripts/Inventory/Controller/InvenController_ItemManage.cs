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
                return;
            }

            //�����Ͱ� ������ ĭ�� �������
            if (isOnDelete)
            {
                ItemReleaseInDelete();
                return;
            }

            //�����Ͱ� �κ��丮 �׸��忡 ��ġ
            if (isGridSelected)
            {
                gridPosition = WorldToGridPos();
                ItemReleaseInGrid(selectedItem, gridPosition);
                return;
            }

            //� ��쵵 �������� ���Ұ�� ��ġ ����
            UndoGridSlot();
            UndoItem();
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
                if (clickedItem == null) { return; }

                //Ŭ���� �������� ������ ��쿡�� ������ �����ϰ� �ƴϸ� �������� ��
                if (clickedItem.isHide == true)
                {
                    clickedItem.UnhideItem();
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

    /// <summary>
    /// �������� ���� �õ�.
    /// </summary>
    private void ItemReleaseInGrid(ItemObject item, Vector2Int pos)
    {
        //bool complete = selectedGrid.CanPlaceItem(item, pos.x, pos.y, ref placeOverlapItem);
        //if (complete)
        if (itemPlaceableInGrid)
        {
            HandleItemPlacementInGrid(item, pos);

        }
        else
        {
            UndoGridSlot();
            UndoItem();
        }

        itemPlaceableInGrid = false;
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
            RejectEquipItem();
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
            selectedItem.backUpItemGrid.RemoveItemFromItemList(selectedItem);
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
        InventoryGrid playerGrid = playerInvenUI.instantGridList[0]; //���� �κ� �ϳ��� �׸��尡 �������� �ȴٸ� �����ؾ���

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
            CompleteItemPlacement(toGridItem, findSpacePos.Value);
            toGridItem.backUpItemGrid = playerGrid;
            targetEquip.UnequipItem();

            toSlotItem.backUpEquipSlot = targetEquip;
            targetEquip.EquipItem(toSlotItem);

            if (toSlotItem.backUpItemGrid != null)
            {
                toSlotItem.backUpItemGrid.RemoveItemFromItemList(toSlotItem);
                toSlotItem.backUpItemGrid = null;
            }
        }
        else
        {
            if(toGridItem.itemData.itemRotate != 0)
            {
                toGridItem.itemData.itemRotate = 0;
                toGridItem.Rotate(toGridItem.itemData.itemRotate);
            }
            //�ڸ� ���� ��ġ ����-> ���� ��ġ��
            UndoGridSlot();
            UndoItem();
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
    private void RejectEquipItem()
    {
        //�׸��忡�� �õ��Ұ��
        if (selectedItem.backUpItemGrid != null)
        {
            UndoGridSlot();
            UndoItem();
        }
        else if (selectedItem.backUpEquipSlot != null)
        {//����ĭ���� �õ��Ұ��
            selectedItem.curEquipSlot = selectedItem.backUpEquipSlot;
            selectedItem.backUpEquipSlot.EquipItem(selectedItem);
            SelectedItem = null;
        }
    }


    /// <summary>
    /// ����ĭ�� ��ġ�Ұ�� -> ������ ����ó��
    /// </summary>
    private void ItemReleaseInDelete()
    {
        if (selectedItem.backUpEquipSlot == null)
        {
            //�� �������� ���� ��ġ�� �÷��̾��� �κ��丮���� ��쿡�� ������ ����.
            C_DeleteItem packet = new C_DeleteItem();
            packet.PlayerId = Managers.Object.MyPlayer.Id;
            packet.ItemData = selectedItem.itemData.GetItemData();
            packet.GridId = selectedItem.backUpItemGrid.gridData.gridId;
            packet.LastGridId = selectedItem.backUpItemGrid.gridData.gridId; //�ǹ̾���
            Managers.Network.Send(packet);
            Debug.Log("C_DeleteItem");

            selectedItem.backUpItemGrid.RemoveItemFromItemList(selectedItem);
            DestroySelectedItem();
            return;
        }
        else
        {   //���ĭ�� �ִ� �������� �������
            //���������� �����ؾ���
            C_DeleteItem packet = new C_DeleteItem();
            packet.PlayerId = Managers.Object.MyPlayer.Id;
            packet.ItemData = selectedItem.itemData.GetItemData();
            packet.GridId = -1; //���� �������� ���ĭ�� ����
            packet.LastGridId = selectedItem.backUpItemGrid.gridData.gridId;
            Managers.Network.Send(packet);
            Debug.Log("C_DeleteItem");

            DestroySelectedItem();
            return;
        }
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
            if (CheckAbleToMerge(item))
            {
                MergeItems(item, pos);
            }
            else
            {
                UndoGridSlot();
                UndoItem();
            }
        }
        else
        {
            CompleteItemPlacement(item, pos);
        }
    }

    /// <summary>
    /// ������ ������ �����Ҷ� ������ �������� üũ
    /// </summary>
    private bool CheckAbleToMerge(ItemObject item)
    {
        return selectedItem.itemData.isItemConsumeable &&
               selectedItem.itemData.itemCode == overlapItem.itemData.itemCode &&
               overlapItem.itemData.itemAmount < ItemObject.maxItemMergeAmount &&
               !overlapItem.isHide;
    }

    /// <summary>
    /// ������ ���� �ǽ�. üũ�� �Ϸ�Ǿ� ������ ������������ �������� ����
    /// </summary>
    private void MergeItems(ItemObject item, Vector2Int pos)
    {
        int totalAmount = selectedItem.itemData.itemAmount + overlapItem.itemData.itemAmount;

        selectedItem.itemData.itemPos = pos;
        SendMoveItemInGridPacket(item, pos);

        if (totalAmount <= ItemObject.maxItemMergeAmount)
        {
            selectedItem.MergeItem(overlapItem, selectedItem.itemData.itemAmount);
            if(selectedItem.backUpItemGrid != null)
            {
                BackUpGridSlot(selectedItem.backUpItemGrid);
            }
            
            DestroySelectedItem();
        }
        else
        {
            int needAmount = ItemObject.maxItemMergeAmount - overlapItem.itemData.itemAmount;

            selectedItem.MergeItem(overlapItem, needAmount);

            // *** ���Կ��� �׸��� ���������� ������ ��� ���� �������� ���������� ���ư����� Ȯ���Ұ�
            UndoGridSlot();
            UndoItem();
        }

        ResetSelection();
    }

    /// <summary>
    /// ��Ʈ�ѷ� �󿡼� ���� ó��
    /// </summary>
    private void DestroySelectedItem()
    {
        if (selectedItem.backUpEquipSlot != null)
        {
            selectedItem.backUpEquipSlot.equippedItem = null;
        }

        if (selectedItem.backUpItemGrid != null)
        {
            selectedItem.backUpItemGrid.RemoveItemFromItemList(selectedItem);
        }

        selectedItem.DestroyItem();
        SelectedItem = null;
    }

    /// <summary>
    /// �׸��忡 ��ġ�� ������ 
    /// </summary>
    private void CompleteItemPlacement(ItemObject item, Vector2Int pos)
    {
        item.curItemGrid.PlaceItem(item, pos.x, pos.y);
        item.curItemGrid.PrintInvenContents(item.curItemGrid, item.curItemGrid.ItemSlot); //üũ
        item.curItemGrid.AddItemToItemList(item.itemData.itemPos, item);

        if (item.backUpItemGrid != null) //�׸��� -> �׸���
        {
            item.backUpItemGrid.RemoveItemFromItemList(item); //���� �׸����� �������� ������ ������ ����Ʈ���� �ش� ������ ����
        }

        if (item.backUpEquipSlot != null) //����ĭ -> �׸���
        {
            //item.backUpEquipSlot.equippedItem = null; //�̺κ��� �ּ��̾ �� ���ư����� üũ�� �����Ұ�
            item.backUpEquipSlot = null;
        }

        SendMoveItemInGridPacket(item, pos); //��� ���� �������� lastItem ������ ���� ����� �Ҵ��

        if(item.curItemGrid != null)
        {
            BackUpItem(item);
            BackUpGridSlot(item.curItemGrid);
        }
        

        ResetSelection();

    }
}


