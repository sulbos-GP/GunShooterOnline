using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    /*
     * 이 코드는 item프리팹에 부착되며 아이템의 객체를 정의합니다. 
     * 아이템의 정보와 이 아이템의 현재 상태(위치? ,회전?, 가려짐? 등)
     * 
     * 1. Set 함수는 컨트롤러에서 생성과 동시에 불려지며 등록된 데이터를 현재 변수에 적용시키고
     *    회전과 숨김여부를 초기화합니다.
     *    
     * 2. SearchingItem 함수는 컨트롤러에서 해당 아이템 오브젝트를 클릭했을때 isHide가 true라면
     *    이 함수를 실행시킵니다. 코루틴을 사용하여 조회시간 후 isHide를 해제하고 아이템의 이미지
     *    를 바꾸게 됩니다.
     *    
     * 3. RotateRight, RotateLeft 함수는 컨트롤러에서 회전 명령을 줄때 사용합니다.
     *    rotated변수를 바꾸며 현재 RectTransform에 적용시킵니다.
    */


    public ItemObjData itemData; //데이터(아이템 코드, 이름, 조회시간, 크기 , 이미지)
    public Sprite hideSprite; //조회 전에 보여질 스프라이트

    //아이템의 회전에 따른 높이와 넓이 변화
    #region Height
    public int Height
    {
        get
        {
            if (curItemRotate % 2 == 1)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }
    #endregion
    #region Width
    public int Width
    {
        get
        {
            if (curItemRotate % 2 == 1)
            {
                return itemData.width;
            }
            return itemData.height;
        }
    }
    #endregion

    public bool ishide; //아이템 정보 숨겨짐
    private bool isOnSearching; //아이템 조회중
    public List<int> searchedPlayerId = new List<int>(); //이 리스트에 포함된 플레이어에겐 아이템이 보여짐

    //현재 아이템의 위치, 회전도, 그리드
    public ItemGrid curItemGrid;
    public Vector2Int curItemPos; //아이템의 기본위치
    public int curItemRotate = 0; // 1 : 0 or 360, 2: 90, 3 : 180, 4 : 270

    //백업 변수
    public ItemGrid backUpItemGrid; //아이템이 원래 보관된 그리드
    public Vector2Int backUpItemPos; //아이템의 원래 위치
    public int backUpItemRotate; //아이템의 원래 회전도

    /// <summary>
    /// 아이템의 데이터를 적용
    /// </summary>
    public void Set(ItemObjData itemData)
    {
        //아이템 데이터 업데이트
        this.itemData = itemData;
        //아이템 사이즈 업데이트
        Vector2 size = new Vector2();
        size.x = Width * ItemGrid.tilesizeWidth;
        size.y = Height * ItemGrid.tilesizeHeight;
        transform.GetComponent<RectTransform>().sizeDelta = size;

        //조회플레이어 리스트에 포함된 플레이어 여부에 따른 설정
        if (searchedPlayerId.Contains(InventoryController.invenInstance.playerId) == false)
        {
            transform.GetComponent<Image>().sprite = hideSprite;
            ishide = true;
            isOnSearching = false;
        }
        else
        {
            transform.GetComponent<Image>().sprite = itemData.itemSprite;
            ishide = false;
            isOnSearching = true;
        }
    }

    /// <summary>
    /// 가려진 아이템을 클릭한 경우 아이템 조회
    /// </summary>
    public void SearchingItem()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //아이템의 조회 시간동안 대기후 아이템 공개
        StartCoroutine(SearchingTimer(itemData.itemSearchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ishide = false;
        transform.GetComponent<Image>().sprite = itemData.itemSprite;
        searchedPlayerId.Add(InventoryController.invenInstance.playerId);
    }

    /// <summary>
    /// 아이템을 우회전
    /// </summary>
    public void RotateRight()
    {
        curItemRotate = (curItemRotate + 1) % 4;
        Rotate(curItemRotate);
    }

    /// <summary>
    /// 아이템을 좌회전
    /// </summary>
    public void RotateLeft()
    {
        curItemRotate = (curItemRotate - 1) % 4;
        Rotate(curItemRotate);
    }

    /// <summary>
    /// 실질적인 회전. rect가 회전하면 이미지 또한 회전함.
    /// </summary>
    public void Rotate(int rotateInt)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
    }
}
