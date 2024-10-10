using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyWall : MonoBehaviour
{
    private const float onAlpha = 1f;
    private const float offAlpha = 0f;
    public SpriteRenderer wallSprite;
    public float fadeSpeed = 1f;
    public bool isFading = false;
    private float targetAlpha = 1f;

    private void Awake()
    {
        wallSprite = transform.parent.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ʈ���� ��");
        if (collision.CompareTag("Player"))
        {
            targetAlpha = offAlpha; // �����ϰ� ����
            isFading = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Ʈ���� ����");
        if (collision.CompareTag("Player"))
        {
            targetAlpha = onAlpha; // �ٽ� ���� ���·�
            isFading = true;
        }
    } 

    private void Update()
    {
        if (isFading)
        {
            ChangeAlpha(wallSprite, targetAlpha);
        }
    }

    private void ChangeAlpha(SpriteRenderer sprite, float targetAlpha)
    {
        Color wallColor = sprite.color;
        wallColor.a = Mathf.Lerp(wallColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        sprite.color = wallColor;

        if (Mathf.Abs(wallColor.a - targetAlpha) < 0.01f)
        {
            isFading = false; // ���̵� �Ϸ�
        }
    }
}
