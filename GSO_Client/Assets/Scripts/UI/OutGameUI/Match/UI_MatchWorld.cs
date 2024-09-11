using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MatchWorld : MonoBehaviour
{
    [SerializeField]
    private TMP_Text    worldNameText;

    [SerializeField]
    private Image       worldImage;

    [SerializeField]
    private Button      worldButton;

    private Action<string> onWorldButtonClicked;

    private void Awake()
    {
        worldButton.onClick.AddListener(OnClickWorld);
    }

    public void OnDestroy()
    {
        onWorldButtonClicked = null;
    }

    private void OnClickWorld()
    {
        if(onWorldButtonClicked != null)
        {
            onWorldButtonClicked.Invoke(worldNameText.text);
        }
    }

    public void SetMatchWorld(string name, Sprite sprite, Action<string> action)
    {
        worldNameText.text = name;
        worldImage.sprite = sprite;
        onWorldButtonClicked += action;
    }
}
