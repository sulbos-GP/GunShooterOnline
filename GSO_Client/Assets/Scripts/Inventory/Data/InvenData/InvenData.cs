using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new InvenData", menuName = "Inventory/InvenData")]
public class InvenData : ScriptableObject
{
    //public bool isFloatInven;
    public int inventoryId;
    public float limitWeight; //�ش� �κ��丮�� �Ѱ蹫��
    public List<GridData> gridList; //�� �κ��丮���� ������ �׸����� ������
}
