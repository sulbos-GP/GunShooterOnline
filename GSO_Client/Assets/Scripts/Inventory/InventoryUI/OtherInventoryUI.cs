using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OtherInventoryUI : InventoryUI
{
    /*other은 매번 UI가 켜질때마다 내용이 달라질수 있다.
     * 받은 패킷이 없으면 해당 인벤토리는 비어있음.
     * 받은 패킷이 있다면 패킷의 내용으로 인벤토리의 내용을 재구성
     * 
     * 또한 플레이어 인벤토리와 다르게 위에 어떤 인벤토리인지 이름을 표기하는 부분이 있으니 적용
    */
    private void OnDisable()
    {
        invenData = null;

        // 아이템을 제거할 리스트를 먼저 만듭니다.
        List<ItemObject> itemsToRemove = new List<ItemObject>();

        foreach (InventoryGrid grids in instantGridList)
        {
            // 제거할 아이템을 수집합니다.
            foreach (ItemObject item in InventoryController.invenInstance.instantItemList)
            {
                if (grids.gridData.itemList.Contains(item.itemData))
                {
                    itemsToRemove.Add(item);
                }
            }

            // 수집된 아이템들을 리스트에서 제거합니다.
            foreach (ItemObject item in itemsToRemove)
            {
                InventoryController.invenInstance.instantItemList.Remove(item);
            }

            itemsToRemove.Clear(); // 다음 루프를 위해 리스트를 비웁니다.

            Destroy(grids.gameObject);
        }

        instantGridList.Clear();
        invenWeight = 0;
    }
}
