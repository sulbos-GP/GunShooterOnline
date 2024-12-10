using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGunFire : MonoBehaviour
{
    public Material mat;

    public void Awake()
    {
        var originalMaterial = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().material;
        // Instantiate를 사용하여 기존 머티리얼의 복제본을 생성
        mat = Instantiate(originalMaterial);
        // 이 복제본을 자식 오브젝트의 SpriteRenderer에 적용
        transform.GetChild(0).GetComponent<SpriteRenderer>().material = mat;
    }

    public void setMat()
    {
        // 키워드를 활성화하여 변경 적용
        mat.EnableKeyword("OUTBASE_ON");
        mat.EnableKeyword("INNEROUTLINE_ON");
    }
}
