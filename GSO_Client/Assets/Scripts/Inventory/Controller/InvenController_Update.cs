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
    private void Update()
    {
        if (!isActive) // UI�� ��Ȱ��ȭ��� ����
            return;

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
                else if (isEquipSelected)
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

        HandleHighlight();

    }

    /// <summary>
    /// �����ۿ� ���콺�� ������� ���̶���Ʈ ȿ��
    /// </summary>
    private void HandleHighlight()
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


