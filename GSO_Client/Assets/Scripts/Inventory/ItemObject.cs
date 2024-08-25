using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;

public class ItemObject : MonoBehaviour
{
    public const int maxItemMergeAmount = 64;

    //�ڽİ�ü
    public Image imageUI;
    public TextMeshProUGUI amountText; //������ ���� �ؽ�Ʈ
    public TextMeshProUGUI unhideTimer; //������ ���� Ÿ�̸� �ؽ�Ʈ

    //������Ʈ
    public RectTransform itemRect;

    //�������� ��������Ʈ
    public List<Sprite> spriteList;
    private Sprite itemSprite;
    public Sprite hideSprite; //��ȸ ���� ������ ��������Ʈ

    public Coroutine searchingCoroutine;

    //������ ������ ���
    public ItemData itemData; //������(������ �ڵ�, �̸�, ��ȸ�ð�, ũ�� , �̹���)
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
            if (itemData.isSearched)
            {
                TextControl();
            }
        }
    }

    

    //�� ����
    public bool isHide; //������ ���� ������
    private bool isOnSearching; //������ ��ȸ��

    //���� ��ġ�� �׸���
    public GridObject curItemGrid;

    //��� ����
    public GridObject backUpItemGrid; //�������� ���� ������ �׸���
    public Vector2Int backUpItemPos; //�������� ���� ��ġ
    public int backUpItemRotate; //�������� ���� ȸ����

    //���� �ش� �������� ��ġ�� ����
    public EquipSlot curEquipSlot;
    //���������� ������ ����
    public EquipSlot backUpEquipSlot;


    private void Init()
    {
        itemRect = GetComponent<RectTransform>();
        imageUI = transform.GetChild(0).GetComponent<Image>(); 
        imageUI.raycastTarget = false;

        unhideTimer = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        unhideTimer.raycastTarget = false;
        unhideTimer.gameObject.SetActive(false);
        
        amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        amountText.raycastTarget = false;
        amountText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(searchingCoroutine != null)
        {
            StopCoroutine(searchingCoroutine);
            unhideTimer.gameObject.SetActive(false);
            searchingCoroutine = null;
            isOnSearching = false;
        }
    }

    /// <summary>
    /// �������� �����͸� ����
    /// </summary>
    public void SetItem(ItemData itemData)
    {
        Init();

        //������ ������ ������Ʈ
        this.itemData = itemData;
        itemRect = GetComponent<RectTransform>();
        itemSprite = FindItemSprtie(itemData.itemId);
        isOnSearching = false;
        ItemAmount = itemData.amount;

        //��ȸ�÷��̾� ����Ʈ�� ���Ե� �÷��̾� ���ο� ���� ����
        if (itemData.isSearched == false)
        {
            imageUI.sprite = hideSprite;
            isHide = true;
        }
        else
        {
            imageUI.sprite = itemSprite;
            isHide = false;
        }

        //������ ������Ʈ�� �ʱ� ũ�⸦ ����
        //�������� rotate�� 0�϶��� ���� �����ϰ� �� �Ŀ� rotate�� ����
        Vector2 size = new Vector2();
        size.X = itemData.width * GridObject.WidthOfTile;
        size.Y = itemData.height * GridObject.HeightOfTile;
        itemRect.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
        imageUI.GetComponent<RectTransform>().sizeDelta = itemRect.sizeDelta;

        Rotate(itemData.rotate);

        //�������� ��ġ�� ����
        itemRect.localPosition = new UnityEngine.Vector2(itemData.width * GridObject.WidthOfTile + 50, itemData.height * GridObject.HeightOfTile - 50);

        
    }

    private Sprite FindItemSprtie(int itemCode)
    {
        string spritePath = $"Assets/Sprite/ItemSprite/{itemCode}.png";
        Sprite itemSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        if (itemSprite == null)
        {
            Debug.LogError("Failed to load the sprite in editor!");
            return null;
        }

        Debug.Log("Sprite loaded successfully in editor!");
        // ��������Ʈ�� ����ϴ� ����
        return itemSprite;
    }

    /// <summary>
    /// ������ �������� Ŭ���� ��� ������ ��ȸ
    /// </summary>
    public void UnhideItem()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //�������� ��ȸ �ð����� ����� ������ ����
        searchingCoroutine = StartCoroutine(SearchingTimer(itemData.item_searchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        float timeRemaining = duration;

        unhideTimer.gameObject.SetActive(true);

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(timeRemaining);
            int milliseconds = Mathf.FloorToInt((timeRemaining - seconds) * 10); // One decimal place

            unhideTimer.text = string.Format("{0}:{1}", seconds, milliseconds);
            yield return null;
        }

        // Hide the timer text after the countdown is complete
        unhideTimer.gameObject.SetActive(false);

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

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateRight()
    {
        itemData.rotate = (itemData.rotate + 1) % 4;
        Rotate(itemData.rotate);
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateLeft()
    {
        itemData.rotate = (itemData.rotate - 1) % 4;
        Rotate(itemData.rotate);
    }

    /// <summary>
    /// �������� ȸ��. rect�� ȸ���ϸ� �̹��� ���� ȸ����.
    /// </summary>
    public void Rotate(int rotateInt)
    {
        itemRect.sizeDelta = new UnityEngine.Vector2(Width* GridObject.WidthOfTile, Height*GridObject.HeightOfTile);
       
        imageUI.GetComponent< RectTransform >().rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
        
    }

    public void MergeItem(ItemObject targetItem, int mergeAmount)
    {
        //�̹� ��ȿ�� üũ�� ������.
        targetItem.ItemAmount += mergeAmount;
        ItemAmount -= mergeAmount;

    }

    public void TextControl()
    {
        amountText.text = itemData.amount.ToString();

        //������ ���� �ؽ�Ʈ�� ��Ȱ��ȭ �����̰� 2 �̻��̸� Ȱ��ȭ
        if (itemData.amount > 1 && amountText.gameObject.activeSelf == false && !isHide) 
        {
            amountText.gameObject.SetActive(true);
        }
    }

    public void DestroyItem()
    {
        Managers.Resource.Destroy(gameObject);
    }
}
