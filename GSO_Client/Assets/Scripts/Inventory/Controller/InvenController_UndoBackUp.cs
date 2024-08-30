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
    /// �ش� �������� ��ġ�� �׸��� ������Ʈ�� �����۽����� �����. �̰� ���� backupitem�� ���� �Ұ�
    /// </summary>
    public void BackUpSlot(ItemObject item)
    {
        if (item.parentObjId > 0 && item.parentObjId <= 7)
        {
            return;
        }

        if (item.backUpParentId == 0) 
        { 
            playerInvenUI.instantGrid.UpdateBackUpSlot();
        }
        else
        {
            otherInvenUI.instantGrid.UpdateBackUpSlot();
        }
        
        
    }

    /// <summary>
    /// �������� ���¿� ��ġ�� �����
    /// </summary>
    public void BackUpItem(ItemObject item)
    {
        item.backUpItemPos = item.itemData.pos; //���� ��ġ
        item.backUpItemRotate = item.itemData.rotate; //���� ȸ��

        item.backUpParentId = item.parentObjId;
    }

    /// <summary>
    /// ������ �迭�� ���� �迭�� �ǵ���.
    /// </summary>
    public void UndoSlot(ItemObject item)
    {
        if(item.backUpParentId > 0 && item.backUpParentId <= 7)
        {
            //���� �������� ������
            EquipSlot UndoEquipSlot = EquipSlot.GetEquipSlot(item.backUpParentId);
            UndoEquipSlot.EquipItem(item);
            return;
        }

        if (item.backUpParentId == 0)
        {
            playerInvenUI.instantGrid.UndoItemSlot(); ;
        }
        else
        {
            otherInvenUI.instantGrid.UndoItemSlot();
        }
    }

    /// <summary>
    /// �������� ����� ��ġ�� ������ �ǵ��� selectedItem �����Ǵ� ����
    /// </summary>
    public void UndoItem(ItemObject item)
    {
        item.itemData.pos = item.backUpItemPos;
        item.itemData.rotate = item.backUpItemRotate;
        item.Rotate(item.itemData.rotate);

        item.parentObjId = item.backUpParentId;

        if(item.parentObjId >0 && item.parentObjId <= 7)
        {
            EquipSlot targetSlot = EquipSlot.GetEquipSlot(item.parentObjId);
            targetSlot.EquipItem(item);
        }
        else if(item.parentObjId == 0)
        {
            playerInvenUI.instantGrid.UpdateItemPosition(item, item.itemData.pos.x, item.itemData.pos.y);
        }
        else
        {
            otherInvenUI.instantGrid.UpdateItemPosition(item, item.itemData.pos.x, item.itemData.pos.y);
        }
    }


}