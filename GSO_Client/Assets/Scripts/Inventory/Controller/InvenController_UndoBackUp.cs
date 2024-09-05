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
    public void BackUpSlot(ItemObject item)
    {
        if (IsEquipSlot(item.parentObjId))
        {
            return;
        }

        if (IsPlayerSlot(item.parentObjId)) 
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

        Debug.Log("BackupSlot");
    }

    public void BackUpItem(ItemObject item)
    {
        ItemObject.BackUpItem(item);
        Debug.Log("BackupItem");
    }


    public void UndoSlot(ItemObject item)
    {
        if(IsEquipSlot(item.backUpParentId))
        {
            EquipSlot UndoEquipSlot = EquipSlot.GetEquipSlot(item.backUpParentId);
            UndoEquipSlot.EquipItem(item);
            return;
        }

        if (IsPlayerSlot(item.backUpParentId))
        {
            playerInvenUI.instantGrid.UndoItemSlot();
        }
        else
        {
            otherInvenUI.instantGrid.UndoItemSlot();
        }
        Debug.Log("UndoSlot");
    }

    public void UndoItem(ItemObject item)
    {
        item.itemData.pos = item.backUpItemPos;
        item.itemData.rotate = item.backUpItemRotate;
        item.Rotate(item.itemData.rotate);

        item.parentObjId = item.backUpParentId;

        if(IsEquipSlot(item.parentObjId))
        {
            EquipSlot targetSlot = EquipSlot.GetEquipSlot(item.parentObjId);
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
        Debug.Log("UndoItem");
    }


}