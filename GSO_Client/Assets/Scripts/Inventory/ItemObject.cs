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

    //자식객체
    public Image imageUI;
    public TextMeshProUGUI amountText; //아이템 갯수 텍스트
    public TextMeshProUGUI unhideTimer; //아이템 수색 타이머 텍스트

    //컴포넌트
    public RectTransform itemRect;

    //아이템의 스프라이트
    public List<Sprite> spriteList;
    private Sprite itemSprite;
    public Sprite hideSprite; //조회 전에 보여질 스프라이트

    public Coroutine searchingCoroutine;

    //아이템 데이터 요소
    public ItemData itemData; //데이터(아이템 코드, 이름, 조회시간, 크기 , 이미지)
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

    

    //현 상태
    public bool isHide; //아이템 정보 숨겨짐
    private bool isOnSearching; //아이템 조회중

    //현재 위치한 그리드
    public GridObject curItemGrid;

    //백업 변수
    public GridObject backUpItemGrid; //아이템이 원래 보관된 그리드
    public Vector2Int backUpItemPos; //아이템의 원래 위치
    public int backUpItemRotate; //아이템의 원래 회전도

    //현재 해당 아이템이 위치한 슬롯
    public EquipSlot curEquipSlot;
    //마지막으로 장착된 슬롯
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
    /// 아이템의 데이터를 적용
    /// </summary>
    public void SetItem(ItemData itemData)
    {
        Init();

        //아이템 데이터 업데이트
        this.itemData = itemData;
        itemRect = GetComponent<RectTransform>();
        itemSprite = FindItemSprtie(itemData.itemId);
        isOnSearching = false;
        ItemAmount = itemData.amount;

        //조회플레이어 리스트에 포함된 플레이어 여부에 따른 설정
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

        //아이템 오브젝트의 초기 크기를 지정
        //아이템의 rotate가 0일때로 먼저 생성하고 그 후에 rotate로 변경
        Vector2 size = new Vector2();
        size.X = itemData.width * GridObject.WidthOfTile;
        size.Y = itemData.height * GridObject.HeightOfTile;
        itemRect.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
        imageUI.GetComponent<RectTransform>().sizeDelta = itemRect.sizeDelta;

        Rotate(itemData.rotate);

        //아이템의 위치를 설정
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
        // 스프라이트를 사용하는 예시
        return itemSprite;
    }

    /// <summary>
    /// 가려진 아이템을 클릭한 경우 아이템 조회
    /// </summary>
    public void UnhideItem()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //아이템의 조회 시간동안 대기후 아이템 공개
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
    /// 아이템을 우회전
    /// </summary>
    public void RotateRight()
    {
        itemData.rotate = (itemData.rotate + 1) % 4;
        Rotate(itemData.rotate);
    }

    /// <summary>
    /// 아이템을 좌회전
    /// </summary>
    public void RotateLeft()
    {
        itemData.rotate = (itemData.rotate - 1) % 4;
        Rotate(itemData.rotate);
    }

    /// <summary>
    /// 실질적인 회전. rect가 회전하면 이미지 또한 회전함.
    /// </summary>
    public void Rotate(int rotateInt)
    {
        itemRect.sizeDelta = new UnityEngine.Vector2(Width* GridObject.WidthOfTile, Height*GridObject.HeightOfTile);
       
        imageUI.GetComponent< RectTransform >().rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
        
    }

    public void MergeItem(ItemObject targetItem, int mergeAmount)
    {
        //이미 유효성 체크가 성공함.
        targetItem.ItemAmount += mergeAmount;
        ItemAmount -= mergeAmount;

    }

    public void TextControl()
    {
        amountText.text = itemData.amount.ToString();

        //아이템 갯수 텍스트가 비활성화 상태이고 2 이상이면 활성화
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
