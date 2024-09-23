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

    public int objectId;
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

    private void OnDisable()
    {
        DestroyInterface();
    }

    public void SetAmountIndex(ItemObject item, Vector2Int pos, ItemObject overlap)
    {
        InventoryController.invenInstance.isDivideInterfaceOn = true;
        targetPos = pos;
        targetItem = item;
        objectId = item.parentObjId;
        overlapItem = overlap != null ? overlap : null;

        if (targetItem == null)
        {
            return;
        }

        splitAmountIndex = 0;
        maxAmountIndex = targetItem.ItemAmount;

        InitializeScrollbar();
        scrollBar.value = splitAmountIndex/maxAmountIndex;

        AddButtonHandler();

    }

    public void SetInterfacePos()
    {
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
        InventoryController inven = InventoryController.invenInstance;
        GridObject playerGridObj = inven.playerInvenUI.instantGrid;
        splitAmountIndex = Mathf.RoundToInt(value * (maxAmountIndex - 1)) + 1;
        UpdateCountText();

        UpdateWeightText(playerGridObj.GridWeight);

        if (InventoryController.IsPlayerSlot(objectId))
        {
            if (!InventoryController.IsPlayerSlot(targetItem.backUpParentId))
            {
                // 플레이어 그리드 -> 상대 그리드
                UpdateWeightText(playerGridObj.GridWeight + targetItem.itemData.item_weight * splitAmountIndex);
            }
        }
        else
        {
            if (InventoryController.IsPlayerSlot(targetItem.backUpParentId))
            {
                // 상대 그리드 -> 플레이어 그리드
                UpdateWeightText(playerGridObj.GridWeight - targetItem.itemData.item_weight * splitAmountIndex);
            }
        }
    }

    private void UpdateWeightText(double currentWeight)
    {
        InventoryController inven = InventoryController.invenInstance;
        GridObject playerGridObj = inven.playerInvenUI.instantGrid;
        double roundedWeight = Math.Round(currentWeight, 2);
        inven.playerInvenUI.weightText.text = $"WEIGHT \n{roundedWeight} / {playerGridObj.limitWeight}";

        inven.playerInvenUI.weightText.color = roundedWeight > playerGridObj.limitWeight ? Color.red : Color.white;
    }

    private void UpdateCountText()
    {
        amountText.text = $"{splitAmountIndex} / {maxAmountIndex}";
    }

    private void OnCancelButtonClicked()
    {
        InventoryController.invenInstance.UndoSlot(targetItem);
        InventoryController.invenInstance.UndoItem(targetItem);
        DestroyInterface();
    }
    

    public void OnConfirmButtonClicked()
    {
        Debug.Log("아이템이 " + splitAmountIndex +" 만큼 분리됨.");
            

        if (overlapItem != null)
        {
            InventoryPacket.SendMergeItemPacket(targetItem, overlapItem, splitAmountIndex);
        }
        else if (splitAmountIndex == targetItem.ItemAmount)
        {
            InventoryPacket.SendMoveItemPacket(targetItem, targetPos);
        }
        else if(splitAmountIndex < targetItem.ItemAmount)
        {
            InventoryPacket.SendDivideItemPacket(targetItem, targetPos, splitAmountIndex);
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
