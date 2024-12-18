using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;


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
            Material transparentMaterial = GetMaterial(targetObjs[i]);

            if (transparentMaterial == null)
            {
                Debug.LogError($"��ϵ� ��ü {targetObjs[i]}�� ���׸����� ������ ����");
                isFading = false;
                return;
            }

            Color wallColor = transparentMaterial.color;
            wallColor.a = Mathf.Lerp(wallColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
            transparentMaterial.color = wallColor;

            if(i == targetObjs.Length - 1 && Mathf.Abs(wallColor.a - targetAlpha) < 0.01f)
            {
                isFading = false; // ���̵� �Ϸ�
            }
        }
    }

    private Material GetMaterial(GameObject target)
    {
        if (target.TryGetComponent<SpriteRenderer>(out SpriteRenderer sprite))
        {
            return sprite.material;
        }
        else if (target.TryGetComponent<TilemapRenderer>(out TilemapRenderer tile))
        {
            return tile.material;
        } //���ɼ��ִ� �������� �߰�
        else
        {
            return null;
        }
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
}
