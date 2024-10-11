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
        // HP 비율을 계산하여 HPFactor 설정
        float hpFactor = Mathf.Lerp(0.5f, 1f, currentHP / maxHP);
        redTintMaterial.SetFloat("_HPFactor", hpFactor); // HP가 0일 때 완전 투명
    }
}
