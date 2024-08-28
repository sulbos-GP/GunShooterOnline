using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController
{
    

    private void Update()
    {
        if (!isActive) // UI가 비활성화라면 리턴
            return;

        if (isDivideInterfaceOn)
        {
            return; //나누기 인터페이스가 켜져있다면 아래의 아이템 이벤트를 진행시키지 않음
        }

        if (isPress && !isItemSelected && (isEquipSelected || isGridSelected))
        {
            ItemEvent(); // 아이템 이벤트에서 아이템을 들기 실행
        }

        DragObject();

        HandleHighlighting();
    }

    /// <summary>
    /// 아이템을 들고 있거나 드래깅 중이라면 해당 UI의 렉트의 위치를 마우스의 위치로 계속 업데이트
    /// </summary>
    private void DragObject()
    {
        if (isItemSelected)
        {
            selectedRect.position = new UnityEngine.Vector2(mousePosInput.X, mousePosInput.Y);

            // 먼저 조건을 검사하여 조기 리턴을 통해 코드의 깊이를 줄입니다
            if (dontCheckDivide || isDivideMode || selectedItem.ItemAmount <= 1)
            {
                return;
            }

            // 아이템의 위치나 그리드가 변경되었는지 확인합니다
            bool itemMoved = selectedItem.backUpItemPos != gridPosition;

            if (itemMoved)
            {
                Debug.Log("움직임");
                dontCheckDivide = true; // 나중에 나누기 모드를 확인하지 않도록 설정
                return; // 조건을 만족했으므로 함수 종료
            }

            dragTime += Time.deltaTime;

            if (dragTime >= maxDragTime)
            {
                Debug.Log("나누기 모드 On");
                CreateItemPreview(); // 아이템 미리보기 생성
                isDivideMode = true; // 나누기 모드 활성화
            }
        }
    }

    private void CreateItemPreview()
    {
        if (selectedItem != null)
        {
            itemPreviewInstance = Managers.Resource.Instantiate("UI/DivideImageInstance", selectedGrid.transform);

            RectTransform previewRect = itemPreviewInstance.GetComponent<RectTransform>();
            GameObject selectedImage = selectedItem.transform.GetChild(0).gameObject;
            itemPreviewInstance.GetComponent<Image>().sprite = selectedImage.GetComponent<Image>().sprite;

            if (previewRect != null)
            {
                // 아이템의 크기, 위치, 회전 설정
                previewRect.sizeDelta = selectedRect.sizeDelta;

                Vector2 imagePos = new Vector2(gridPosition.x * GridObject.WidthOfTile + GridObject.WidthOfTile * selectedItem.Width
                    / 2, -(gridPosition.y * GridObject.HeightOfTile + GridObject.HeightOfTile * selectedItem.Height / 2));

                previewRect.localPosition = new UnityEngine.Vector2(imagePos.X, imagePos.Y);
                previewRect.rotation = selectedImage.GetComponent<RectTransform>().rotation;
            }
        }
    }

    private void HandleHighlighting()
    {
        if (!isItemSelected)
        {
            invenHighlight.Show(false);
            return;
        }

        if (!isGridSelected) // 그리드 밖에 있을 경우
        {
            if (isItemSelected)
            {
                if (isOnDelete)
                {
                    HighlightForDelete();
                }
                else if (isEquipSelected)
                {
                    HighlightForEquip();
                }
                else
                {
                    invenHighlight.Show(false); // 하이라이트 없앰
                }
            }
            return;
        }
        else //포인터가 그리드 안에 있음
        {
            HighlightForGrid();
            return;
        }
    }

    private void HighlightForDelete()
    {
        invenHighlight.Show(true);
        invenHighlight.SetColor(HighlightColor.Yellow);
        invenHighlight.SetSize(selectedItem);
        InvenHighLight.highlightObj.transform.SetParent(deleteUI);
        InvenHighLight.highlightObj.transform.position = selectedItem.transform.position;
    }

    private void HighlightForEquip()
    {
        invenHighlight.Show(true);

        // 장착칸의 아이템 타입이 다르면 빨간색 하이라이트
        if (selectedItem.itemData.item_type != selectedEquip.allowedItemType)
        {
            invenHighlight.SetColor(HighlightColor.Red);
        }
        else
        {
            // 장착 가능: 이미 장착된 아이템이 있으면 노랑, 없으면 초록
            invenHighlight.SetColor(selectedEquip.equippedItem != null ? HighlightColor.Yellow : HighlightColor.Green);
        }
        invenHighlight.SetSize(selectedItem);
        InvenHighLight.highlightObj.transform.SetParent(selectedEquip.transform);
        InvenHighLight.highlightObj.transform.position = selectedItem.transform.position;
    }

    private void HighlightForGrid()
    {
        if (!isItemSelected)
        {
            invenHighlight.Show(false);
            return;
        }

        gridPosition = WorldToGridPos();

        if(gridPosition != null)
        {
            if(gridPosition == gridPositionIndex)
            {
                return;
            }
            gridPositionIndex = gridPosition;
        }
        
        Color32 highlightColor = selectedGrid.PlaceCheckInGridHighLight(selectedItem, gridPosition.x, gridPosition.y, ref overlapItem);
        invenHighlight.SetColor(highlightColor);

        HandleHighlight();

    }

    /// <summary>
    /// 아이템에 마우스를 가져대면 하이라이트 효과
    /// </summary>
    private void HandleHighlight()
    {
        //이미 isGridSelected가 true일때만 호출됨
        if (gridPosition == null)
        {
            invenHighlight.Show(false);
            return;
        }

        //같은위치에 반복실행을 막음
        if (HighlightPosition == gridPosition && invenHighlight.gameObject.activeSelf)
        {
            return;
        }

        HighlightPosition = gridPosition;

        if (isItemSelected)
        {
            //아이템을 들고 있다면 그 아이템이 위치한 곳에 하이라이팅
            invenHighlight.Show(true);
            invenHighlight.SetSize(selectedItem);
            invenHighlight.SetParent(selectedGrid);
            invenHighlight.SetPositionOnGridByPos(selectedGrid, selectedItem, gridPosition.x, gridPosition.y);
        }
    }
}


