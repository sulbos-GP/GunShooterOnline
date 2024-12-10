using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using static System.TimeZoneInfo;
using Vector2 = System.Numerics.Vector2;

public class ItemObject : MonoBehaviour
{
    public const int maxItemMergeAmount = 64;

    /// <summary>
    /// 아이템 데이터로 새로운 아이템 UI를 생성함
    /// </summary>
    public static ItemObject CreateNewItemObj(ItemData data, Transform parent = null)
    {
        ItemObject newItem = Managers.Resource.Instantiate("UI/InvenUI/ItemUI", parent).GetComponent<ItemObject>();
        newItem.SetItem(data);
        if (!InventoryController.instantItemDic.ContainsKey(data.objectId))
        {
            InventoryController.instantItemDic.Add(data.objectId, newItem);
        }

        return newItem;
    }

    /// <summary>
    /// 해당 아이템의 현재 위치, 회전, 부모의 아이디를 백업
    /// </summary>
    public static void BackUpItem(ItemObject item)
    {
        item.backUpItemPos = item.itemData.pos; //현재 위치
        item.backUpItemRotate = item.itemData.rotate; //현재 회전
        item.backUpParentId = item.parentObjId; //현재 부모 오브젝트
    }


    //자식 컴포넌트
    public Image imageUI;
    public TextMeshProUGUI amountText; //아이템 갯수 텍스트
    public TextMeshProUGUI searchTimerUI; //아이템 수색 타이머 텍스트

    //컴포넌트
    public RectTransform itemRect;

    //아이템의 스프라이트
    private Sprite itemSprite;
    public Sprite hideSprite; //조회 전에 보여질 스프라이트

    public Coroutine searchingCoroutine;

    //아이템 데이터 요소
    public ItemData itemData; //데이터(아이템 코드, 이름, 조회시간, 크기 , 이미지)
    public Vector2Int backUpItemPos; //아이템의 원래 위치
    public int backUpItemRotate; //아이템의 원래 회전도

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

    //현 상태
    public bool isHide; //아이템 정보 숨겨짐
    public bool isOnSearching; //아이템 조회중

    public double itemWeight; //지금까지 쓰인 itemData.itme_weight를  이변수로 바꿀것

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

    /// <summary>
    /// 아이템의 데이터를 적용
    /// </summary>
    public void SetItem(ItemData itemData)
    {
        Init();

        //아이템 데이터 업데이트
        this.itemData = itemData;
        itemRect = GetComponent<RectTransform>();
        itemSprite = FindItemSprtie(itemData);
        isOnSearching = false;
        ItemAmount = itemData.amount;

        //클라 입장에서 이미 조회된 데이터라면 해당 아이템의 이미지로 아니면 숨김 이미지로
        if (itemData.isSearched == false)
        {
            imageUI.sprite = hideSprite;
            isHide = true;
        }
        else
        {
            imageUI.sprite = itemSprite != null ?  itemSprite : hideSprite;
            isHide = false;
        }

        //아이템 오브젝트의 초기 크기를 지정
        //아이템의 rotate가 0일때로 먼저 생성하고 그 후에 rotate로 변경
        Vector2 size = new Vector2();
        size.X = itemData.width * GridObject.WidthOfTile; //그리드 1칸의 크기 * 높이,너비
        size.Y = itemData.height * GridObject.HeightOfTile;
        itemRect.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
        imageUI.GetComponent<RectTransform>().sizeDelta = itemRect.sizeDelta;

        Rotate(itemData.rotate);

        //아이템의 위치를 설정. 현재 앵커가 좌상단 기준임
        itemRect.localPosition = new UnityEngine.Vector2(itemData.width * GridObject.WidthOfTile + 50, itemData.height * GridObject.HeightOfTile - 50);
    }

    public static Sprite FindItemSprtie(ItemData itemData)
    {
        Sprite itemSprite = Resources.Load<Sprite>($"Sprite/Item/{itemData.iconName}");

        if (itemSprite == null)
        {
            Debug.Log("스프라이트 검색 실패");
            return null;
        }

        return itemSprite;
    }

    /// <summary>
    /// 가려진 아이템을 클릭한 경우 아이템 조회
    /// </summary>
    public void SearchItemHandler()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //아이템의 조회 시간동안 대기후 아이템 공개
        searchingCoroutine = StartCoroutine(SearchingTimer(itemData.item_searchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        float timeRemaining = duration;

        searchTimerUI.gameObject.SetActive(true);

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(timeRemaining);
            int milliseconds = Mathf.FloorToInt((timeRemaining - seconds) * 10); // One decimal place

            searchTimerUI.text = string.Format("{0}:{1}", seconds, milliseconds);
            yield return null;
        }

        RevealItem();

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
        amountText.text = ItemAmount.ToString();

        //아이템 갯수 텍스트가 비활성화 상태이고 2 이상이면 활성화
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
            // 경과 시간에 따라 색상 변화
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
