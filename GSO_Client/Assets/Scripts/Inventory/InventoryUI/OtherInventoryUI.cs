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

    public TextMeshProUGUI invenNameUI;

    private void OnEnable()
    {
        //UI�� �������� ��Ŷ���� �޾ƿ� �κ��丮�� id�� ������쿡�� ������ ���� �״��
        //�ٸ��� ��� �׸��� �ı�
        //base.InventorySet();

        //other�κ��� ���� ��ü�� ���۷��� �ϱ� ���� �÷��̾� ��ǲ�� ���ͷ�Ʈ�� ��ü�� �޾ƾ���
        //�ش� ���ͷ�Ʈ�� ��ü�� OtherInventory�� �κ������͸� �� ��ũ��Ʈ�� �κ������Ϳ� �Ҵ��ϰ� �κ��丮 Set�Ұ�
        //invenNameUI.text = 
    }

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
