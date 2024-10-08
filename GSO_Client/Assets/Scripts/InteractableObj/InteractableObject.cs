using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InteractType{
    InventoryObj,
    Door,
    Exit
}

public abstract class InteractableObject : MonoBehaviour
{
    public int objectId;
    public InteractType interactType;
    public float interactRange;

    protected virtual void Init()
    {

    }

    public abstract void Interact();

    protected abstract void SetTriggerSize();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //���� �´�� ������� ���� �浹�� ������ ��ŭ ������ ����(���� ����ؼ� ���ͷ�Ʈ�� �����ϱ� ����)

        //�÷��̾ Ʈ���� �����ȿ� �ö���� �÷��̾��� ��ȣ�ۿ� ����Ʈ�� ����ϰ�
        //�÷��̾�� ���� ����� ������Ʈ�� ������ �ش� ������Ʈ�� ���̶���Ʈ �� ���ͷ�Ʈ
        if (collision.CompareTag("Player"))
        {
            //�÷��̾��� ��ȣ�ۿ� ������ ������Ʈ ����Ʈ�� ���
            if (Managers.Object.MyPlayer == null)
            {
                Debug.Log("myplayer�� ����");
                return;
            }
            InputController playerInput = Managers.Object.MyPlayer.GetComponent<InputController>();
            playerInput.interactList.Add(gameObject);
            playerInput.HandleInteraction();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //�÷��̾��� ��ȣ�ۿ� ������ ������Ʈ ����Ʈ���� ����
            if(Managers.Object.MyPlayer == null)
            {
                Debug.Log("myplayer�� ����");
                return;
            }
            InputController playerInput = Managers.Object.MyPlayer.GetComponent<InputController>();
            playerInput.interactList.Remove(gameObject);
            playerInput.HandleInteraction();
        }
    }

}
