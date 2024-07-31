using System;
using System.Collections;
using System.Collections.Generic;
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
    /*
     * �� �ڵ�� �κ��丮 ��Ʈ�ѷ��� �����Ǹ� ���̶���Ʈ UI ������Ʈ�� �����մϴ�.
     * ���̶���ƮUI ������Ʈ�� ���� ���� ���ٸ� �����մϴ�.
     * 
     * 1. Show�Լ��� ���̶���Ʈ�� ��Ƽ�� ���θ� �޾� setActive
     * 
     * 2. SetSize�Լ��� �������� ũ�⿡ ���� ���̶���Ʈ�� ����� �����մϴ�
     * 
     * 3. SetPosition�� SetPositionByPos�� ���̶���Ʈ�� ��ġ�� �̵������ָ� SetPosition�� 
     *    selectedItem�� ������ SetPositionByPos�´� selectedItem�� ������ ����մϴ�.
     *    
     * 4. SetParent�Լ��� ���̶���Ʈ�� �����ٴ� �׸����� �θ�ü�� ��ġ�ϰ� �Ͽ� �׻� �׸��� ����
     *    �������� �մϴ�.(�׸��� �ڿ� ���̶���Ʈ�� ����°��� ����)
     * 
     * 5. SetColor�Լ��� �Ű������� ���� ������ ���̶���Ʈ�� �̹��� ���� �����մϴ�.
     * 
     */
    private RectTransform highlighter;
    public GameObject highlightPrefab;

    private void Awake()
    {
        if(highlighter == null)
        {
            GameObject highlightObj = Instantiate(highlightPrefab);
            InventoryController.invenInstance.highlightObj = highlightObj;
            highlighter = highlightObj.GetComponent<RectTransform>();
            highlighter.GetComponent<Image>().raycastTarget = false;
        }
    }

    /// <summary>
    /// ���̶������� ��Ƽ�� ����
    /// </summary>
    /// <param name="tf">��Ƽ�� ����</param>
    public void Show(bool tf)
    {
        highlighter.gameObject.SetActive(tf);
    }

    /// <summary>
    /// ���̶���Ʈ�� ����� �ش� �������� ũ���
    /// </summary>
    /// <param name="targetItem">���ؾ�����</param>
    public void SetSize(ItemObject targetItem)
    {
        Vector2 size = new Vector2();
        size.X = targetItem.Width * InventoryGrid.WidthOfTile;
        size.Y = targetItem.Height* InventoryGrid.HeightOfTile;
        highlighter.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
    }

    /// <summary>
    /// ���̶���Ʈ�� ��ġ�� �ش� �������� ��ġ��
    /// </summary>
    public void SetPositionOnGrid(InventoryGrid targetGrid, ItemObject targetItem)
    {
        SetParent(targetGrid);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem,
            targetItem.curItemPos.x, targetItem.curItemPos.y);
        highlighter.localPosition = new UnityEngine.Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// ���� setPosition���� ��ġ�� ���� ����
    /// </summary>
    public void SetPositionOnGridByPos(InventoryGrid targetGrid, ItemObject targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highlighter.localPosition = new UnityEngine.Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// ���̶���Ʈ�� �θ�UI ����.
    /// </summary>
    /// <param name="targetGrid"></param>
    public void SetParent(InventoryGrid targetGrid)
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
