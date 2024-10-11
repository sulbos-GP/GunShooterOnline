using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHpController : MonoBehaviour
{
    public const float MAXHpFactor = 0.5f;
    public Material redTintMaterial;
    public float maxHP = 100f;

    [Range(0, 100)]
    public float currentHP = 100f;

    void Update()
    {
        // HP ������ ����Ͽ� HPFactor ����
        float hpFactor = Mathf.Lerp(0.5f, 1f, currentHP / maxHP);
        redTintMaterial.SetFloat("_HPFactor", hpFactor); // HP�� 0�� �� ���� ����
    }
}
