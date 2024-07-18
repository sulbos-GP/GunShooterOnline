using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct HighlightColor
{
    public static Color32 Green = new Color32(127, 255, 127, 125);
    public static Color32 Yellow = new Color32(255, 255, 127, 125);
    public static Color32 Red = new Color32(255, 127, 127, 125);
    public static Color32 Gray = new Color32(230, 230, 230, 125);
}

public class InvenHighLight : MonoBehaviour
{
    /*
     * 이 코드는 인벤토리 컨트롤러에 부착되며 하이라이트 UI 오브젝트를 관리합니다.
     * 하이라이트UI 오브젝트가 게임 내에 없다면 생성합니다.
     * 
     * 1. Show함수는 하이라이트의 액티브 여부를 받아 setActive
     * 
     * 2. SetSize함수는 아이템의 크기에 맞춰 하이라이트의 사이즈를 조절합니다
     * 
     * 3. SetPosition과 SetPositionByPos는 하이라이트의 위치를 이동시켜주며 SetPosition은 
     *    selectedItem이 없을때 SetPositionByPos는는 selectedItem이 있을때 사용합니다.
     *    
     * 4. SetParent함수는 하이라이트를 가져다댄 그리드의 부모객체에 위치하게 하여 항상 그리드 위에
     *    나오도록 합니다.(그리드 뒤에 하이라이트가 생기는것을 방지)
     * 
     * 5. SetColor함수는 매개변수로 받은 색으로 하이라이트의 이미지 색을 지정합니다.
     * 
     */
    private RectTransform highlighter;
    public GameObject highlightPrefab;

    private void Awake()
    {
        if(highlighter == null)
        {
            GameObject highlightObj = Instantiate(highlightPrefab);
            highlighter = highlightObj.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// 하이라이팅의 액티브 여부
    /// </summary>
    /// <param name="tf">액티브 여부</param>
    public void Show(bool tf)
    {
        highlighter.gameObject.SetActive(tf);
    }

    /// <summary>
    /// 하이라이트의 사이즈를 해당 아이템의 크기로
    /// </summary>
    /// <param name="targetItem">기준아이템</param>
    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.Width * ItemGrid.tilesizeWidth;
        size.y = targetItem.Height* ItemGrid.tilesizeHeight;
        highlighter.sizeDelta = size;
    }

    /// <summary>
    /// 하이라이트의 위치를 해당 아이템의 위치로
    /// </summary>
    /// <param name="targetGrid">기준 그리드</param>
    /// <param name="target">기준 아이템</param>
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        SetParent(targetGrid);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem,
            targetItem.curItemPos.x, targetItem.curItemPos.y);
        highlighter.localPosition = pos;
    }

    /// <summary>
    /// 기존 setPosition에서 위치를 직접 지정
    /// </summary>
    /// <param name="posX">x좌표</param>
    /// <param name="posY">y좌표</param>
    public void SetPositionByPos(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highlighter.localPosition = pos;
    }

    /// <summary>
    /// 하이라이트의 부모UI 지정.
    /// </summary>
    /// <param name="targetGrid"></param>
    public void SetParent(ItemGrid targetGrid)
    {
        if(targetGrid == null)
        {
            return;
        }
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }

    internal void SetColor(Color32 color)
    {
        highlighter.gameObject.GetComponent<Image>().color = color;
    }
}
