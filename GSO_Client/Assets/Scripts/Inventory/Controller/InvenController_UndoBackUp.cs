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
        item.backUpItemGrid = item.curItemGrid; //���� �׸���
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
        //���� ������ ������Ʈ�� ������ ����� ������ ������ �ѹ�
        item.itemData.pos = item.backUpItemPos;
        item.itemData.rotate = item.backUpItemRotate;
        item.curItemGrid = item.backUpItemGrid;
        item.Rotate(item.itemData.rotate);

        item.backUpItemGrid.UpdateItemPosition(item, item.itemData.pos.x, item.itemData.pos.y);
    }


}