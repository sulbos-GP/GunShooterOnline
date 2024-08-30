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

    public void SetAmountIndex(ItemObject item, Vector2Int pos, ItemObject overlapItem)
    {
        InventoryController.invenInstance.isDivideInterfaceOn = true;
        targetPos = pos;
        targetItem = item;
        objectId = item.parentObjId;
        this.overlapItem = overlapItem != null ? overlapItem : null;

        if (targetItem == null)
        {
            Debug.Log("�������� ���� ����");
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
        //todo : ���� �ʿ�� ����
        //����� �׳� �ش� Ÿ�� �׸��� ������Ʈ�� �߾ӿ� ��ġ�ǵ��� ����
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

        if(objectId > 0 && objectId <= 7)
        {
            //장비칸에 아이템의 무게를 추가한다고 했나?
            double result = Math.Round(playerGridObj.GridWeight,2);
                inven.playerInvenUI.weightText.text = $"WEIGHT \n{result} / {playerGridObj.limitWeight}";
        }

        if (objectId == 0) //���� �̵��Ϸ��� Ÿ��
        {
            if (targetItem.backUpParentId == 0)
            {
                //�� -> �� = ��ȭ ����
                double result = Math.Round(playerGridObj.GridWeight,2);
                inven.playerInvenUI.weightText.text = $"WEIGHT \n{result} / {playerGridObj.limitWeight}";
            }
            else
            {   //�� -> �� 
                double result = Math.Round(inven.playerInvenUI.instantGrid.GridWeight + targetItem.itemData.item_weight * splitAmountIndex, 2);
                inven.playerInvenUI.weightText.text = $"WEIGHT \n{result} / {playerGridObj.limitWeight}";

                if (result > playerGridObj.limitWeight)
                {
                    inven.playerInvenUI.weightText.color = Color.red;
                }
                else
                {
                    inven.playerInvenUI.weightText.color = Color.white;
                }
            }
        }
        else 
        {
            if (targetItem.backUpParentId == 0)
            {
                //�� -> �� = ����
                double result = Math.Round(playerGridObj.GridWeight - targetItem.itemData.item_weight * splitAmountIndex, 2);
                inven.playerInvenUI.weightText.text = $"WEIGHT \n{result} / {inven.playerInvenUI.instantGrid.limitWeight}";

                if (result > playerGridObj.limitWeight)
                {
                    inven.playerInvenUI.weightText.color = Color.red;
                }
                else
                {
                    inven.playerInvenUI.weightText.color = Color.white;
                }
            }
            else
            {   //�� -> �� = ��ȭ����
                double result = Math.Round(inven.playerInvenUI.instantGrid.GridWeight,2);
                inven.playerInvenUI.weightText.text = $"WEIGHT \n{result} / {inven.playerInvenUI.instantGrid.limitWeight}";
            }
        }
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
        Debug.Log("�������� " + splitAmountIndex + "���� �и��մϴ�.");

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
