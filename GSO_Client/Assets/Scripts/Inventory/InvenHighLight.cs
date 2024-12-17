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
    public GameObject highlightObj;

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
    /// ���̶������� ��Ƽ�� ����
    /// </summary>
    /// <param name="tf">��Ƽ�� ����</param>
    public void Show(bool tf)
    {
        if (highlightRect == null)
        {
            InstantHighlighter();
        }
        highlightRect.gameObject.SetActive(tf);
    }

    /// <summary>
    /// ���̶���Ʈ�� ����� �ش� 
    /// </summary>
    public void SetSize(Vector2 _size)
    {
        Vector2 size = new Vector2();
        size.X = _size.X * GridObject.WidthOfTile;
        size.Y = _size.Y* GridObject.HeightOfTile;
        
        highlightRect.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
    }

    /// <summary>
    /// ���̶���Ʈ�� ��ġ�� �ش� �������� ��ġ��
    /// </summary>
    public void SetPositionOnGrid(GridObject targetGrid, ItemObject targetItem)
    {
        SetHighlightParent(targetGrid.gameObject);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem,
            targetItem.itemData.pos.x, targetItem.itemData.pos.y);
        
        highlightRect.localPosition = new UnityEngine.Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// ���� setPosition���� ��ġ�� ���� ����
    /// </summary>
    public void SetPositionOnGridByPos(GridObject targetGrid, ItemObject targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);
        
        highlightRect.localPosition = new UnityEngine.Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// ���̶���Ʈ�� �θ�UI ����.
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
