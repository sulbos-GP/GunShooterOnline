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

        switch (bagLv)
        {
            case 0: curSize = BagSize.level0; break;
            case 1: curSize = BagSize.level1; break;
            case 2: curSize = BagSize.level2; break;
            case 3: curSize = BagSize.level3; break;
        }

        //������ �׸��带 �ʱ⼼���ϰ� ����ִ� ������
        instantGrid.InitializeGrid(curSize, 5f); // ������ ũ��� �ٲܰ�
    }

}
