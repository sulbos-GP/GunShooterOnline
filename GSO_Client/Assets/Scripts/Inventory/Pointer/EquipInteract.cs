using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryController.Instance.SelectedEquip = GetComponent<EquipSlotBase>();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryController.Instance.SelectedEquip = null;
    }
}
