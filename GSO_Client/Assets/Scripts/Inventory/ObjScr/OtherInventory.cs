using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherInventory : MonoBehaviour
{
    //박스 혹은 적 시체 등 플레이어가 인터렉트할 객체의 컴포넌트
    //해당 객체가 인터렉트 하면 이 변수로 인벤데이터가 넘어옴
    //otherInvenUI에서 이곳의 인벤데이터를 조회할것
    public InvenData InputInvenData
    {
        get => otherInventoryData;
        set
        {
            otherInventoryData = value;
        }
    }
    [SerializeField]
    private InvenData otherInventoryData;
    private OtherInventoryUI otherInvenUI;
}
