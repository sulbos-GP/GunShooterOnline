using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenManager : MonoBehaviour
{
    public InventoryController inventoryController;

    private void Awake()
    {
        Debug.Log("invenInstance");
        if (InventoryController.invenInstance == null)
        {
            InventoryController.invenInstance = inventoryController;

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
