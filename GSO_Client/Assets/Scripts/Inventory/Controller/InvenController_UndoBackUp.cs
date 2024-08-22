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
    /// 아이템 슬롯을 백업함(아이템을 들때 슬롯이 업데이트되기에 백업 필요)
    /// </summary>
    private void BackUpGridSlot(InventoryGrid grid)
    {
        grid.UpdateBackUpSlot();
    }

    /// <summary>
    /// 아이템의 상태와 위치를 백업함
    /// </summary>
    private void BackUpItem(ItemObject item)
    {
        item.backUpItemPos = selectedItem.itemData.itemPos; //현재 위치
        item.backUpItemRotate = selectedItem.itemData.itemRotate; //현재 회전
        item.backUpItemGrid = selectedItem.curItemGrid; //현재 그리드

    }

    /// <summary>
    /// 아이템 배열을 이전 배열로 되돌림.
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
    /// 아이템을 들었던 위치와 각도로 되돌림 selectedItem 해제되니 주의
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
        //현재 아이템 오브젝트의 변수를 백업한 변수의 값으로 롤백
        selectedItem.itemData.itemPos = selectedItem.backUpItemPos;
        selectedItem.itemData.itemRotate = selectedItem.backUpItemRotate;

        //바뀐 변수를 적용. 해당 아이템을 이전상태로 되돌림
        selectedItem.Rotate(selectedItem.itemData.itemRotate);
        selectedItem.backUpItemGrid.PlaceItem(selectedItem, selectedItem.itemData.itemPos.x, selectedItem.itemData.itemPos.y);
        SelectedItem = null;
    }
}


