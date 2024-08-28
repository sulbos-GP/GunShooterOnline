using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DivideInterface : MonoBehaviour
{
    public Scrollbar scrollBar;
    public TextMeshProUGUI amountText;
    public Button cancelBtn;
    public Button enterBtn;

    public int splitAmountIndex;
    public int maxAmountIndex;

    public GridObject targetGrid;
    public Vector2Int targetPos;
    public ItemObject targetItem;

    private ItemObject overlapItem;
    private void Awake()
    {
        scrollBar = transform.GetChild(0).GetComponent<Scrollbar>();
        amountText = transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        Transform Buttons = transform.GetChild(2);
        cancelBtn = Buttons.GetChild(0).GetComponent<Button>();
        enterBtn = Buttons.GetChild(1).GetComponent<Button>();

        targetItem = null;
    }

    public void SetAmountIndex(ItemObject item, Vector2Int gridPos, GridObject grid, ItemObject overlapItem)
    {
        InventoryController.invenInstance.isDivideInterfaceOn = true;
        targetPos = gridPos;
        targetItem = item;
        targetGrid = grid;
        this.overlapItem = overlapItem != null ? overlapItem : null;

        if (targetItem == null)
        {
            Debug.Log("아이템을 받지 못함");
            return;
        }
        splitAmountIndex = 0;
        maxAmountIndex = targetItem.ItemAmount;

        InitializeScrollbar();
        scrollBar.value = splitAmountIndex/maxAmountIndex;

        AddButtonHandler();
    }

    public void SetInterfacePos(ItemObject item)
    {
        //todo : 변경 필요시 변경
        //현재는 그냥 해당 타겟 그리드 오브젝트의 중앙에 배치되도록 설정
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void AddButtonHandler()
    {
        enterBtn.onClick.RemoveAllListeners();
        enterBtn.onClick.AddListener(OnConfirmButtonClicked);
        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(OnCancelButtonClicked);
    }

    private void InitializeScrollbar()
    {
        scrollBar.numberOfSteps = maxAmountIndex;      
        scrollBar.value = 0;                    
        splitAmountIndex = 1;                         
        UpdateCountText();                         

        scrollBar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }


    private void OnScrollbarValueChanged(float value)
    {
        splitAmountIndex = Mathf.RoundToInt(value * (maxAmountIndex - 1)) + 1;
        UpdateCountText();

        if(targetGrid.objectId == 0)
        {
            GridObject playerGrid = targetGrid;
            double result = Math.Round(playerGrid.GridWeight + targetItem.itemData.item_weight * splitAmountIndex, 2);
            InventoryController.invenInstance.playerInvenUI.weightText.text = $"WEIGHT \n{result} / {playerGrid.limitWeight}";

            if (result > playerGrid.limitWeight)
            {
                InventoryController.invenInstance.playerInvenUI.weightText.color = Color.red;
            }
            else
            {
                InventoryController.invenInstance.playerInvenUI.weightText.color = Color.white;
            }
        }
    }


    private void UpdateCountText()
    {
        amountText.text = $"{splitAmountIndex} / {maxAmountIndex}";
    }

    private void OnCancelButtonClicked()
    {
        InventoryController.invenInstance.UndoGridSlot(targetItem);
        InventoryController.invenInstance.UndoItem(targetItem);
        DestroyInterface();
    }
    

    public void OnConfirmButtonClicked()
    {
        Debug.Log("아이템을 " + splitAmountIndex + "개로 분리합니다.");

        if (overlapItem != null)
        {
            InventoryController.invenInstance.SendMergeItemPacket(targetItem, overlapItem, splitAmountIndex);
        }
        else if (splitAmountIndex == targetItem.ItemAmount)
        {
            InventoryController.invenInstance.SendMoveItemPacket(targetItem, targetPos);
        }
        else if(splitAmountIndex < targetItem.ItemAmount)
        {
            InventoryController.invenInstance.SendDivideItemPacket(targetItem, targetPos, splitAmountIndex);
        }
        InventoryController.invenInstance.playerInvenUI.weightText.color = Color.white;
        DestroyInterface();
    }

    private void DestroyInterface()
    {
        InventoryController.invenInstance.isDivideInterfaceOn = false;
        Managers.Resource.Destroy(gameObject);
    }

}
