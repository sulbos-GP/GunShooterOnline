using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OtherInventoryUI : InventoryUI
{

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void InventorySet()
    {
        base.InventorySet();

        //������ �׸��带 �ʱ⼼���ϰ� ����ִ� ������
        instantGrid.InitializeGrid(new Vector2Int(6, 7)); // ������ ũ��� �ٲܰ�
    }
}
