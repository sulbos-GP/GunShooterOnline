using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    /*
     * �� �ڵ�� Grid�� �����Ǹ� �������� �κ��丮�� �κ��Դϴ�. 
     * �κ��丮�� Ÿ��ũ��, ����, ��ġ ���� �����մϴ�.
     * 
     * 1. Init�Լ��� �׸��嵥���Ϳ��� x,y�� Ÿ�ϼ��� �޾� �κ��丮�� ũ�⸦ �����ϰ� �κ��丮
     *    �� ������ �ʱ�ȭ�մϴ�. OtherUISet�Լ��� ���� �ٸ� UI�鵵 �׸��忡 ���� �����մϴ�
     *    
     * 2. MouseToGridPosition�Լ��� ��Ʈ�ѷ����� ȣ��Ǹ� ���콺�� ��ġ�� �׸����� ��ǥ�� 
     *    �����Ͽ� �� ��ǥ�� �����մϴ�. Vector2 -> Vector2Int
     *    
     * 3. PickUpItem�Լ� �������� �ڵ��� �ϳ��̸� x,y �׸��� ��ǥ�� �޾� �ش� ��ǥ�� �������� 
     *    �����ϴ��� �˻��ϰ� �ִٸ� �ش� �������� ��ȯ�մϴ�.
     *    ����� �ڵ�� GetItem�� �ִµ� PickUpItem�� �ش� ��ġ�� �����ϸ� ������ ������ ������
     *    �������� GetItem�� ������ ����°� ���� ���� �������� �ִ����� Ȯ���ϴ� �뵵�Դϴ�.
     * 
     * 4. CleanItemReference�� �Ű������� �޾ƿ� �������� ��ġ�� ũ�⸦ ���� inventoryItemSlot
     *    �� ������ �����մϴ�.
     *    
     * 5. PlaceItemCheck�� �������� �ڵ��� �ϳ��� �ش� ��ġ�� ��ġ�� ��������InsideGridCheck(�ش� 
     *    ��ġ�� �׸����� ������),BoundaryCheck(�������� ũ�Ⱑ �׸��带 �Ѿ����),
     *    CheckAvailableSpace(������ ũ�Ⱑ�������� ũ�⸸ŭ ū��) Ȯ���ϰ�
     *    
     *    ��ġ�� �������� 1���ų� ��ġ�� �����ϴٸ� PlaceItem�� ���� �������� ��ġ�ϰ� true�� ��ȯ�ϸ�
     *    ��ġ�� �������� 2�� �̻��̰ų� ��ġ�� �Ұ����ϴٸ� �״�� false�� �����մϴ�.
     *    ���� ������ inventoryItemSlot���� ��ġ�̹� PlaceSprite�� ���� �ش� ��ġ�� ���� 
     *    �������� ��ġ�� �̵���ŵ�ϴ�.
     * 
     * 6. CalculatePositionOnGrid�� ������ ��ü�� �ش� ��ǥ�� �̵���Ű�� ���� �׸��� ���� ��ǥ��
     *    ������ġ�� �ٲ��ݴϴ�.
     *    
     * 7. UpdateBackupSlot�� UndoItemSlot�� ����� ����ϸ� ���������� ��ġ�ɶ� ���� 
     *    inventoryItemSlot�� ������ savedInvenItemSlot�� ���� ��ġ�� �����ϸ� inventoryItemSlot��
     *    savedInvenItemSlot�� �����ߴٸ� �� �ݴ�� ����� �˴ϴ�.
     *    
     * 8. PlaceCheckForHighlightColor�� ��ġ ���ɿ��ο� ���� ���̶���Ʈ ������ ��ȯ�մϴ�.
     *    PlaceItemCheck�� �����մϴٸ� �������������� ������ ������ �ʾ� ������ ���� ����� �ƴϴ�
     *    �����ؾ��մϴ�.
     */

    public int gridId = 0; // ���� �Ҵ�
    public InventoryItem[,] inventoryItemSlot; //�κ��丮 ������ ��ϵ� �迭

    //Ÿ���� ũ�� offset
    public const float tilesizeWidth = 100;
    public const float tilesizeHeight = 100;

    //�׸��� ����
    private RectTransform rectTransform; //�ش� �׸����� transform
    private Vector2Int gridSize; //�׸����� ��,���̺� Ÿ�� ���� �Է�
    private Vector2 positionOnTheGrid = new Vector2(); //�׸��� ���� ���콺 ��ġ
    private Vector2Int tileGridPosition = new Vector2Int(); //���콺 �Ʒ��� Ÿ�� ��ġ��ǥ
    public GridObjectData gridObjData; // �ش� �׸����� ������(�׸��� ũ��)

    //��� ����
    public InventoryItem[,] savedInvenItemSlot; //��� �迭
    [SerializeField]private GameObject inventoryItemPref; //�������� prefab

    //�ٸ� �ش� �κ��丮�� �ٸ� UI
    public GameObject BgPanel;
    public GameObject HeaderPanel;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gridSize = gridObjData.gridSize;
        if (gridSize.x <= 0 || gridSize.y <= 0)
        {
            //�׸��� ����� �������� ������ �⺻������ 5,5 �Ҵ�
            gridSize = new Vector2Int(5, 5);
        }

        Init(gridSize.x, gridSize.y);
    }

    /// <summary>
    /// �ش� �׸����� �κ��丮 ������ �迭 ���� �� ������ �Ҵ�
    /// </summary>
    /// <param name="width">�׸����� ��</param>
    /// <param name="height">�׸����� ����</param>
    private void Init(int width, int height)
    {
        //Init�� �κ��丮���� ������ �޾Ƽ� �ݿ��ϴ� �ڵ� ©����
        inventoryItemSlot = new InventoryItem[width, height];
        savedInvenItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tilesizeWidth, height * tilesizeHeight);
        rectTransform.sizeDelta = size;
        OtherUISet();
    }

    /// <summary>
    /// ���� ��� UI ������Ʈ�� ��ġ�� ũ�� ����.
    /// </summary>
    private void OtherUISet()
    {
        RectTransform bgRect = BgPanel.GetComponent<RectTransform>();
        RectTransform headerRect = HeaderPanel.GetComponent<RectTransform>();
        int offsetX = 10;
        int offsetY = 10;

        bgRect.sizeDelta = new Vector2(rectTransform.sizeDelta.x + offsetX * 2, rectTransform.sizeDelta.y + offsetY * 2);
        bgRect.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x - offsetX, rectTransform.anchoredPosition.y + offsetY);
        bgRect.localScale = Vector3.one;

        // ��� �г��� ��ġ ����
        headerRect.sizeDelta = new Vector2(rectTransform.sizeDelta.x, headerRect.sizeDelta.y);
        headerRect.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + headerRect.sizeDelta.y + offsetY);
    }

    /// <summary>
    /// ���콺�� �� ��ġ�� �׸����� Ÿ�� ��ġ�� ��ȯ
    /// </summary>
    /// <param name="mousePosition">���콺 ��ġ</param>
    public Vector2Int MouseToGridPosition(Vector2 mousePosition)
    {
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;
        tileGridPosition.x = (int)(positionOnTheGrid.x / tilesizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tilesizeHeight);
        return tileGridPosition;
    }

    /// <summary>
    /// �ش� ��ǥ�� �ִ� ������ ��ȯ
    /// </summary>
    public InventoryItem GetItem(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return null;
        }
        return inventoryItemSlot[x, y];
    }

    /// <summary>
    /// �ش� ��ǥ�� Ÿ�Ͽ� �������� �ִ��� �˻� �� �ش� �������� ����
    /// </summary>
    public InventoryItem PickUpItem(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return null;
        }

        InventoryItem targetItem = inventoryItemSlot[x, y];

        if (targetItem == null) { return null; }
        CleanItemReference(targetItem);

        PrintInventoryContents(inventoryItemSlot);
        return targetItem;
    }

    /// <summary>
    /// ������ ũ�⿡ ���� �κ��丮 ������ ������ ����
    /// </summary>
    /// <param name="item">���õ� ������</param>
    private void CleanItemReference(InventoryItem item)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                inventoryItemSlot[item.curItemPos.x + x, item.curItemPos.y + y] = null;
            }
        }
    }

    
    /// <summary>
    /// �������� ��ġ �������� ������ üũ �� �迭�� ������ ��ġ
    /// </summary>
    public bool PlaceItemCheck(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        //�׸��� ������ �������� üũ
        if (BoundaryCheck(posX, posY, inventoryItem.Width, inventoryItem.Height) == false)
        {
            return false;
        }

        //��ġ�� �������� �ִٸ� overlapItem���� �ʱ�ȭ
        if (OverLapCheck(posX, posY, inventoryItem.Width, inventoryItem.Height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        //�������� �������� �ִٸ� �ش� ������ �������� �迭���� ����
        if (overlapItem != null)
        {
            if (overlapItem.ishide == true)
            {
                overlapItem = null;
                return false;
            }
            CleanItemReference(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);
        PrintInventoryContents(inventoryItemSlot);
        
        return true;
    }

    /// <summary>
    /// �������� ���� PlaceItemCheck. ���� ��ġ������ ������ ���� ����
    /// </summary>
    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        //�ش� �������� ũ���� ��ǥ�� ����
        for (int x = 0; x < inventoryItem.Width; x++)
        {
            for (int y = 0; y < inventoryItem.Height; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }
        PlaceSprite(inventoryItem, posX, posY, rectTransform);
    }

    public void PlaceSprite(InventoryItem inventoryItem, int posX, int posY, RectTransform rectTransform)
    {
        //�̹��� ��������Ʈ�� ����
        inventoryItem.curItemPos = new Vector2Int(posX, posY);
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
        rectTransform.localPosition = position;
    }

    /// <summary>
    /// �׸��� ���� ������ġ ���. �������� �ش� ��ġ�� ������Ű�� ����
    /// </summary>
    /// <param name="inventoryItem">�ش� ������</param>
    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tilesizeWidth + tilesizeWidth * inventoryItem.Width / 2;
        position.y = -(posY * tilesizeHeight + tilesizeHeight * inventoryItem.Height / 2);
        return position;
    }

    public void UpdateBackupSlot()
    {
        
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                savedInvenItemSlot[i, j] = inventoryItemSlot[i, j];
            }
        }
    }

    /// <summary>
    /// ������ ������ ����� �������� �ǵ���.
    /// </summary>
    public void UndoItemSlot()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                inventoryItemSlot[i, j] = savedInvenItemSlot[i, j];
            }
        }
    }

    public bool OverLapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        //������ ���� ��ȸ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //�ش� ������ ������ ������� ���� ��� 
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    //��ġ�� �������� 1�� ��쿡�� ���þ����۰� ��ü �� �̻��̶�� Place �Ұ�
                    if (overlapItem == null)
                    {
                        //�ش� ��ǥ�� �������� ���������������� ����
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        //�̹� ������ �������� �����Ǿ� �ִµ� �ٸ� �������� �� �������Ǹ� ���� false;
                        if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    

    /// <summary>
    /// InventoryItemSlot�� �����.
    /// </summary>
    public void PrintInventoryContents(InventoryItem[,] slots)
    {
        string content = "";
        for (int i = 0; i < inventoryItemSlot.GetLength(1); i++)
        {
            for (int j = 0; j < inventoryItemSlot.GetLength(0); j++)
            {
                InventoryItem item = inventoryItemSlot[j, i];
                if (item != null)
                {
                    content += $"| {item.itemData.name} |";
                }
                else
                {
                    content += $"| Null |";
                }
            }
            content += "\n";
        }

        Debug.Log(content);
    }

    /// <summary>
    /// �������� ũ�⸸ŭ �� ��Ҹ� ã��
    /// ?�� �� ������ ������ ���ϰ��� �ΰ��� ����ϱ� ����
    /// </summary>
    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int width = gridSize.x - (itemToInsert.Width - 1);
        int height = gridSize.y - (itemToInsert.Height - 1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemToInsert.Width, itemToInsert.Height) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    /// <summary>
    /// �ش� ��ġ�� grid�� ������ üũ
    /// </summary>
    private bool InsideGridCheck(int posX, int posY)
    {
        if (posX < 0 || posY < 0)
        {
            return false;
        }
        if (posX >= gridSize.x || posY >= gridSize.y)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// �������� ũ�Ⱑ �׸��带 ������������ üũ
    /// </summary>
    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        if (InsideGridCheck(posX, posY) == false) { return false; }
        if (InsideGridCheck(posX += (width - 1), posY += (height - 1)) == false) { return false; }
        return true;
    }

    /// <summary>
    /// �ش� �����ȿ� �������� ũ�⸸ŭ�� ������ �Ǵ��� Ȯ��
    /// </summary>
    public bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        //������ ���� ��ȸ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //�ش� ������ ������ ������� ���� ��� 
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// ��Ʈ�ѷ��� ������Ʈ���� ����
    /// �ش� �׸��� ���� �������� ��ġ�� �������� ���ο� ���� ���� ��ȯ
    /// </summary>
    public Color32 PlaceCheckForHighlightColor(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem checkOverlap)
    {
        //�׸��� ������ �������� üũ
        if (BoundaryCheck(posX, posY, inventoryItem.Width, inventoryItem.Height) == false)
        {
            return HighlightColor.Red;
        }

        //��ġ�� �������� �ΰ� �̻� �ִٸ� overlapItem���� �ʱ�ȭ�� false ��ȯ
        if (OverLapCheck(posX, posY, inventoryItem.Width, inventoryItem.Height, ref checkOverlap) == false)
        {
            //������ üũ���� ��ǥ�� �����۽����� �˻��ϴ� �������� �� ��ǥ �߻����� ���� �߻� �̸� �ذ��Ұ�
            checkOverlap = null;
            return HighlightColor.Red;
        }

        //�������� �������� ������ ��ġ�� ������ ���
        if (checkOverlap != null)
        {
            //�� ������ �������� ishide�� ��� false
            if (checkOverlap.ishide == true)
            {
                checkOverlap = null;
                return HighlightColor.Red;
            }
            return HighlightColor.Yellow;
        }
        return HighlightColor.Green;
    }
}
