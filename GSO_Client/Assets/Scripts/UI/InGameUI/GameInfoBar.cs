using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoBar : BaseInfoBar
{
    public GameObject PlayerExpBarHandle;
    public GameObject PlayerHpBarHandle;
    public GameObject PlayerProfileBtn;

   

    public override void SetHpBar(float radio)
    {
        PlayerHpBarHandle.GetComponent<Slider>().value = radio;
    }
    
    public override void SetExpBar(float radio)
    {
        Debug.Log("EXP : " + radio);
        PlayerExpBarHandle.GetComponent<Slider>().value = radio;
    }
}


