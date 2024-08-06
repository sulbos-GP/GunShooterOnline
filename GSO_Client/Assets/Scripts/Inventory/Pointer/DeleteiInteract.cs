using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryController.invenInstance.IsOnDelete = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryController.invenInstance.IsOnDelete = false;
    }
}
