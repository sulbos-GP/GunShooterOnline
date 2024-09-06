using Google.Protobuf.Protocol;
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
    public int bagLv = 0;

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

        Vector2Int curSize = Vector2Int.zero;
        double limitWeight = 0;
        switch (bagLv)
        {
            case 0: curSize = BagSize.level0; limitWeight = 5f; break;
            case 1: curSize = BagSize.level1; limitWeight = 15f; break;
            case 2: curSize = BagSize.level2; limitWeight = 20f; break;
            case 3: curSize = BagSize.level3; limitWeight = 40f; break;
        }

        //������ �׸��带 �ʱ⼼���ϰ� ����ִ� ������
        instantGrid.InitializeGrid(curSize, limitWeight); // ������ ũ��� �ٲܰ�
    }

}
