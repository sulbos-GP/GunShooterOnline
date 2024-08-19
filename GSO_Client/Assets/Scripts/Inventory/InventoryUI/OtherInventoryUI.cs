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

        // �������� ������ ����Ʈ�� ���� ����ϴ�.
        List<ItemObject> itemsToRemove = new List<ItemObject>();

        foreach (InventoryGrid grids in instantGridList)
        {
            // ������ �������� �����մϴ�.
            foreach (ItemObject item in InventoryController.invenInstance.instantItemList)
            {
                if (grids.gridData.itemList.Contains(item.itemData))
                {
                    itemsToRemove.Add(item);
                }
            }

            // ������ �����۵��� ����Ʈ���� �����մϴ�.
            foreach (ItemObject item in itemsToRemove)
            {
                InventoryController.invenInstance.instantItemList.Remove(item);
            }

            itemsToRemove.Clear(); // ���� ������ ���� ����Ʈ�� ���ϴ�.

            Destroy(grids.gameObject);
        }

        instantGridList.Clear();
        invenWeight = 0;
    }
}
