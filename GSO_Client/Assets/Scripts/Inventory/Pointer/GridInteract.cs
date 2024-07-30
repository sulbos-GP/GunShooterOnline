using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryController invenController;
    private InventoryGrid itemGrid;
    //private GameObject grid;
    private void Awake()
    {
        //�ν��Ͻ��� ������ �ٸ� �׸��尡 �κ� ��Ʈ�ѷ��� ã�� ����
        invenController = GameObject.Find("InvenManager").GetComponent<InventoryController>();
        itemGrid = GetComponent<InventoryGrid>();

    }
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

        invenController.SelectedItemGrid = itemGrid;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        invenController.SelectedItemGrid = null;
    }
}
