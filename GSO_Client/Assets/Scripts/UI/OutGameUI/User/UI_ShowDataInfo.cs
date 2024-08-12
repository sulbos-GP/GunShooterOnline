using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShowDataInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject showObject;

    // Start is called before the first frame update
    void Start()
    {
        showObject.SetActive(false);
    }

    public void OnClickShowData()
    {
        showObject.SetActive(!showObject.activeSelf);
    }
}
