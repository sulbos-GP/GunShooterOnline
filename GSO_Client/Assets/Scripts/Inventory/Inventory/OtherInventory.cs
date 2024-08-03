using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherInventory : Inventory
{
    /*other�� �Ź� UI�� ���������� ������ �޶����� �ִ�.
     * ���� ��Ŷ�� ������ �ش� �κ��丮�� �������.
     * ���� ��Ŷ�� �ִٸ� ��Ŷ�� �������� �κ��丮�� ������ �籸��
     * 
     * ���� �÷��̾� �κ��丮�� �ٸ��� ���� � �κ��丮���� �̸��� ǥ���ϴ� �κ��� ������ ����
    */
    private void Awake()
    {
        InventorySet();
    }

    protected override void InventorySet()
    {
        base.InventorySet();
    }

    private void OnEnable()
    {
        //UI�� �������� ��Ŷ���� �޾ƿ� �κ��丮�� id�� ������쿡�� ������ ���� �״��
        //�ٸ��� ��� �׸��� �ı�
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
