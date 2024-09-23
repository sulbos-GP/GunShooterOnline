using UnityEngine;

public partial class InventoryController
{
    /// <summary>
    /// �׸����� ��� �������� ������Ʈ��
    /// </summary>
    public void BackUpSlot(ItemObject item)
    {
        if (IsEquipSlot(item.parentObjId)) 
        {
            return;
        }
        else if (IsPlayerSlot(item.parentObjId)) 
        { 
            playerInvenUI.instantGrid.UpdateBackUpSlot();
            invenInstance.playerInvenUI.WeightTextSet(
                invenInstance.playerInvenUI.instantGrid.GridWeight,
                invenInstance.playerInvenUI.instantGrid.limitWeight);
        }
        else
        {
            otherInvenUI.instantGrid.UpdateBackUpSlot();
        }
    }

    /// <summary>
    /// ������ ������Ʈ�� ��� ������ ������Ʈ
    /// </summary>
    public void BackUpItem(ItemObject item)
    {
        ItemObject.BackUpItem(item);
    }

    /// <summary>
    /// �������� �ű������ ��ġ�� �ǵ���. ����ĭ�� ��� �ٽ� ����. �׸����� ��� ��������� �ǵ���
    /// </summary>
    public void UndoSlot(ItemObject item)
    {
        if(IsEquipSlot(item.backUpParentId))
        {
            EquipSlot UndoEquipSlot = equipSlotDic[item.backUpParentId];
            UndoEquipSlot.EquipItem(item);
            return;
        }
        else if (IsPlayerSlot(item.backUpParentId))
        {
            playerInvenUI.instantGrid.UndoItemSlot();
        }
        else
        {
            otherInvenUI.instantGrid.UndoItemSlot();
        }
        Debug.Log("UndoSlot");
    }

    /// <summary>
    /// �ش� �������� ������ ����� ������ �ǵ���
    /// </summary>
    /// <param name="item"></param>
    public void UndoItem(ItemObject item)
    {
        item.itemData.pos = item.backUpItemPos;
        item.itemData.rotate = item.backUpItemRotate;
        item.Rotate(item.itemData.rotate);

        item.parentObjId = item.backUpParentId;

        if(IsEquipSlot(item.parentObjId))
        {
            EquipSlot targetSlot = equipSlotDic[item.parentObjId];
            targetSlot.EquipItem(item);
        }
        else if(IsPlayerSlot(item.parentObjId))
        {
            playerInvenUI.instantGrid.UpdateItemPosition(item, item.itemData.pos.x, item.itemData.pos.y);

        }
        else
        {
            otherInvenUI.instantGrid.UpdateItemPosition(item, item.itemData.pos.x, item.itemData.pos.y);
        }
    }
}