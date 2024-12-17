using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using TMPro;
using UnityEditor;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using static System.TimeZoneInfo;
using Vector2 = System.Numerics.Vector2;

public class ItemObject : MonoBehaviour
{
    public const int maxItemMergeAmount = 64;

    public static ItemObject InstantItemObj(ItemData data, Transform parent = null)
    {
        ItemObject newItem = Managers.Resource.Instantiate("UI/InvenUI/ItemUI", parent).GetComponent<ItemObject>();
        newItem.SetItem(data);
        if (!InventoryController.instantItemDic.ContainsKey(data.objectId))
        {
            InventoryController.instantItemDic.Add(data.objectId, newItem);
        }
        return newItem;
    }

    public static void BackUpItem(ItemObject item)
    {
        item.backUpItemPos = item.itemData.pos;
        item.backUpItemRotate = item.itemData.rotate;
        item.backUpParentId = item.parentObjId;
    }


    public Image imageUI;
    public TextMeshProUGUI amountText; 
    public TextMeshProUGUI searchTimerUI; 

    public RectTransform itemRect;

    public Sprite itemSprite;
    public Sprite hideSprite;

    public Coroutine searchingCoroutine;

    public ItemData itemData; 
    public Vector2Int backUpItemPos; 
    public int backUpItemRotate; 

    public int Width
    {
        get
        {
            if (itemData.rotate % 2 == 0)
            {
                return itemData.width;
            }
            return itemData.height;
        }
    }
    public int Height
    {
        get
        {
            if (itemData.rotate % 2 == 0)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }
    public int ItemAmount
    {
        get { return itemData.amount; }
        set
        {
            itemData.amount = value;

            itemWeight = itemData.item_weight * value;
            itemWeight = Math.Round(itemWeight,2);
            if (itemData.isSearched)
            {
                TextControl();
            }
        }
    }

    public bool isHide;
    public bool isOnSearching; 

    public double itemWeight;

    public int parentObjId;
    public int backUpParentId;

    private void Init()
    {
        itemRect = GetComponent<RectTransform>();
        imageUI = transform.GetChild(0).GetComponent<Image>(); 
        imageUI.raycastTarget = false;

        searchTimerUI = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        searchTimerUI.raycastTarget = false;
        searchTimerUI.gameObject.SetActive(false);
        
        amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        amountText.raycastTarget = false;
        amountText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(searchingCoroutine != null)
        {
            StopCoroutine(searchingCoroutine);
            searchTimerUI.gameObject.SetActive(false);
            searchingCoroutine = null;
            isOnSearching = false;
        }
    }

    public void SetItem(ItemData itemData)
    {
        Init();
        this.itemData = itemData;
        itemRect = GetComponent<RectTransform>();
        itemSprite = GetItemSprite(itemData);
        isOnSearching = false;
        ItemAmount = itemData.amount;


        if (itemData.isSearched == false)
        {
            imageUI.sprite = hideSprite;
            isHide = true;
        }
        else
        {
            imageUI.sprite = itemSprite != null ?  itemSprite : hideSprite;
            Debug.Log($"itemName : {itemData.item_name} , itemPirce : {itemData.item_sell_price}");
            //Item Sprite OutLine Material
            SetMaterialOutLine(imageUI);
            isHide = false;
        }

        Vector2 size = new Vector2();
        size.X = itemData.width * GridObject.WidthOfTile; 
        size.Y = itemData.height * GridObject.HeightOfTile;
        itemRect.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
        imageUI.GetComponent<RectTransform>().sizeDelta = itemRect.sizeDelta;

        Rotate(itemData.rotate);

        itemRect.localPosition = new UnityEngine.Vector2(itemData.width * GridObject.WidthOfTile + 50, itemData.height * GridObject.HeightOfTile - 50);
    }

    public static Sprite GetItemSprite(ItemData itemData)
    {
        Sprite itemSprite = Resources.Load<Sprite>($"Sprite/Item/{itemData.iconName}");

        if (itemSprite == null)
        {
            return null;
        }

        return itemSprite;
    }

    public void SearchItemHandler()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        searchingCoroutine = StartCoroutine(SearchingTimer(itemData.item_searchTime));
    }

    public void SetMaterialOutLine(Image imageUI)
    {
        if(itemData.item_sell_price<=500)
            imageUI.material = Resources.Load<Material>("Material/WhiteOutLine");
        else if(itemData.item_sell_price<=1000)
            imageUI.material = Resources.Load<Material>("Material/YellowOutLine");
        else if(itemData.item_sell_price<=1500)
            imageUI.material = Resources.Load<Material>("Material/RedOutLine");
        else
            imageUI.material = Resources.Load<Material>("Material/PurpleOutLine");
    }

    private IEnumerator SearchingTimer(float duration)
    {
        float timeRemaining = duration;

        searchTimerUI.gameObject.SetActive(true);
        AudioManager.instance.PlaySound("Search",gameObject.GetComponent<AudioSource>());
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(timeRemaining);
            int milliseconds = Mathf.FloorToInt((timeRemaining - seconds) * 10); // One decimal place
    
            searchTimerUI.text = string.Format("{0}:{1}", seconds, milliseconds);
            yield return null;
        }
        gameObject.GetComponent<AudioSource>().Stop();
        RevealItem();
        SetMaterialOutLine(imageUI);
        searchTimerUI.gameObject.SetActive(false);
        searchingCoroutine = null;
    }

    public void RevealItem()
    {
        isHide = false;
        itemData.isSearched = true;

        imageUI.sprite = itemSprite;
        TextControl();
    }

    public void HideItem()
    {
        isHide = true;
        itemData.isSearched = false;
        if (searchingCoroutine != null)
        {
            StopCoroutine(searchingCoroutine);
            isOnSearching = false;
            searchingCoroutine = null;
        }

        imageUI.sprite = hideSprite;
    }


    public void RotateRight()
    {
        itemData.rotate = (itemData.rotate + 1) % 4;
        Rotate(itemData.rotate);
    }

    public void Rotate(int rotateInt)
    {
        itemRect.sizeDelta = new UnityEngine.Vector2(Width* GridObject.WidthOfTile, Height*GridObject.HeightOfTile);
       
        imageUI.GetComponent< RectTransform >().rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
    }

    public void TextControl()
    {
        amountText.text = ItemAmount.ToString();

        if (ItemAmount > 1  && !isHide) 
        {
            amountText.gameObject.SetActive(true);
        }
        else
        {
            amountText.gameObject.SetActive(false);
        }
    }

    public Coroutine blinkCoroutine = null;

    public void StartBlink()
    {
        if(blinkCoroutine != null)
        {
            StopBlink();
        }

        blinkCoroutine = StartCoroutine(BlinkEffect());
    }

    public void StopBlink()
    {
        StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
    }
    public IEnumerator BlinkEffect()
    {
        float elapsedTime = 0f;
        float transitionTime = 1f;
        imageUI.color = Color.red;
        while (elapsedTime < transitionTime)
        {
            imageUI.color = Color.Lerp(Color.red, Color.white, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        imageUI.color = Color.white;
    }

    public void DestroyItem()
    {
        Managers.Resource.Destroy(gameObject);
    }

}
