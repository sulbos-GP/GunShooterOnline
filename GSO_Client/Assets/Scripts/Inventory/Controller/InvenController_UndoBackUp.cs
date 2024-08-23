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
    /// ������ ������ �����(�������� �鶧 ������ ������Ʈ�Ǳ⿡ ��� �ʿ�)
    /// </summary>
    private void BackUpGridSlot(GridObject grid)
    {
        grid.UpdateBackUpSlot();
    }

    /// <summary>
    /// �������� ���¿� ��ġ�� �����
    /// </summary>
    private void BackUpItem(ItemObject item)
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
        if (item.backUpItemGrid != null)
        {
            item.curItemGrid = item.backUpItemGrid;
        }

        if (item.curItemGrid == null) { return; }

        item.backUpItemGrid.UndoItemSlot();
        item.backUpItemGrid.PrintInvenContents(selectedItem.curItemGrid, selectedItem.curItemGrid.ItemSlot);
    }

    /// <summary>
    /// �������� ����� ��ġ�� ������ �ǵ��� selectedItem �����Ǵ� ����
    /// </summary>
    public void UndoItem(ItemObject item)
    {

        if (item.backUpEquipSlot)
        {
            selectedRect.localPosition = Vector3.zero;
            item.backUpEquipSlot.EquipItem(item);
            return;
        }
        //���� ������ ������Ʈ�� ������ ����� ������ ������ �ѹ�
        item.itemData.pos = item.backUpItemPos;
        item.itemData.rotate = item.backUpItemRotate;

        //�ٲ� ������ ����. �ش� �������� �������·� �ǵ���
        item.Rotate(item.itemData.rotate);
        item.backUpItemGrid.PlaceItem(item, item.itemData.pos.x, item.itemData.pos.y);
    }
}


