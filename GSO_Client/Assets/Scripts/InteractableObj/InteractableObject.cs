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
        //바로 앞까지 접근하면 충돌이 발생할 만큼 플레이어 이동을 방지(필요 따라서 상호작용을 원활하기 위해)

        //플레이어가 트리거 범위에 들어오면 플레이어의 보호막에 상호작용 버튼이 활성화되고
        //플레이어에게 맞는 상호작용이 필요하여 해당 상호작용 리스트에 아이콘으로 추가
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
            //효과가 있다면 효과를 제거
            if (gameObject.GetComponent<Box>() != null)
            {
                var mat = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
                mat.DisableKeyword("OUTBASE_ON");
                mat.DisableKeyword("INNEROUTLINE_ON");
            }
            playerInput.interactList.Remove(gameObject);
            playerInput.HandleInteraction();
        }
    }

}
