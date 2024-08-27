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
using static UnityEditor.Progress;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController
{
    /// <summary>
    /// 좌클릭시 아이템을 집거나 내려 놓기
    /// </summary>
    private void ItemEvent()
    {
        if (isItemSelected) //아이템을 배치해야하는경우
        {
            //포인터가 장착 슬롯에 있을경우
            if (SelectedEquip != null)
            {
                ItemReleaseInEquip();
                ResetSelection(); //아이템을 놓은 상태라면 selection 리셋
                return;
            }

            //포인터가 휴지통 칸에 있을경우
            if (isOnDelete)
            {
                ItemReleaseInDelete();
                ResetSelection();
                return;
            }

            //포인터가 인벤토리 그리드에 위치
            if (isGridSelected)
            {
                gridPosition = WorldToGridPos();
                if (isDivideMode)
                {
                    ItemDivideInGrid(selectedItem, gridPosition);
                }
                else
                {
                    ItemReleaseInGrid(selectedItem, gridPosition);
                }
                ResetSelection();
                return;
            }

            //어떤 경우도 만족하지 못할경우 배치 실패
            UndoGridSlot(SelectedItem);
            UndoItem(SelectedItem);
            ResetSelection();
            Debug.Log("아이템 배치 실패");

        }
        else//아이템을 픽업 해야하는 경우
        {
            if (isEquipSelected)
            {
                if (selectedEquip.equippedItem != null)
                {
                    //장착된 아이템이 있으면 해당 아이템 장착 해제
                    SelectedItem = selectedEquip.equippedItem;
                    selectedEquip.UnequipItem();
                    SetSelectedObjectToLastSibling(selectedItem.transform);
                }
                return;
            }

            if (isGridSelected)
            {
                
                gridPosition = WorldToGridPos();
                
                ItemObject clickedItem = selectedGrid.GetItem(gridPosition.x, gridPosition.y);
                if (clickedItem == null) { Debug.Log("해당 위치에 아이템이 없음");  return; }

                //클릭한 아이템이 숨겨진 경우에는 숨김을 해제하고 아니면 아이템을 듬
                if (clickedItem.isHide == true)
                {
                    if (!clickedItem.isOnSearching)
                    {
                        SendSearchItemPacket(clickedItem.backUpItemGrid.objectId, clickedItem);
                        clickedItem.UnhideItem();
                    }
                }
                else
                {
                    ItemGet(gridPosition);
                }
                return;
            }
        }
    }

    


    /// <summary>
    /// 아이템을 집는 시도.
    /// </summary>
    private void ItemGet(Vector2Int pos)
    {
        if (!isGridSelected) { return; }
        SelectedItem = selectedGrid.PickUpItem(pos.x, pos.y);

        //아이템이 그리드에 가려지는것을 방지
        SetSelectedObjectToLastSibling(selectedRect);
    }

    private void ItemDivideInGrid(ItemObject item, Vector2Int gridPosition)
    {
        if (item.curItemGrid == selectedGrid && item.itemData.pos == gridPosition || !itemPlaceableInGrid)
        {
            UndoGridSlot(item);
            UndoItem(item);
            return;
        }

        DivideInterface divideInterface = Managers.Resource.Instantiate("UI/DivideItemInterface",item.transform.parent).GetComponent<DivideInterface>(); //그리드 오브젝트의 자식으로 생성
        divideInterface.SetInterfacePos(item);
        divideInterface.SetAmountIndex(item, gridPosition);
        
    }


    /// <summary>
    /// 아이템을 놓는 시도.
    /// </summary>
    private void ItemReleaseInGrid(ItemObject item, Vector2Int pos)
    {
        if (itemPlaceableInGrid)
        {
            HandleItemPlacementInGrid(item, pos);
            
        }
        else
        {
            Debug.Log("아이템 배치 실패");
            item.curItemGrid.PrintInvenContents();
            item.backUpItemGrid.PrintInvenContents();

            UndoGridSlot(item);
            UndoItem(item);
        }

    }

    
    
    /// <summary>
    /// 장비칸에 아이템을 배치할 경우
    /// </summary>
    private void ItemReleaseInEquip()
    {
        if (SelectedEquip.allowedItemType == SelectedItem.itemData.item_type)
        {
            if (SelectedEquip.equippedItem == null)//타입이 일치하고 장착칸에 아이템이 없을경우 -> 해당 장착칸에 아이템 장착
            {
                EquipSelectedItem();
            }
            else //타입이 일치하나 장착칸에 아이템이 있음
            {
                if (SelectedItem.backUpItemGrid != null)
                { //그리드 -> 장착칸
                    SwapWithGrid();
                }
                else
                { //장착칸 -> 장착칸
                    SwapBetweenEquipSlots();
                }
            }
        }
        else //타입이 일치하지 않을 경우 -> 장착 거부. 넣으려는 아이템을 원래 있던 자리로 귀환시킴
        {
            RejectEquipItem(selectedItem);
        }
    }

    /// <summary>
    /// 장비칸이 비어있기에 선택된 아이템을 바로 배치
    /// </summary>
    private void EquipSelectedItem()
    {
        selectedItem.backUpEquipSlot = selectedEquip;
        if (selectedItem.backUpItemGrid != null)
        {
            selectedItem.backUpItemGrid = null;
        }

        selectedEquip.EquipItem(selectedItem);

        SelectedItem = null;
    }

    /// <summary>
    /// 장비창의 아이템을 그리드에 넣고 선택한 아이템을 장비창에 배치 혹은 선택된 아이템 undo 
    /// </summary>
    private void SwapWithGrid()
    {
        ItemObject toSlotItem = SelectedItem;
        ItemObject toGridItem = SelectedEquip.equippedItem;
        EquipSlot targetEquip = SelectedEquip;
        GridObject playerGrid = playerInvenUI.instantGrid; //만약 인벤 하나에 그리드가 여러개가 된다면 수정해야함

        Vector2Int? findSpacePos = playerGrid.FindSpaceForObject(toGridItem);
        if(findSpacePos == null)
        {
            toGridItem.RotateRight();
            findSpacePos = playerGrid.FindSpaceForObject(toGridItem);
        }

        if (findSpacePos != null)
        {
            toGridItem.curItemGrid = playerGrid;
            toGridItem.curEquipSlot = null;
            toGridItem.backUpEquipSlot = null;
            //인벤토리에 기존의 아이템을 넣을수 있음 -> 교환
            toGridItem.backUpItemGrid = playerGrid;
            targetEquip.UnequipItem();

            toSlotItem.backUpEquipSlot = targetEquip;
            targetEquip.EquipItem(toSlotItem);

            if (toSlotItem.backUpItemGrid != null)
            {
                toSlotItem.backUpItemGrid = null;
            }
        }
        else
        {
            if(toGridItem.itemData.rotate != 0)
            {
                toGridItem.itemData.rotate = 0;
                toGridItem.Rotate(toGridItem.itemData.rotate);
            }
            //자리 없음 배치 실패-> 원래 위치로
            UndoGridSlot(SelectedItem);
            UndoItem(SelectedItem);
        }
    }

    /// <summary>
    /// 장비창에서 장비창으로 교환 할경우
    /// </summary>
    private void SwapBetweenEquipSlots()
    {
        if (selectedItem.backUpEquipSlot == null) { Debug.Log("Something Wrong In Equip to Equip"); return; }
        ItemObject targetItem = SelectedEquip.equippedItem;
        EquipSlot targetSlot = targetItem.backUpEquipSlot;

        SelectedItem.backUpEquipSlot.UnequipItem(); //테스트해보기
        targetSlot.UnequipItem();

        SelectedItem.backUpEquipSlot.EquipItem(targetItem); //선택한 아이템이 있던 장착칸에 장착하려는 칸에 있는 아이템 배치
        targetSlot.EquipItem(selectedItem);
        SelectedItem = null;
    }

    /// <summary>
    /// 장비칸에 배치가 불가능할경우 Undo
    /// </summary>
    public void RejectEquipItem(ItemObject item)
    {
        //그리드에서 시도할경우
        if (item.backUpItemGrid != null)
        {
            UndoGridSlot(item);
            UndoItem(item);
        }
        else if (item.backUpEquipSlot != null)
        {//장착칸에서 시도할경우
            item.curEquipSlot = item.backUpEquipSlot;
            item.backUpEquipSlot.EquipItem(item);
            item = null;
        }
    }


    /// <summary>
    /// 삭제칸에 배치할경우 -> 아이템 삭제처리
    /// </summary>
    private void ItemReleaseInDelete()
    {
        SendDeleteItemPacket(selectedItem);
    }

    /// <summary>
    /// 아이템 배치 성공. 병합 및 아이템 배치 실행
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pos"></param>
    private void HandleItemPlacementInGrid(ItemObject item, Vector2Int pos)
    {
        if (overlapItem != null)
        {
            if (CheckAbleToMerge(item , overlapItem))
            {
                //아이템의 머지가 가능함
                int needAmount = selectedItem.ItemAmount + overlapItem.ItemAmount <= ItemObject.maxItemMergeAmount 
                    ? selectedItem.ItemAmount : ItemObject.maxItemMergeAmount - overlapItem.ItemAmount;

                SendMergeItemPacket(item, overlapItem, needAmount);
            }
            else
            {
                UndoGridSlot(SelectedItem);
                UndoItem(SelectedItem);
            }
        }
        else
        {
            SendMoveItemPacket(item, pos);
        }
    }

    /// <summary>
    /// 오버랩 아이템 존재할때 머지가 가능한지 체크
    /// </summary>
    public bool CheckAbleToMerge(ItemObject item, ItemObject _overlapItem)
    {
        return item.itemData.isItemConsumeable &&
               item.itemData.itemId == _overlapItem.itemData.itemId &&
               _overlapItem.ItemAmount < ItemObject.maxItemMergeAmount &&
               !_overlapItem.isHide;
    }

    

    /// <summary>
    /// 컨트롤러 상에서 삭제 처리
    /// </summary>
    public void DestroyItem(ItemObject targetItem)
    {
        instantItemDic.Remove(targetItem.itemData.objectId);
        targetItem.DestroyItem();
    }

    /// <summary>
    /// 그리드에 배치를 성공함 
    /// </summary>
    public void CompleteItemPlacement(ItemObject item, Vector2Int pos)
    {
        

        if (item.backUpEquipSlot != null) //장착칸 -> 그리드
        {
            //item.backUpEquipSlot.equippedItem = null; //이부분이 주석이어도 잘 돌아가는지 체크후 제거할것
            item.backUpEquipSlot = null;
        }

        item.curItemGrid.PlaceItem(item, pos.x, pos.y);


        ResetSelection();

    }
}


