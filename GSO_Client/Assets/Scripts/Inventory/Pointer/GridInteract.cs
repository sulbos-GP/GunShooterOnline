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

        AdjustWeight(inven, inven.SelectedItem.parentObjId, inven.SelectedItem.itemWeight, true);
        AdjustWeight(inven, inven.SelectedItem.backUpParentId, inven.SelectedItem.itemWeight, false);
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

    private void AdjustWeight(InventoryController inven, int parentId, double weight, bool isAdding)
    {
        double curWeight, resultWeight;

        if (parentId == 0 && inven.playerInvenUI?.instantGrid != null)
        {
            curWeight = inven.playerInvenUI.instantGrid.GridWeight;
            resultWeight = isAdding ? curWeight + weight : curWeight - weight;
            inven.playerInvenUI.SetWeightText(resultWeight, inven.playerInvenUI.instantGrid.limitWeight);
        }
        else if (parentId > 7 && inven.otherInvenUI?.instantGrid != null)
        {
            curWeight = inven.otherInvenUI.instantGrid.GridWeight;
            resultWeight = isAdding ? curWeight + weight : curWeight - weight;
            inven.otherInvenUI.SetWeightText(resultWeight, inven.otherInvenUI.instantGrid.limitWeight);
        }
        else
        {
            Debug.Log("장비칸이거나 유효하지 않은 아이디입니다.");
        }
    }
}
