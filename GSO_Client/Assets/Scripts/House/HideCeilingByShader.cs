using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class HideCeilingByShader : MonoBehaviour
{
    [Header("�ݶ��̴��� ���˽� Ÿ�ٵ��� ����ȭ")]
    [Description("����ȭ��ų ������Ʈ���� �����ÿ�")]
    public GameObject[] targetObjs;

    [Description("������ ����")]
    public float onAlpha = 1f;
    [Description("���� ����")]
    public float offAlpha = 0f;
    [Description("��ȭ �ӵ�")]
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
                isFading = false; // ���̵� �Ϸ�
            }
        }
        
        
    }
}
