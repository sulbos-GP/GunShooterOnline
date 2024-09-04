using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool activeSwitch = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activeSwitch) 
        { 
            InventoryController.invenInstance.SelectedGrid = GetComponent<GridObject>(); 
        }
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (activeSwitch)
        {
            InventoryController.invenInstance.SelectedGrid = null;
        }
    }
}
