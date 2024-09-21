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
    public TextMeshProUGUI moneyText; //���� ���� ������ ���ý� �߰�
                                      //
    public BagData equipBag; //���� ������ ����

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
    /// �ڵ鷯���� �ҷ��� �Լ�
    /// </summary>
    public override void InventorySet()
    {
        base.InventorySet();

        Vector2Int newSize = new Vector2Int(equipBag.total_scale_x,equipBag.total_scale_y);


        //������ �׸��带 �ʱ⼼���ϰ� ����ִ� ������
        instantGrid.InitializeGrid(newSize, equipBag.total_weight); // ������ ũ��� �ٲܰ�
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
            // ��� �ð��� ���� ���� ��ȭ
            weightText.color = Color.Lerp(Color.red, Color.white, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weightText.color = Color.white;
    }

}
