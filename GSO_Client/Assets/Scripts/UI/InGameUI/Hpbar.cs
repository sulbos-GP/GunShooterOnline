using UnityEngine;
using UnityEngine.UI;

public class Hpbar : BaseInfoBar
{
    public Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
    }

    public override void SetHpBar(float radio)
    {
        slider.value = radio;
    }
}