using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryController.invenInstance.SelectedEquip = GetComponent<EquipSlot>();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryController.invenInstance.SelectedEquip = null;
    }
}
