using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCeilingByShader : MonoBehaviour
{
    public Material transparentMaterial;
    public SpriteRenderer objectRenderer; // 투명화할 오브젝트의 SpriteRenderer
    
    public float onAlpha = 1f;
    public float offAlpha = 0f;
    public float fadeSpeed = 1f;
    private float targetAlpha = 1f;

    private bool isFading = false;

    private void Start()
    {
        objectRenderer = transform.parent.GetComponent<SpriteRenderer>();
        transparentMaterial = objectRenderer.material;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetAlpha = offAlpha;
            isFading = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetAlpha = onAlpha;
            isFading = true;
        }
    }

    private void Update()
    {
        if (isFading)
        {
            ChangeAlpha(targetAlpha);
        }
    }

    private void ChangeAlpha(float targetAlpha)
    {
        Color wallColor = transparentMaterial.color;
        wallColor.a = Mathf.Lerp(wallColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        transparentMaterial.color = wallColor;
        if (Mathf.Abs(wallColor.a - targetAlpha) < 0.01f)
        {
            isFading = false; // 페이드 완료
        }
    }
}
