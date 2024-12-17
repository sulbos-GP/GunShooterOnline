using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryController inven = InventoryController.Instance;
        inven.SelectedGrid = GetComponent<GridObject>();

        if(inven.SelectedItem == null)
        {
            return;
        }

        if (inven.SelectedItem.parentObjId == inven.SelectedItem.backUpParentId)
        {
            if (inven.SelectedItem.parentObjId == 0)
                inven.playerInvenUI.SetWeightText(inven.playerInvenUI.instantGrid.GridWeight, inven.playerInvenUI.instantGrid.limitWeight);
            else if (inven.SelectedItem.parentObjId > 7)
                inven.otherInvenUI.SetWeightText(inven.otherInvenUI.instantGrid.GridWeight, inven.otherInvenUI.instantGrid.limitWeight);
            return;
        }
        double increaseItemWeight = inven.isDivideMode ? inven.SelectedItem.itemData.item_weight : inven.SelectedItem.itemWeight;
        double decreaseItemWeight = inven.isDivideMode ? inven.SelectedItem.itemWeight - inven.SelectedItem.itemData.item_weight : inven.SelectedItem.itemWeight;
        InventoryController.AdjustWeight(inven, inven.SelectedItem.parentObjId, increaseItemWeight, true);
        InventoryController.AdjustWeight(inven, inven.SelectedItem.backUpParentId, decreaseItemWeight, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryController inven = InventoryController.Instance;
        inven.SelectedGrid = null;

        if (inven.SelectedItem == null)
        {
            return;
        }

        InventoryController.UpdateInvenWeight(); //모든 아이템의 무게를 원래대로
        InventoryController.UpdateInvenWeight(false);
    }

    
}
