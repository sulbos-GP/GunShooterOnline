using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvenInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //�÷��� �κ��丮�� �����ϸ� �κ��丮�� ���콺 �����Ͱ� ��ġ�ϸ� �ش� ������ ������ UI�� ��ȯ
    public InventoryController invenController;
    private Inventory inven;
    private void Awake()
    {
        //�ν��Ͻ��� ������ �ٸ� �׸��尡 �κ� ��Ʈ�ѷ��� ã�� ����
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
