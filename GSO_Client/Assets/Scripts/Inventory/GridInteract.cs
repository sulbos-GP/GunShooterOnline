using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryController invenController;
    private ItemGrid itemGrid;
    private void Awake()
    {
        //인스턴스로 넣으면 다른 그리드가 인벤 컨트롤러를 찾지 못함
        invenController = GameObject.Find("InvenManager").GetComponent<InventoryController>();
        itemGrid = GetComponent<ItemGrid>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        invenController.SelectedItemGrid = itemGrid;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        invenController.SelectedItemGrid = null;
    }
}
