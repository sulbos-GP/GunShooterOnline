using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DeleteZone : MonoBehaviour
{
    private Image uiImage;

    [SerializeField]
    private bool isOn;

    public Sprite onSprite;
    public Sprite offSprite;

    public bool IsDeleteOn
    {
        get => isOn;
        set
        {
            isOn = value;
            ChangeImage();
        }
    }

    private void Awake()
    {
        uiImage = GetComponent<Image>();
    }


    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        
    }

    private void ChangeImage()
    {
        if (isOn&& InventoryController.invenInstance.isItemSelected)
        {
            uiImage.sprite = onSprite;
            return;
        }
        uiImage.sprite = offSprite;
    }
}
