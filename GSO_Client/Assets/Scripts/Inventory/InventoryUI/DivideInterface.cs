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
        InventoryController.Instance.isDivideInterfaceOn = true;
        targetPos = pos;
        targetItem = item;
        objectId = item.parentObjId;
        overlapItem = overlap != null ? overlap : null;

        if (targetItem == null)
        {
            return;
        }

        splitAmountIndex = 1;
        maxAmountIndex = targetItem.ItemAmount;

        InitializeScrollbar();
        scrollBar.value = (float)(splitAmountIndex - 1) / (maxAmountIndex - 2); // 초기값 설정

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
        scrollBar.numberOfSteps = maxAmountIndex - 1; 
        scrollBar.value = (float)(splitAmountIndex - 1) / (maxAmountIndex - 2); 
        UpdateCountText();

        scrollBar.onValueChanged.RemoveAllListeners();
        scrollBar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    private void OnScrollbarValueChanged(float value)
    {
        InventoryController inven = InventoryController.Instance;
        GridObject playerGridObj = inven.playerInvenUI.instantGrid;

        // 0~maxAmountIndex 값을 1~(maxAmountIndex-1)로 변환
        splitAmountIndex = Mathf.RoundToInt(value * (maxAmountIndex - 2)) + 1;
        UpdateCountText();

        //여기서부터 weight 텍스트 변화
        //parentObjId에는 무게 가산, backupParenObjId에는 무게 감산, 같으면 무게변화 없음
        if(targetItem.parentObjId == targetItem.backUpParentId)
        {
            return;
        }

        InventoryController.AdjustWeight(inven, targetItem.parentObjId, targetItem.itemData.item_weight * splitAmountIndex, true);
        InventoryController.AdjustWeight(inven, targetItem.backUpParentId, targetItem.itemData.item_weight * splitAmountIndex, false);
    }

    private void UpdateCountText()
    {
        amountText.text = $"{splitAmountIndex} / {maxAmountIndex}";
    }

    private void OnCancelButtonClicked()
    {
        InventoryController.Instance.UndoSlot(targetItem);
        InventoryController.Instance.UndoItem(targetItem);
        DestroyInterface();
    }
    

    public void OnConfirmButtonClicked()
    {
        if (overlapItem != null)
        {
            //오버렙이 있다면 머지 패킷
            InventoryPacket.SendMergeItemPacket(targetItem, overlapItem, splitAmountIndex);
        }
        else
        {
            InventoryPacket.SendDivideItemPacket(targetItem, targetPos, splitAmountIndex);
        }

        DestroyInterface();
    }

    private void DestroyInterface()
    {
        InventoryController.Instance.isDivideInterfaceOn = false;
        Managers.Resource.Destroy(gameObject);
    }

}
