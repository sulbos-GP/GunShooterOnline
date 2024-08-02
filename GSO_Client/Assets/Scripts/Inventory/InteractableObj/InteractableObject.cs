using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //벽과 맞닿아 있을경우 벽과 충돌한 사이즈 만큼 사이즈 조절(벽을 통과해서 인터렉트를 방지하기 위함)

        //플레이어가 트리거 범위안에 올라오면 플레이어의 상호작용 리스트에 등록하고
        //플레이어에서 가장 가까운 오브젝트를 결정후 해당 오브젝트에 하이라이트 및 인터렉트
        if (collision.CompareTag("Player"))
        {
            //플레이어의 상호작용 가능한 오브젝트 리스트에 등록
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //플레이어의 상호작용 가능한 오브젝트 리스트에서 제거
        }
    }

}
