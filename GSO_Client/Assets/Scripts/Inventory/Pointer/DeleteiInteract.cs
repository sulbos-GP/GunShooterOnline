using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryController invenController;
    private void Awake()
    {
        //인스턴스로 넣으면 다른 그리드가 인벤 컨트롤러를 찾지 못함
        invenController = GameObject.Find("InvenManager").GetComponent<InventoryController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        invenController.IsOnDelete = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        invenController.IsOnDelete = false;
    }
}
