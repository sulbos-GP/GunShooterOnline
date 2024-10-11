using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCeilingByRenderer : MonoBehaviour
{
    public SpriteRenderer wallSprite;

    public float onAlpha = 1f;
    public float offAlpha = 0f;
    public float fadeSpeed = 1f;
    private float targetAlpha = 1f;

    private bool isFading = false;

    private void Awake()
    {
        wallSprite = transform.parent.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("트리거 온");
        if (collision.CompareTag("Player"))
        {
            targetAlpha = offAlpha; // 투명하게 만듦
            isFading = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("트리거 오프");
        if (collision.CompareTag("Player"))
        {
            targetAlpha = onAlpha; // 다시 원래 상태로
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
            isFading = false; // 페이드 완료
        }
    }
}
