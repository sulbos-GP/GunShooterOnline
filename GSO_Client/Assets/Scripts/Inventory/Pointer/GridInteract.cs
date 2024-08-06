using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        /*
        if(eventData.pointerEnter.GetComponent<InventoryGrid>() != null) {
            if (invenController.isDragging == true)
            {
                invenController.SelectedItemGrid = null;
            }
            else
            {
                invenController.SelectedItemGrid = itemGrid;
            }
        }
        else
        {
            Debug.Log(eventData.pointerEnter.name);
        }*/

        InventoryController.invenInstance.SelectedItemGrid = GetComponent<InventoryGrid>();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryController.invenInstance.SelectedItemGrid = null;
    }
}
