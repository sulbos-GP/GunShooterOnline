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
    private void BackUpGridSlot(InventoryGrid grid)
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
    private void UndoGridSlot()
    {
        if (selectedItem.backUpItemGrid != null)
        {
            selectedItem.curItemGrid = selectedItem.backUpItemGrid;
        }

        if (selectedItem.curItemGrid == null) { return; }
        if (!isItemSelected) { return; }
        selectedItem.backUpItemGrid.UndoItemSlot();
        selectedItem.backUpItemGrid.PrintInvenContents(selectedItem.curItemGrid, selectedItem.curItemGrid.ItemSlot);
    }

    /// <summary>
    /// �������� ����� ��ġ�� ������ �ǵ��� selectedItem �����Ǵ� ����
    /// </summary>
    private void UndoItem()
    {
        if (!isItemSelected) { return; }
        if (selectedItem.backUpEquipSlot)
        {
            selectedRect.localPosition = Vector3.zero;
            selectedItem.backUpEquipSlot.EquipItem(selectedItem);
            SelectedItem = null;
            return;
        }
        //���� ������ ������Ʈ�� ������ ����� ������ ������ �ѹ�
        selectedItem.itemData.pos = selectedItem.backUpItemPos;
        selectedItem.itemData.rotate = selectedItem.backUpItemRotate;

        //�ٲ� ������ ����. �ش� �������� �������·� �ǵ���
        selectedItem.Rotate(selectedItem.itemData.rotate);
        selectedItem.backUpItemGrid.PlaceItem(selectedItem, selectedItem.itemData.pos.x, selectedItem.itemData.pos.y);
        SelectedItem = null;
    }
}


