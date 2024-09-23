using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;

public struct HighlightColor
{
    public static Color32 Green = new Color32(127, 255, 127, 125);
    public static Color32 Yellow = new Color32(255, 255, 127, 125);
    public static Color32 Red = new Color32(255, 127, 127, 125);
    public static Color32 Gray = new Color32(230, 230, 230, 125);
}

public class InvenHighLight : MonoBehaviour
{
    public static GameObject highlightObj;

    private RectTransform highlighter;
    public GameObject highlightPrefab;
    
    public void InstantHighlighter()
    {
        highlightObj = Managers.Resource.Instantiate("UI/InvenUI/Highlight");
        highlighter = highlightObj.GetComponent<RectTransform>();
        highlighter.GetComponent<Image>().raycastTarget = false;
    }

    public void DestroyHighlighter()
    {
        highlighter = null;
        Managers.Resource.Destroy(highlightObj);
    }

    /// <summary>
    /// 하이라이팅의 액티브 여부
    /// </summary>
    /// <param name="tf">액티브 여부</param>
    public void Show(bool tf)
    {
        if (highlighter == null)
        {
            InstantHighlighter();
        }
        highlighter.gameObject.SetActive(tf);
    }

    /// <summary>
    /// 하이라이트의 사이즈를 해당 아이템의 크기로
    /// </summary>
    /// <param name="targetItem">기준아이템</param>
    public void SetSize(ItemObject targetItem)
    {
        Vector2 size = new Vector2();
        size.X = targetItem.Width * GridObject.WidthOfTile;
        size.Y = targetItem.Height* GridObject.HeightOfTile;
        
        highlighter.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
    }

    /// <summary>
    /// 하이라이트의 위치를 해당 아이템의 위치로
    /// </summary>
    public void SetPositionOnGrid(GridObject targetGrid, ItemObject targetItem)
    {
        SetParent(targetGrid.gameObject);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem,
            targetItem.itemData.pos.x, targetItem.itemData.pos.y);
        
        highlighter.localPosition = new UnityEngine.Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// 기존 setPosition에서 위치를 직접 지정
    /// </summary>
    public void SetPositionOnGridByPos(GridObject targetGrid, ItemObject targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);
        
        highlighter.localPosition = new UnityEngine.Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// 하이라이트의 부모UI 지정.
    /// </summary>
    public void SetParent(GameObject target)
    {
        if (target == null)
        {
            highlighter.SetParent(GameObject.Find("Canvas").transform);
            return;
        }

        highlighter.SetParent(target.transform);
    }

    internal void SetColor(Color32 color)
    {
        highlighter.gameObject.GetComponent<Image>().color = color;
    }
}
