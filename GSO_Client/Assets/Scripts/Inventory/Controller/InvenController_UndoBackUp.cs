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
    public void BackUpGridSlot(ItemObject item)
    {
        item.backUpItemGrid.UpdateBackUpSlot();
    }

    /// <summary>
    /// �������� ���¿� ��ġ�� �����
    /// </summary>
    public void BackUpItem(ItemObject item)
    {
        item.backUpItemPos = item.itemData.pos; //���� ��ġ
        item.backUpItemRotate = item.itemData.rotate; //���� ȸ��

        
        item.backUpItemGrid =item.curItemGrid != null ? item.curItemGrid : null; //���� �׸���
        item.backUpEquipSlot = item.curEquipSlot != null ? item.curEquipSlot : null; 
    }

    /// <summary>
    /// ������ �迭�� ���� �迭�� �ǵ���.
    /// </summary>
    public void UndoGridSlot(ItemObject item)
    {
        item.backUpItemGrid.UndoItemSlot();
    }

    /// <summary>
    /// �������� ����� ��ġ�� ������ �ǵ��� selectedItem �����Ǵ� ����
    /// </summary>
    public void UndoItem(ItemObject item)
    {
        item.itemData.pos = item.backUpItemPos;
        item.itemData.rotate = item.backUpItemRotate;
        item.Rotate(item.itemData.rotate);

        //���� ������ ������Ʈ�� ������ ����� ������ ������ �ѹ�
        if (item.backUpItemGrid != null)
        {
            item.curItemGrid = item.backUpItemGrid;
            item.curEquipSlot = null;
            item.backUpItemGrid.UpdateItemPosition(item, item.itemData.pos.x, item.itemData.pos.y);
        }
        else if (item.backUpEquipSlot != null) 
        {
            item.curEquipSlot = item.backUpEquipSlot;
            item.curItemGrid = null;
            item.backUpEquipSlot.EquipItem(item);
        }
    }


}