using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : InteractableObject
{
    public InvenData invenData;
    public GameObject gameEndUI;


    private void Awake()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
        interactRange = 1;

        SetTriggerSize();
    }


    protected override void SetTriggerSize()
    {
        BoxCollider2D Collider = transform.GetComponent<BoxCollider2D>();
        Collider.size = new Vector2(interactRange, interactRange);
    }

    public override void Interact()
    {
        //�ΰ��� ���� �� �÷��̾ �κ������ �̵�.(�κ��丮 ����)
        //������ Ż���� �÷��̾��� ID, Ż�ⱸ ID�� ����
    }
}
