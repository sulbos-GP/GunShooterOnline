using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrailTestUI : MonoBehaviour
{
    public TrailShooter trailShooter;

    // 슬라이더와 텍스트 UI
    public Slider projNumSlider;
    public TextMeshProUGUI projNumText;

    public Slider distanceSlider;
    public TextMeshProUGUI distanceText;

    public Slider spawnTimeSlider;
    public TextMeshProUGUI spawnTimeText;

    private void Awake()
    {
        
    }

    private void Start()
    {
        // 슬라이더 값 초기화
        projNumSlider.minValue = 0;
        projNumSlider.maxValue = TrailShooter.MaxProj;
        projNumSlider.value = trailShooter.projNum;
        projNumSlider.onValueChanged.AddListener(UpdateProjNum);
        projNumText.text = $"ProjNum\n {trailShooter.projNum} ,  {TrailShooter.MaxProj}";

        distanceSlider.minValue = 0;
        distanceSlider.maxValue = TrailShooter.MaxDistance;
        distanceSlider.value = trailShooter.distance;
        distanceSlider.onValueChanged.AddListener(UpdateDistance);
        distanceText.text = $"Distance\n {trailShooter.distance} , {TrailShooter.MaxDistance}";

        spawnTimeSlider.minValue = 0;
        spawnTimeSlider.maxValue = TrailShooter.MaxDistance;
        spawnTimeSlider.value = trailShooter.spawnTime;
        spawnTimeSlider.onValueChanged.AddListener(UpdateSpawnTime);
        spawnTimeText.text = $"Spawn Time\n {trailShooter.spawnTime} , {TrailShooter.MaxDistance}";
    }

    private void UpdateProjNum(float value)
    {
        trailShooter.projNum = (int)value;
        projNumText.text = $"ProjNum\n{trailShooter.projNum} , {TrailShooter.MaxProj}";
    }

    private void UpdateDistance(float value)
    {
        trailShooter.distance = value;
        distanceText.text = $"Distance\n{trailShooter.distance} , {TrailShooter.MaxDistance}";
    }

    private void UpdateSpawnTime(float value)
    {
        trailShooter.spawnTime = value;
        spawnTimeText.text = $"Spawn Time\n{trailShooter.spawnTime} , {TrailShooter.MaxDistance}";
    }
}
