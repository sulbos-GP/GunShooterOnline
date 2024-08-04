using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherInventory : MonoBehaviour
{
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
