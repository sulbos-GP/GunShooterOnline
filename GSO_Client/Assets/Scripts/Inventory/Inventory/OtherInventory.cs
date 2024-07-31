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
        Init();
    }

    protected override void Init()
    {
        base.Init();
    }

    private void OnEnable()
    {
        //UI�� �������� ��Ŷ���� �޾ƿ� �κ��丮�� id�� ������쿡�� ������ ���� �״��
        //�ٸ��� ��� �׸��� �ı�
    }

    private void OnDisable()
    {
        invenData = null;
        foreach(InventoryGrid grids in gridList)
        {
            Destroy(grids.gameObject);
        }

        gridList.Clear();

        InvenId = 0;
        invenWeight = 0;
        limitWeight = 0;
    }
}
