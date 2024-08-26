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



    private void Awake()
    {
        scrollBar = transform.GetChild(0).GetComponent<Scrollbar>();
        amountText = transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        Transform Buttons = transform.GetChild(2);
        cancelBtn = Buttons.GetChild(0).GetComponent<Button>();
        enterBtn = Buttons.GetChild(1).GetComponent<Button>();

        SetAmountIndex(32);
    }

    public void SetAmountIndex(int _maxIndex)
    {
        splitAmountIndex = 0;
        maxAmountIndex = _maxIndex;

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

    public void OnConfirmButtonClicked()
    {
        Debug.Log("아이템을 " + splitAmountIndex + "개로 분리합니다.");

        //todo : 아이템 분리로직 작성할것

        DestroyInterface();
    }


    public void DestroyInterface()
    {
        Managers.Resource.Destroy(gameObject);
    }
}
