using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryController.invenInstance.SelectedGrid = GetComponent<InventoryGrid>();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryController.invenInstance.SelectedGrid = null;
    }
}
