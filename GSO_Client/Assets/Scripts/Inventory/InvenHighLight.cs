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

    private RectTransform highlightRect;
    public GameObject highlightPrefab;

    
    public void InstantHighlighter()
    {
        highlightObj = Managers.Resource.Instantiate("UI/InvenUI/Highlight", GameObject.Find("Canvas").transform);
        highlightRect = highlightObj.GetComponent<RectTransform>();
        highlightRect.GetComponent<Image>().raycastTarget = false;
        highlightObj.SetActive(false);
    }

    public void DestroyHighlighter()
    {
        highlightRect = null;
        Managers.Resource.Destroy(highlightObj);
    }

    /// <summary>
    /// 하이라이팅의 액티브 여부
    /// </summary>
    /// <param name="tf">액티브 여부</param>
    public void Show(bool tf)
    {
        if (highlightRect == null)
        {
            InstantHighlighter();
        }
        highlightRect.gameObject.SetActive(tf);
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
        
        highlightRect.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
    }

    /// <summary>
    /// 하이라이트의 위치를 해당 아이템의 위치로
    /// </summary>
    public void SetPositionOnGrid(GridObject targetGrid, ItemObject targetItem)
    {
        SetHighlightParent(targetGrid.gameObject);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem,
            targetItem.itemData.pos.x, targetItem.itemData.pos.y);
        
        highlightRect.localPosition = new UnityEngine.Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// 기존 setPosition에서 위치를 직접 지정
    /// </summary>
    public void SetPositionOnGridByPos(GridObject targetGrid, ItemObject targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);
        
        highlightRect.localPosition = new UnityEngine.Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// 하이라이트의 부모UI 지정.
    /// </summary>
    public void SetHighlightParent(GameObject target)
    {
        if (!highlightObj.activeSelf || highlightObj == null)
        {
            return;
        }

        if (target == null)
        {
            highlightObj.transform.SetParent(GameObject.Find("Canvas").transform);
            return;
        }

        highlightObj.transform.SetParent(target.transform);
    }

    internal void SetColor(Color32 color)
    {
        highlightRect.gameObject.GetComponent<Image>().color = color;
    }
}
