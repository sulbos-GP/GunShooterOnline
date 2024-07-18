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
        //�ν��Ͻ��� ������ �ٸ� �׸��尡 �κ� ��Ʈ�ѷ��� ã�� ����
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
