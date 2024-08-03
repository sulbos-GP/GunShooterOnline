using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvenInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //플로팅 인벤토리에 부착하며 인벤토리에 마우스 포인터가 위치하면 해당 변수에 부착된 UI를 반환
    public InventoryController invenController;
    private Inventory inven;
    private void Awake()
    {
        //인스턴스로 넣으면 다른 그리드가 인벤 컨트롤러를 찾지 못함
        invenController = GameObject.FindObjectOfType<InventoryController>();
        inven = GetComponent<Inventory>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //invenController.SelectedInven = inven;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //invenController.SelectedInven = null;
    }
}
