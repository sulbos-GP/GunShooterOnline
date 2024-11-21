using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController
{
    private void Update()
    {
        if (!isActive || isDivideInterfaceOn)
            return;

        if (isPress && !isItemSelected && (isEquipSelected || isGridSelected))
        {
            ItemEvent();
        }

        DragObject();

        HandleHighlighting();
    }

    private void DragObject()
    {
        if (!isItemSelected)
            return;

        selectedRect.position = new UnityEngine.Vector2(mousePosInput.X, mousePosInput.Y);

        if (divideCheckOff || isDivideMode || selectedItem.ItemAmount <= 1)
            return;

        bool itemMoved = false; //아이템이 움직였는지 체크. 2초이상 안움직이면 나누기 모드
        if (IsEquipSlot(selectedItem.backUpParentId))
        {
            itemMoved = selectedItem.backUpParentId != selectedItem.parentObjId;
        }
        else
        {
            itemMoved = selectedItem.backUpItemPos != gridPosition;
        }

        if (itemMoved)
        {
            divideCheckOff = true;
            return;
        }

        dragTime += Time.deltaTime;

        if (dragTime >= maxDragTime)
        {
            CreateItemPreview();
            isDivideMode = true;
        }
    }

    private void CreateItemPreview()
    {
        if (selectedItem == null)
            return;

        GameObject parentInstance = null;
        if (IsEquipSlot(selectedItem.parentObjId))
        {
            parentInstance = equipSlotDic[selectedItem.parentObjId].transform.gameObject;
        }
        else
        {
            parentInstance = selectedGrid.gameObject;
        }

        if (parentInstance == null)
        {
            Debug.Log("부모 객체가 정해지지 않음");
            return;
        }
        itemPreviewInstance = Managers.Resource.Instantiate("UI/InvenUI/DivideImageInstance", parentInstance.transform);

        Vector2 imagePos;
        if (IsEquipSlot(selectedItem.parentObjId))
        {
            imagePos = Vector2.Zero;
        }
        else
        {
            imagePos = new Vector2(gridPosition.x * GridObject.WidthOfTile + GridObject.WidthOfTile * selectedItem.Width
            / 2, -(gridPosition.y * GridObject.HeightOfTile + GridObject.HeightOfTile * selectedItem.Height / 2));
        }

        GameObject selectedImage = selectedItem.transform.GetChild(0).gameObject;
        itemPreviewInstance.GetComponent<Image>().sprite = selectedImage.GetComponent<Image>().sprite;

        RectTransform previewRect = itemPreviewInstance.GetComponent<RectTransform>();
        previewRect.sizeDelta = selectedRect.sizeDelta;
        previewRect.localPosition = new UnityEngine.Vector2(imagePos.X, imagePos.Y);
        previewRect.rotation = selectedImage.GetComponent<RectTransform>().rotation;

    }

    private void HandleHighlighting()
    {
        if (!isItemSelected)
        {
            invenHighlight.Show(false);
            return;
        }

        invenHighlight.Show(true);
        invenHighlight.SetSize(selectedItem);

        if (isGridSelected)
        {
            HighlightForGrid();
        }
        else if (isOnDelete)
        {
            HighlightForDelete();
        }
        else if (isEquipSelected)
        {
            HighlightForEquip();
        }
        else
        {
            invenHighlight.SetColor(HighlightColor.Red);
            invenHighlight.SetHighlightParent(null);
            InvenHighLight.highlightObj.transform.position = selectedItem.transform.position;
        }
        return;
        
    }

    private void HighlightForDelete()
    {
        if(InvenHighLight.highlightObj.transform.position == deleteUI.transform.position)
        {
            return;
        }
        invenHighlight.SetColor(HighlightColor.Yellow);
        InvenHighLight.highlightObj.transform.SetParent(deleteUI);
        InvenHighLight.highlightObj.transform.position = deleteUI.transform.position;
    }

    private void HighlightForEquip()
    {
        if(InvenHighLight.highlightObj.transform.position == selectedEquip.transform.position)
        {
            return;
        }

        if (selectedItem.itemData.item_type != selectedEquip.equipType)
        {
            invenHighlight.SetColor(HighlightColor.Red);
        }
        else
        {
            invenHighlight.SetColor(selectedEquip.equipItemObj != null ? HighlightColor.Yellow : HighlightColor.Green);
        }
        InvenHighLight.highlightObj.transform.SetParent(selectedEquip.transform);
        InvenHighLight.highlightObj.transform.position = selectedEquip.transform.position;
    }

    private void HighlightForGrid()
    {
        gridPosition = WorldToGridPos();
        if (gridPosition == gridPositionIndex)
        {
            return;
        }

        gridPositionIndex = gridPosition;

        Color32 highlightColor = selectedGrid.PlaceCheckInGridHighLight(selectedItem, gridPosition.x, gridPosition.y, ref overlapItem);
        invenHighlight.SetColor(highlightColor);

        invenHighlight.SetHighlightParent(selectedGrid.gameObject);
        invenHighlight.SetPositionOnGridByPos(selectedGrid, selectedItem, gridPosition.x, gridPosition.y);
    }
}


