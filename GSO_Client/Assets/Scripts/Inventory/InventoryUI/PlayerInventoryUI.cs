using Google.Protobuf.Protocol;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryUI : InventoryUI
{
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI moneyText; //돈에 대한 정보가 나올시 추가
                                      //
    public BagData equipBag; //현재 장착한 가방

    public void WeightTextSet(double GridWeigt, double limitWeight)
    {
        weightText.text = $"WEIGHT \n {GridWeigt} / {limitWeight}"; 
    }

    private void Awake()
    {

    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    /// <summary>
    /// 핸들러에서 불려질 함수
    /// </summary>
    public override void InventorySet()
    {
        base.InventorySet();

        Vector2Int newSize = new Vector2Int(equipBag.total_scale_x,equipBag.total_scale_y);


        //생성된 그리드를 초기세팅하고 들어있는 아이템
        instantGrid.InitializeGrid(newSize, equipBag.total_weight); // 가방의 크기로 바꿀것
    }

    public Coroutine blinkCoroutine = null;
    public void StartBlink()
    {
        if (blinkCoroutine != null)
        {
            StopBlink();
        }

        blinkCoroutine = StartCoroutine(BlinkEffect());
    }

    public void StopBlink()
    {
        StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
    }
    public IEnumerator BlinkEffect()
    {
        float elapsedTime = 0f;
        float transitionTime = 1f;
        weightText.color = Color.red;
        while (elapsedTime < transitionTime)
        {
            // 경과 시간에 따라 색상 변화
            weightText.color = Color.Lerp(Color.red, Color.white, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weightText.color = Color.white;
    }

}
