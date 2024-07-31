using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new InvenData", menuName = "Inventory/InvenData")]
public class InvenData : ScriptableObject
{
    //public bool isFloatInven;
    public int inventoryId;
    public float limitWeight; //해당 인벤토리의 한계무게
    public List<GridData> gridList; //이 인벤토리에서 생성될 그리드의 데이터
}
