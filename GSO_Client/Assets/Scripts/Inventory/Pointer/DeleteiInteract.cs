using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryController invenController;

    //private GameObject grid;
    private void Awake()
    {
        //�ν��Ͻ��� ������ �ٸ� �׸��尡 �κ� ��Ʈ�ѷ��� ã�� ����
        invenController = GameObject.FindObjectOfType<InventoryController>();
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
