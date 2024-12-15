using UnityEngine;

public partial class InventoryController
{
    /// <summary>
    /// 그리드의 백업 변수들을 업데이트함
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
    /// 아이템 오브젝트의 백업 변수를 업데이트
    /// </summary>
    public void BackUpItem(ItemObject item)
    {
        ItemObject.BackUpItem(item);
    }

    /// <summary>
    /// 아이템을 옮기기전의 위치로 되돌림. 장착칸의 경우 다시 장착. 그리드의 경우 백업변수로 되돌림
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
    /// 해당 아이템의 정보를 백업된 정보로 되돌림
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