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
        
        else
        {
            playerInvenUI.instantGrid.UpdateBackUpSlot();
            Instance.playerInvenUI.SetWeightText(
                Instance.playerInvenUI.instantGrid.GridWeight,
                Instance.playerInvenUI.instantGrid.limitWeight);
            if(otherInvenUI.instantGrid != null)
            {
                otherInvenUI.instantGrid.UpdateBackUpSlot();
                Instance.otherInvenUI.SetWeightText(
                    Instance.otherInvenUI.instantGrid.GridWeight,
                    Instance.otherInvenUI.instantGrid.limitWeight);
            }
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
            EquipSlotBase UndoEquipSlot = equipSlotDic[item.backUpParentId];
            UndoEquipSlot.SetItemEquip(item);
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
            EquipSlotBase targetSlot = equipSlotDic[item.parentObjId];
            targetSlot.SetItemEquip(item);
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