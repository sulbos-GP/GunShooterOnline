using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OtherInventoryUI : InventoryUI
{
    /*other�� �Ź� UI�� ���������� ������ �޶����� �ִ�.
     * ���� ��Ŷ�� ������ �ش� �κ��丮�� �������.
     * ���� ��Ŷ�� �ִٸ� ��Ŷ�� �������� �κ��丮�� ������ �籸��
     * 
     * ���� �÷��̾� �κ��丮�� �ٸ��� ���� � �κ��丮���� �̸��� ǥ���ϴ� �κ��� ������ ����
    */
    private void OnDisable()
    {
        invenData = null;
        foreach(InventoryGrid grids in instantGridList)
        {
            Destroy(grids.gameObject);
        }
        instantGridList.Clear();
        invenWeight = 0;
    }
}
