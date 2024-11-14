using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryController.Instance.SelectedGrid = GetComponent<GridObject>();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryController.Instance.SelectedGrid = null;
    }
}
