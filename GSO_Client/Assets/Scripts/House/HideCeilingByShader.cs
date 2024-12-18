using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class HideCeilingByShader : MonoBehaviour
{
    [Header("콜라이더에 접촉시 타겟들을 투명화")]
    [Description("투명화시킬 오브젝트들을 넣으시오")]
    public GameObject[] targetObjs;

    [Description("불투명 비율")]
    public float onAlpha = 1f;
    [Description("투명 비율")]
    public float offAlpha = 0f;
    [Description("변화 속도")]
    public float fadeSpeed = 1f;

    private float targetAlpha = 1f;
    private bool isFading = false;

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
        for (int i = 0; i < targetObjs.Length; i++)
        {
            SpriteRenderer objectRenderer = targetObjs[i].GetComponent<SpriteRenderer>();
            Material transparentMaterial = objectRenderer.material;

            Color wallColor = transparentMaterial.color;
            wallColor.a = Mathf.Lerp(wallColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
            transparentMaterial.color = wallColor;

            if(i == targetObjs.Length - 1 && Mathf.Abs(wallColor.a - targetAlpha) < 0.01f)
            {
                isFading = false; // 페이드 완료
            }
        }
        
        
    }
}
