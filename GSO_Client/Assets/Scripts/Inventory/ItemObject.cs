using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;

public class ItemObject : MonoBehaviour
{
    /*
     * 이 코드는 item프리팹에 부착되며 아이템의 객체를 정의합니다. 
     * 아이템의 정보와 이 아이템의 현재 상태(위치? ,회전?, 가려짐? 등)
     * 
     * 1. ItemDataSet 함수는 컨트롤러에서 생성과 동시에 불려지며 등록된 데이터를 현재 변수에 적용시키고
     *    회전과 숨김여부를 초기화합니다.
     *    
     * 2. UnhideItem 함수는 컨트롤러에서 해당 아이템 오브젝트를 클릭했을때 isHide가 true라면
     *    이 함수를 실행시킵니다. 코루틴을 사용하여 조회시간 후 isHide를 해제하고 아이템의 이미지
     *    를 바꾸게 됩니다.
     *    
     * 3. RotateRight, RotateLeft 함수는 컨트롤러에서 회전 명령을 줄때 사용합니다.
     *    rotated변수를 바꾸며 현재 RectTransform에 적용시킵니다.
    */
    public const int maxItemMergeAmount = 64;

    public RectTransform itemRect;
    public ItemData itemData; //데이터(아이템 코드, 이름, 조회시간, 크기 , 이미지)
    public Sprite hideSprite; //조회 전에 보여질 스프라이트
    public List<Sprite> spriteList;

    //옵저버(변수 값의 변경을 즉각 탐지하여 함수실행)
    public int Width
    {
        get
        {
            if (itemData.itemRotate % 2 == 0)
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
            if (itemData.itemRotate % 2 == 0)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }
    public int ItemAmount
    {
        get { return itemData.itemAmount; }
        set
        {
            itemData.itemAmount = value;
            TextControl();
        }
    }

    public bool ishide; //아이템 정보 숨겨짐
    private bool isOnSearching; //아이템 조회중


    //현재 아이템의 위치, 회전도, 그리드
    public InventoryGrid curItemGrid;
    
    //백업 변수
    public InventoryGrid backUpItemGrid; //아이템이 원래 보관된 그리드
    public Vector2Int backUpItemPos; //아이템의 원래 위치
    public int backUpItemRotate; //아이템의 원래 회전도
    private Sprite itemSprite;

    private void Awake()
    {
        transform.GetComponent<Image>().raycastTarget = false;
        itemRect = GetComponent<RectTransform>();
    }
    /// <summary>
    /// 아이템의 데이터를 적용
    /// </summary>
    public void ItemDataSet(ItemData itemData)
    {
        //아이템 데이터 업데이트
        this.itemData = itemData;

        //오브젝트의 렉트의 사이즈 업데이트
        Vector2 size = new Vector2();
        size.X = Width * InventoryGrid.WidthOfTile;
        size.Y = Height * InventoryGrid.HeightOfTile;
        transform.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(size.X, size.Y);

        //아이템의 크기및 회전 설정
        itemRect.localPosition = new UnityEngine.Vector2(itemData.itemPos.x * InventoryGrid.WidthOfTile+50, itemData.itemPos.y* InventoryGrid.HeightOfTile-50);
        Rotate(itemData.itemRotate);
        ItemAmount = itemData.itemAmount;
        itemSprite = spriteList[itemData.itemCode - 1];
        //조회플레이어 리스트에 포함된 플레이어 여부에 따른 설정
        if (itemData.searchedPlayerId.Contains(InventoryController.invenInstance.playerId) == false)
        {
            transform.GetComponent<Image>().sprite = hideSprite;
            ishide = true;
            isOnSearching = false;
        }
        else
        {
            transform.GetComponent<Image>().sprite = itemSprite;
            ishide = false;
            isOnSearching = true;
        }
    }

    /// <summary>
    /// 가려진 아이템을 클릭한 경우 아이템 조회
    /// </summary>
    public void UnhideItem()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //아이템의 조회 시간동안 대기후 아이템 공개
        StartCoroutine(SearchingTimer(itemData.item_searchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ishide = false;

        
        transform.GetComponent<Image>().sprite = itemSprite;
        itemData.searchedPlayerId.Add(InventoryController.invenInstance.playerId);
        TextControl();
    }

    /// <summary>
    /// 아이템을 우회전
    /// </summary>
    public void RotateRight()
    {
        itemData.itemRotate = (itemData.itemRotate + 1) % 4;
        Rotate(itemData.itemRotate);
    }

    /// <summary>
    /// 아이템을 좌회전
    /// </summary>
    public void RotateLeft()
    {
        itemData.itemRotate = (itemData.itemRotate - 1) % 4;
        Rotate(itemData.itemRotate);
    }

    /// <summary>
    /// 실질적인 회전. rect가 회전하면 이미지 또한 회전함.
    /// </summary>
    public void Rotate(int rotateInt)
    {
        itemRect.rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
    }

    public void MergeItem(ItemObject targetItem, int mergeAmount)
    {
        if(itemData.itemCode == targetItem.itemData.itemCode)
        {
            targetItem.ItemAmount += mergeAmount;
            ItemAmount -= mergeAmount;
        }
    }

    public void TextControl()
    {
        TextMeshProUGUI amountText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        amountText.raycastTarget = false;
        if (itemData.itemAmount <= 1 || ishide) 
        {
            amountText.gameObject.SetActive(false);
            return; 
        }

        amountText.text = itemData.itemAmount.ToString();
        amountText.gameObject.SetActive(true);
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
