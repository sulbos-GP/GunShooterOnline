using Google.Protobuf.Protocol;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : InteractableObject
{
    private OtherInventoryUI invenUI;

    public Vector2Int size;
    public double weight;

    public bool interactable;

    public Material mat { get;  set; }
    private void Awake()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
        var originalMaterial = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().material;
        // Instantiate를 사용하여 기존 머티리얼의 복제본을 생성
        mat = Instantiate(originalMaterial);
        // 이 복제본을 자식 오브젝트의 SpriteRenderer에 적용
        transform.GetChild(0).GetComponent<SpriteRenderer>().material = mat;
        interactable = true; 
        interactRange = 2;
        invenUI = InventoryController.Instance.otherInvenUI;
        SetTriggerSize();
    }
    
    


    public void SetBox(int x, int y, double weight)
    {
        size.x = x;
        size.y = y;
        this.weight = weight;
    }

    protected override void SetTriggerSize()
    {
        CircleCollider2D Collider = GetComponent<CircleCollider2D>();
        Collider.radius = interactRange;
    }

 
    [ContextMenu("box interact")]
    public override void Interact()
    {
        if (interactable)
        {
            InventoryPacket.SendLoadInvenPacket();
            InventoryPacket.SendLoadInvenPacket(objectId);
        }
        else
        {
            Debug.Log("불가");
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
