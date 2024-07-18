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
            highlighter = highlightObj.GetComponent<RectTransform>();
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
    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.Width * ItemGrid.tilesizeWidth;
        size.y = targetItem.Height* ItemGrid.tilesizeHeight;
        highlighter.sizeDelta = size;
    }

    /// <summary>
    /// ���̶���Ʈ�� ��ġ�� �ش� �������� ��ġ��
    /// </summary>
    /// <param name="targetGrid">���� �׸���</param>
    /// <param name="target">���� ������</param>
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        SetParent(targetGrid);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem,
            targetItem.curItemPos.x, targetItem.curItemPos.y);
        highlighter.localPosition = pos;
    }

    /// <summary>
    /// ���� setPosition���� ��ġ�� ���� ����
    /// </summary>
    /// <param name="posX">x��ǥ</param>
    /// <param name="posY">y��ǥ</param>
    public void SetPositionByPos(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highlighter.localPosition = pos;
    }

    /// <summary>
    /// ���̶���Ʈ�� �θ�UI ����.
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
