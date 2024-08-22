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

    
    
    
    private void ItemReleaseInEquip()
    {
        if (SelectedEquip.allowedItemType == SelectedItem.itemData.item_type)
        {
            if (SelectedEquip.equippedItem == null)//Ÿ���� ��ġ�ϰ� ����ĭ�� �������� ������� -> �ش� ����ĭ�� ������ ����
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
            else //Ÿ���� ��ġ�ϳ� ����ĭ�� �������� ����
            {
                if (SelectedItem.backUpItemGrid != null)
                { //�׸��� -> ����ĭ
                    ItemObject targetItem = SelectedEquip.equippedItem;
                    InventoryGrid playerGrid = playerInvenUI.instantGridList[0]; //���� �κ� �ϳ��� �׸��尡 �������� �ȴٸ� �����ؾ���
                    Vector2Int? findSpacePos = playerGrid.FindSpaceForObject(targetItem);
                    if (findSpacePos != null)
                    {
                        //�κ��丮�� ������ �������� ������ ���� -> ��ȯ
                        playerGrid.PlaceItem(targetItem, findSpacePos.Value.x, findSpacePos.Value.y);
                        BackUpItem(targetItem);

                        targetItem.curEquipSlot = null;
                        targetItem.backUpEquipSlot = null;
                        SelectedEquip.equippedItem = null; //���� ���Կ� �ִ� �������� �÷��̾� �κ��丮�� ��ġ

                        selectedItem.backUpEquipSlot = selectedEquip;
                        selectedEquip.EquipItem(selectedItem);

                        if (selectedItem.backUpItemGrid != null)
                        {
                            selectedItem.backUpItemGrid.RemoveItemFromItemList(selectedItem);
                            selectedItem.backUpItemGrid = null;
                        }

                        SelectedItem = null; //��� �ִ� �������� ���ĭ�� ����
                    }
                    else
                    {
                        //�ڸ� ���� ��ġ ����-> ���� ��ġ��
                        UndoGridSlot();
                        UndoItem();
                    }
                }
                else
                { //����ĭ -> ����ĭ
                    if (selectedItem.backUpEquipSlot == null) { Debug.Log("Something Wrong In Equip to Equip"); return; }
                    ItemObject targetItem = SelectedEquip.equippedItem;
                    EquipSlot targetSlot = targetItem.backUpEquipSlot;
                    //�۾��� ���� ��ȯ �����
                    SelectedItem.backUpEquipSlot.EquipItem(targetItem); //������ �������� �ִ� ����ĭ�� �����Ϸ��� ĭ�� �ִ� ������ ��ġ
                    targetSlot.EquipItem(selectedItem);
                    SelectedItem = null;
                }
            }
        }
        else //Ÿ���� ��ġ���� ���� ��� -> ���� �ź�. �������� �������� ���� �ִ� �ڸ��� ��ȯ��Ŵ
        {
            //�׸��忡�� �õ��Ұ��
            
            if (selectedItem.backUpItemGrid != null)
            {
                UndoGridSlot();
                UndoItem();
            }
            else if (selectedItem.backUpEquipSlot != null)
            {//����ĭ���� �õ��Ұ�� -> �̺κ� ���� �׽�Ʈ
                selectedItem.curEquipSlot = selectedItem.backUpEquipSlot;
                selectedItem.backUpEquipSlot.EquipItem(selectedItem);
                SelectedItem = null;
            }
        }
    }

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

            BackUpGridSlot(selectedItem.backUpItemGrid);
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
    /// ������ ��ġ ����, ���� �������� �׸��� �ȿ� ����
    /// </summary>
    private void CompleteItemPlacement(ItemObject item, Vector2Int pos)
    {
        selectedItem.curItemGrid.PlaceItem(item, pos.x, pos.y);
        selectedItem.curItemGrid.PrintInvenContents(selectedItem.curItemGrid, selectedItem.curItemGrid.ItemSlot); //üũ

        selectedItem.backUpItemGrid.RemoveItemFromItemList(selectedItem); //���� �׸����� �������� ������ ������ ����Ʈ���� �ش� ������ ����
        selectedItem.curItemGrid.AddItemToItemList(selectedItem.itemData.itemPos, selectedItem);
        if (selectedItem.backUpEquipSlot != null) //���� ���ĭ���� �׸���� �������� ��ġ�� ������ ���
        {
            //selectedItem.backUpEquipSlot.equippedItem = null; //�̺κ��� �ּ��̾ �� ���ư����� üũ�� �����Ұ�
            selectedItem.backUpEquipSlot = null;
        }
        SendMoveItemInGridPacket(item, pos); //��� ���� �������� lastItem ������ ���� ����� �Ҵ��


        BackUpItem(selectedItem);
        BackUpGridSlot(selectedItem.curItemGrid);

        ResetSelection();
    }
}


