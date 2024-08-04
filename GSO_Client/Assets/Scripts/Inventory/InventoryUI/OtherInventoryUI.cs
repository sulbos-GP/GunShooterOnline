using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherInventoryUI : InventoryUI
{
    /*other은 매번 UI가 켜질때마다 내용이 달라질수 있다.
     * 받은 패킷이 없으면 해당 인벤토리는 비어있음.
     * 받은 패킷이 있다면 패킷의 내용으로 인벤토리의 내용을 재구성
     * 
     * 또한 플레이어 인벤토리와 다르게 위에 어떤 인벤토리인지 이름을 표기하는 부분이 있으니 적용
    */
    private void OnEnable()
    {
        //UI가 켜졌을때 패킷으로 받아온 인벤토리의 id가 같을경우에는 이전의 내용 그대로
        //다르면 모든 그리드 파괴
        //base.InventorySet();

        //other인벤을 가진 객체를 리퍼런스 하기 위해 플레이어 인풋에 인터렉트할 객체를 받아야함
        //해당 인터렉트할 객체의 OtherInventory의 인벤데이터를 이 스크립트의 인벤데이터에 할당하고 인벤토리 Set할것
        
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
