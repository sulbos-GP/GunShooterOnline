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

    public Vector2Int targetPos;
    public ItemObject targetItem;
    private void Awake()
    {
        scrollBar = transform.GetChild(0).GetComponent<Scrollbar>();
        amountText = transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        Transform Buttons = transform.GetChild(2);
        cancelBtn = Buttons.GetChild(0).GetComponent<Button>();
        enterBtn = Buttons.GetChild(1).GetComponent<Button>();

        targetItem = null;
    }

    public void SetAmountIndex(ItemObject item, Vector2Int gridPos)
    {
        InventoryController.invenInstance.isDivideInterfaceOn = true;
        targetPos = gridPos;
        targetItem = item;

        if(targetItem == null)
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

    private void AddButtonHandler()
    {
        enterBtn.onClick.RemoveAllListeners();
        enterBtn.onClick.AddListener(OnConfirmButtonClicked);
        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(DestroyInterface);
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
    }


    private void UpdateCountText()
    {
        amountText.text = $"{splitAmountIndex} / {maxAmountIndex}";
    }

    private void DestroyInterface()
    {
        InventoryController.invenInstance.isDivideInterfaceOn = false;
        Managers.Resource.Destroy(gameObject);
    }

    public void OnConfirmButtonClicked()
    {
        Debug.Log("�������� " + splitAmountIndex + "���� �и��մϴ�.");

        InventoryController.invenInstance.SendDivideItemPacket(targetItem, targetPos, splitAmountIndex);
        
        DestroyInterface();
    }


    
}
