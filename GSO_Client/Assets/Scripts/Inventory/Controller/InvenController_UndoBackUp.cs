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
    /// 해당 아이템의 배치된 그리드 오브젝트의 아이템슬롯을 백업함. 이거 전에 backupitem을 먼저 할것
    /// </summary>
    public void BackUpGridSlot(ItemObject item)
    {
        item.backUpItemGrid.UpdateBackUpSlot();
    }

    /// <summary>
    /// 아이템의 상태와 위치를 백업함
    /// </summary>
    public void BackUpItem(ItemObject item)
    {
        item.backUpItemPos = item.itemData.pos; //현재 위치
        item.backUpItemRotate = item.itemData.rotate; //현재 회전
        item.backUpItemGrid = item.curItemGrid; //현재 그리드
    }

    /// <summary>
    /// 아이템 배열을 이전 배열로 되돌림.
    /// </summary>
    public void UndoGridSlot(ItemObject item)
    {
        item.backUpItemGrid.UndoItemSlot();
    }

    /// <summary>
    /// 아이템을 들었던 위치와 각도로 되돌림 selectedItem 해제되니 주의
    /// </summary>
    public void UndoItem(ItemObject item)
    {
        //현재 아이템 오브젝트의 변수를 백업한 변수의 값으로 롤백
        item.itemData.pos = item.backUpItemPos;
        item.itemData.rotate = item.backUpItemRotate;
        item.curItemGrid = item.backUpItemGrid;
        item.Rotate(item.itemData.rotate);

        item.backUpItemGrid.UpdateItemPosition(item, item.itemData.pos.x, item.itemData.pos.y);
    }


}