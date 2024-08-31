using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController
{
    private void Update()
    {
        if (!isActive) // UI�� ��Ȱ��ȭ��� ����
            return;

        if (isDivideInterfaceOn)
        {
            return; //������ �������̽��� �����ִٸ� �Ʒ��� ������ �̺�Ʈ�� �����Ű�� ����
        }

        if (isPress && !isItemSelected && (isEquipSelected || isGridSelected))
        {
            ItemEvent(); // ������ �̺�Ʈ���� �������� ��� ����
        }

        DragObject();

        HandleHighlighting();
    }

    /// <summary>
    /// �������� ��� �ְų� �巡�� ���̶�� �ش� UI�� ��Ʈ�� ��ġ�� ���콺�� ��ġ�� ��� ������Ʈ
    /// </summary>
    private void DragObject()
    {
        if (isItemSelected)
        {
            selectedRect.position = new UnityEngine.Vector2(mousePosInput.X, mousePosInput.Y);

            // ���� ������ �˻��Ͽ� ���� ������ ���� �ڵ��� ���̸� ���Դϴ�
            if (dontCheckDivide || isDivideMode || selectedItem.ItemAmount <= 1)
            {
                return;
            }

            bool itemMoved = false;
            // �������� ��ġ�� �׸��尡 ����Ǿ����� Ȯ���մϴ�
            if(selectedItem.backUpParentId > 0 || selectedItem.backUpParentId <= 7)
            {
                itemMoved = selectedItem.backUpParentId != selectedItem.parentObjId;
            }
            else
            {
                itemMoved = selectedItem.backUpItemPos != gridPosition;
            }

            if (itemMoved)
            {
                Debug.Log("������");
                dontCheckDivide = true; // ���߿� ������ ��带 Ȯ������ �ʵ��� ����
                return; // ������ ���������Ƿ� �Լ� ����
            }

            dragTime += Time.deltaTime;

            if (dragTime >= maxDragTime)
            {
                Debug.Log("������ ��� On");
                CreateItemPreview(); // ������ �̸����� ����
                isDivideMode = true; // ������ ��� Ȱ��ȭ
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
                // �������� ũ��, ��ġ, ȸ�� ����
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

        if (!isGridSelected) // �׸��� �ۿ� ���� ���
        {
            if (isItemSelected)
            {
                if (isOnDelete)
                {
                    HighlightForDelete();
                }
                else if(isEquipSelected)
                {
                    HighlightForEquip();
                }
                else
                {
                    invenHighlight.Show(false); // ���̶���Ʈ ����
                }
            }
            return;
        }
        else //�����Ͱ� �׸��� �ȿ� ����
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

        // ����ĭ�� ������ Ÿ���� �ٸ��� ������ ���̶���Ʈ
        if (selectedItem.itemData.item_type != selectedEquip.allowedItemType)
        {
            invenHighlight.SetColor(HighlightColor.Red);
        }
        else
        {
            // ���� ����: �̹� ������ �������� ������ ���, ������ �ʷ�
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

        GridHighlight();

    }

    /// <summary>
    /// �����ۿ� ���콺�� ������� ���̶���Ʈ ȿ��
    /// </summary>
    private void GridHighlight()
    {
        //�̹� isGridSelected�� true�϶��� ȣ���
        if (gridPosition == null)
        {
            invenHighlight.Show(false);
            return;
        }

        //������ġ�� �ݺ������� ����
        if (HighlightPosition == gridPosition && invenHighlight.gameObject.activeSelf)
        {
            return;
        }

        HighlightPosition = gridPosition;

        if (isItemSelected)
        {
            //�������� ��� �ִٸ� �� �������� ��ġ�� ���� ���̶�����
            invenHighlight.Show(true);
            invenHighlight.SetSize(selectedItem);
            invenHighlight.SetParent(selectedGrid);
            invenHighlight.SetPositionOnGridByPos(selectedGrid, selectedItem, gridPosition.x, gridPosition.y);
        }
    }
}


