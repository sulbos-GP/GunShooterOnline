using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWithoutViewObj : MonoBehaviour
{
    private void Awake()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Material mat = renderer.material;

        Debug.Log(mat.GetInt("_Stencil"));
        
    }
}
