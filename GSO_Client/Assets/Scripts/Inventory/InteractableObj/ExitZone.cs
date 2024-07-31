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
        //인게임 종료 및 플레이어를 로비씬으로 이동.(인벤토리 보존)
        //서버에 탈출한 플레이어의 ID, 탈출구 ID를 전송
    }
}
