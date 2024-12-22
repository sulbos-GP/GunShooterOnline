using UnityEngine;

public partial class InventoryController
{
    /// <summary>
    /// �������� ���ų� �������� ��ȣ�ۿ�
    /// </summary>
    private void ItemEvent()
    {
        if (isItemSelected) 
        {
            if (SelectedEquip != null)
            {
                ItemReleaseInEquip(selectedItem, selectedEquip);
                ResetSelection();
                AudioManager.instance.PlaySound("ItemInteract",AudioManager.instance.GetComponent<AudioSource>());
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
                AudioManager.instance.PlaySound("ItemInteract",AudioManager.instance.GetComponent<AudioSource>());
                return;
            }
            UndoSlot(SelectedItem);
            UndoItem(SelectedItem);
            ResetSelection();
            AudioManager.instance.PlaySound("ItemInteract",AudioManager.instance.GetComponent<AudioSource>());
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
                        selectedEquip.SetEquipItemObj(SelectedItem); //���н� ����ġ��
                        ResetSelection();
                        AudioManager.instance.PlaySound("ItemInteract",AudioManager.instance.GetComponent<AudioSource>());
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
                if (clickedItem == null) { Debug.Log("�ش� ��ġ�� �������̾���");  return; }


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
                    AudioManager.instance.PlaySound("ItemInteract",AudioManager.instance.GetComponent<AudioSource>());
                }
                return;
            }
        }
    }

    /// <summary>
    /// pos�� �ش��ϴ� ��ġ�� �������� �����Ұ�� �׸����� ������ ���� �� selectedItem���� ����
    /// </summary>
    /// <param name="pos"></param>
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
        DivideInterface divideInterface = Managers.Resource.Instantiate("UI/InvenUI/DivideItemInterface", item.transform.parent).GetComponent<DivideInterface>();
        divideInterface.SetInterfacePos();

        //�ϴ� divide��忡�� ������ ����쿡�� ����̵� ������� �׳� ��ü ������ ������

        if (IsEquipSlot(item.parentObjId))
        {
            EquipSlotBase target =equipSlotDic[item.parentObjId];
            //�ű� ��ġ�� ����ĭ
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

    /// <summary>
    /// �������� ����ĭ�� ��ġ�� ���
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
                if (slot.equipItemObj != null) //����ĭ�� �������� ������
                {
                    if (CheckAbleToMerge(item, slot.equipItemObj))
                    {
                        //��ĥ �� ���� ���
                        int needAmount = selectedItem.ItemAmount + slot.equipItemObj.ItemAmount <= ItemObject.maxItemMergeAmount
                            ? selectedItem.ItemAmount : ItemObject.maxItemMergeAmount - slot.equipItemObj.ItemAmount;

                        InventoryPacket.SendMergeItemPacket(item, slot.equipItemObj, needAmount);
                    }
                    else if (item.itemData.item_type == slot.equipType) //��ü�� ������ ���
                    {
                        //��ü ��Ŷ ����?
                        //��� �ִ� ������ ���� �� ���� �ִ� �������� ����ִ� �������� �ִ� ��ġ�� �̵�
                        //���� �ִ� ��ġ�� ����ĭ�̶�� �Ȱ��� �����ϰ� �κ��丮��� ��ġ ���ɿ��� Ȯ���� ��ġ. ��ġ �Ұ��ɽ� �ٴڿ� ������(���ڻ���)
                        

                        //�ӽ÷� ��ü�� ������ ��츦 ����
                        UndoSlot(SelectedItem);
                        UndoItem(SelectedItem);
                    }
                    else 
                    {
                        //��ġ�� �Ұ���
                        UndoSlot(SelectedItem);
                        UndoItem(SelectedItem);
                    }
                }
                else //����ĭ�� ��������� �׳� ��ġ
                {
                    if(slot.slotId == item.backUpParentId) 
                    {
                        //����ĭ�� ��ݻ� �������� �ٽ� ��ġ�� ��� ����ó��
                        UndoSlot(SelectedItem);
                        UndoItem(SelectedItem);
                        return;
                    }

                    //������ ���ٸ� move
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
    /// �������� �����Ұ��
    /// </summary>
    private void ItemReleaseInDelete()
    {
        InventoryPacket.SendDeleteItemPacket(selectedItem);
    }

    /// <summary>
    /// �������� �κ��丮 �׸��忡 ��ġ�Ұ��
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
        //��ųʸ����� ���� �� ������ ������Ʈ ����
        instantItemDic.Remove(targetItem.itemData.objectId);
        targetItem.DestroyItem();
    }

}


