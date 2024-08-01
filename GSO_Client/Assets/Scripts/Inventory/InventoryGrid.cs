using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Random = UnityEngine.Random;
using Google.Protobuf.Protocol;


public class InventoryGrid : MonoBehaviour
{
    /*
     * �׸��忡 ���� ������ ������
     * 
     * 
     */

    //�׸��� ����
    public GridData gridData; //�κ��丮���� �Ҵ�� �׸����� ������

    public int gridId = 0; // ���� �Ҵ�. �������� ���� �׸��� �������� id

    public ItemObject[,] gridItemArray; //�κ��丮 ������ ��ϵ� �迭
    public Inventory invenScr; //�ش� �׸��带 �����ϴ� �κ��丮�� ��ũ��Ʈ

    public float gridWeight = 0; //�׸������ �������� ����

    //gridWeight ������. ȣ��� gridWeight ������Ʈ �� �κ��丮�� ���Ե� ������Ʈ
    public float GridWeight
    {
        get => gridWeight;
        set 
        { 
            gridWeight = value;
            invenScr.UpdateInvenWeight();
        }
    }

    //Ÿ���� ũ�� offset
    public const float WidthOfTile = 100;
    public const float HeightOfTile = 100;

    private RectTransform gridRect; //�ش� �׸����� transform
    private Vector2Int gridSize; //�׸����� ��,���̺� Ÿ�� ���� �Է�
    private Vector2 mousePosOnGrid = new Vector2(); //�׸��� ���� ���콺 ��ġ
    private Vector2Int tileGridPos = new Vector2Int(); //���콺 �Ʒ��� Ÿ�� ��ġ��ǥ

    //��� ����
    public ItemObject[,] gridBackupArray; //��� �迭
    public float backupWeight;

    //������ ����
    public GameObject itemPref; //�������� prefab
    private bool createRandomItem;

    private void Start()
    {
        gridRect = GetComponent<RectTransform>();
        if(gridData == null)
        {
            Debug.Log("�ش� �׸��� ������Ʈ�� �׸��� �����Ͱ� ����");
            return;
        }
        gridId = gridData.gridId;
        gridSize = gridData.gridSize;
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
        invenScr = transform.parent.parent.GetComponent<Inventory>();
        gridItemArray = new ItemObject[width, height];
        gridBackupArray = new ItemObject[width, height];
        
        Vector2 size = new Vector2(width * WidthOfTile, height * HeightOfTile);
        gridRect.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
        createRandomItem = gridData.createRandomItem;

        GridSet();
    }

    private void GridSet()
    {
        //�׸��尡 ���� �� ���� �ʰ� �ణ�� �������� ��
        Vector2 offsetGridPosition = new Vector2(gridData.gridPos.X + Inventory.offsetX, gridData.gridPos.Y - Inventory.offsetY);
        transform.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(offsetGridPosition.X, offsetGridPosition.Y);
    
        //�׸����� ��ġ�� �Ϸ�Ǹ� �׸����� �����۽����� �����ϰ� ������ ������ ���� ��ġ
        GridItemSet();
    }

    private void GridItemSet()
    {
        //�������� ���� ����Ʈ�� gridItemArray�� �Ҵ��ϰ�
        //������ �������� ������ �ش� �������� �����͸� �־��ֱ�
        if(gridData.itemList.Count == 0)
        {
            //�� �׸��忡 ������ �����Ͱ� ������� ���ǿ� ���� ������ ������ ��ġ
            if (createRandomItem)
            {
                for(int i =0; i< gridData.randomItemAmount; i++)
                {
                    if (RestSlotCheck() == 0)
                    {
                        break;
                    }
                    InstantRandomItem();
                }
                createRandomItem = false;
            }
            else
            {
                Debug.Log("����ִ� �׸���");
            }
        }
        else
        {
            //�׸��忡 ������ ����Ʈ�� �ִ� ���
            foreach(ItemData item in gridData.itemList)
            {
                CreateItemObj(item);
            }
        }

        UpdateBackupSlot();
    }

    private int RestSlotCheck()
    {
        int restSlot = 0;
        for (int x = 0; x < gridData.gridSize.x; x++)
        {
            for (int y = 0; y < gridData.gridSize.y; y++)
            {
                if (gridItemArray[x, y] == null)
                {
                    restSlot++;
                }
            }
        }

        return restSlot;
    }

    private void InstantRandomItem()
    {
        //�ӽ�(�����ͺ��̽� ������ ����)
        Debug.Log($"���� ���� {transform.name}");
        //InventoryController.invenInstance.InsertRandomItem(this);
        
        ItemObject randomItem = Instantiate(itemPref).GetComponent<ItemObject>();
        InventoryController.invenInstance.SetSelectedObjectToLastSibling(transform);
        int randomId = Random.Range(0, InventoryController.invenInstance.itemsList.Count);
        randomItem.Set(InventoryController.invenInstance.itemsList[randomId]);

        FindPlaceableSlot(randomItem);
        randomItem.curItemGrid = this;

        randomItem.backUpItemPos = randomItem.curItemPos; //���� ��ġ
        randomItem.backUpItemRotate = randomItem.curItemRotate; //���� ȸ��
        randomItem.backUpItemGrid = randomItem.curItemGrid; //���� �׸���
    }

    private void FindPlaceableSlot(ItemObject item)
    {
        Vector2Int? posOnGrid = FindSpaceForObject(item);
        
        if (posOnGrid == null)
        {
            item.RotateRight();
            posOnGrid = FindSpaceForObject(item);
            if (posOnGrid == null)
            {
                return;
            }
        }

        PlaceItem(item, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void CreateItemObj(ItemData item)
    {
        ItemObject itemObj = Instantiate(itemPref, transform).GetComponent<ItemObject>();
        itemObj.Set(item);
        PlaceItem(itemObj , item.itemPos.x, item.itemPos.y);
        itemObj.curItemGrid = this;

        itemObj.backUpItemPos = itemObj.curItemPos; //���� ��ġ
        itemObj.backUpItemRotate = itemObj.curItemRotate; //���� ȸ��
        itemObj.backUpItemGrid = itemObj.curItemGrid; //���� �׸���
    }

    /// <summary>
    /// ���콺�� �� ��ġ�� �׸����� Ÿ�� ��ġ�� ��ȯ
    /// </summary>
    /// <param name="mousePosition">���콺 ��ġ</param>
    public Vector2Int MouseToGridPosition(Vector2 mousePosition)
    {
        mousePosOnGrid.X = mousePosition.X - gridRect.position.x;
        mousePosOnGrid.Y = gridRect.position.y - mousePosition.Y;
        tileGridPos.x = (int)(mousePosOnGrid.X / WidthOfTile);
        tileGridPos.y = (int)(mousePosOnGrid.Y / HeightOfTile);
        return tileGridPos;
    }

    /// <summary>
    /// �ش� ��ǥ�� �ִ� ������ ��ȯ
    /// </summary>
    public ItemObject GetItem(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return null;
        }
        return gridItemArray[x, y];
    }

    /// <summary>
    /// �ش� ��ǥ�� Ÿ�Ͽ� �������� �����ϸ� �迭�󿡼��� ����
    /// </summary>
    public ItemObject PickUpItem(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return null;
        }

        ItemObject targetItem = gridItemArray[x, y];
        
        if (targetItem == null) { return null; }

        gridWeight -= targetItem.itemData.item_weight;
        invenScr.UpdateInvenWeight();
        CleanItemSlot(targetItem);
        PrintInvenContents(this,gridItemArray);

        return targetItem;
    }

    /// <summary>
    /// �ش� ������ ũ�⿡ ���� �κ��丮 ������ ������ ����
    /// </summary>
    /// <param name="item">���õ� ������</param>
    public void CleanItemSlot(ItemObject item)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                gridItemArray[item.curItemPos.x + x, item.curItemPos.y + y] = null;
            }
        }
    }

    
    /// <summary>
    /// �������� ��ġ �������� ������ üũ �� �迭�� ������ ��ġ
    /// </summary>
    public bool PlaceItemCheck(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        if(InventoryController.invenInstance.SelectedItemGrid == null)
        {
            return false;
        }

        //�������� �׸��� ������ ������ ���
        if (BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height) == false)
        {
            return false;
        }

        //��ġ�� �������� �ִٸ� overlapItem������ �Ҵ�. ���⼭���� ������ ������ ������
        if (OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        //���Կ� ���� ��ġ�� �Ұ����� ��� ���
        if (invenScr.CheckingInvenWeight(placeItem.itemData.item_weight, overlapItem) == false)
        {
            return false;
        }

        //�������� �������� �ִٸ�
        if (overlapItem != null)
        {
            //���� �Ҹ�ǰ�� ��� ��ġ�� ���� ���� ä�� true ����
            if (placeItem.itemData.isItemConsumeable && 
                (placeItem.itemData.itemCode == overlapItem.itemData.itemCode)&&
                overlapItem.itemAmount < 64)
            {
                return true;
            }

            overlapItem = null;
            return false;
        }
        
        //������ �迭�� �ش� ������ ��ġ
        PlaceItem(placeItem, posX, posY);
        PrintInvenContents(this, gridItemArray);
        
        return true;
    }

    /// <summary>
    /// �迭�� ������ ���� �� �ش� ������ UI ��ü �̵�
    /// </summary>
    public void PlaceItem(ItemObject item, int posX, int posY)
    {
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.SetParent(gridRect);
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                gridItemArray[posX + x, posY + y] = item;
            }
        }

        gridWeight += item.itemData.item_weight;
        invenScr.UpdateInvenWeight();

        PlaceSprite(item, posX, posY, itemRect);
    }

    /// <summary>
    /// �������� ��Ʈ�� �ش���ġ�� �̵�
    /// </summary>
    public void PlaceSprite(ItemObject inventoryItem, int posX, int posY, RectTransform itemRect)
    {
        inventoryItem.curItemPos = new Vector2Int(posX, posY);
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
        itemRect.localPosition = new UnityEngine.Vector2(position.X,position.Y);
    }

    /// <summary>
    /// �׸��� ���� ������ġ ���. �������� �ش� ��ġ�� ������Ű�� ����
    /// </summary>
    /// <param name="inventoryItem">�ش� ������</param>
    public Vector2 CalculatePositionOnGrid(ItemObject inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.X = posX * WidthOfTile + WidthOfTile * inventoryItem.Width / 2;
        position.Y = -(posY * HeightOfTile + HeightOfTile * inventoryItem.Height / 2);
        return position;
    }

    public void UpdateBackupSlot()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                gridBackupArray[i, j] = gridItemArray[i, j];
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
                gridItemArray[i, j] = gridBackupArray[i, j];
            }
        }
    }

    public bool OverLapCheck(int posX, int posY, int width, int height, ref ItemObject overlapItem)
    {
        //������ ���� ��ȸ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //�ش� ������ ������ ������� ���� ��� 
                if (gridItemArray[posX + x, posY + y] != null)
                {
                    //��ġ�� �������� 1�� ��쿡�� ���þ����۰� ��ü �� �̻��̶�� Place �Ұ�
                    if (overlapItem == null)
                    {
                        //�ش� ��ǥ�� �������� ���������������� ����
                        overlapItem = gridItemArray[posX + x, posY + y];
                    }
                    else
                    {
                        //�̹� ������ �������� �����Ǿ� �ִµ� �ٸ� �������� �� �������Ǹ� ���� false;
                        if (overlapItem != gridItemArray[posX + x, posY + y])
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
    /// InventoryItemSlot�� ���
    /// </summary>
    public void PrintInvenContents(InventoryGrid grid, ItemObject[,] slots)
    {
        string content = grid.gameObject.name + "\n";
        for (int i = 0; i < gridItemArray.GetLength(1); i++)
        {
            for (int j = 0; j < gridItemArray.GetLength(0); j++)
            {
                ItemObject item = gridItemArray[j, i];
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
    public Vector2Int? FindSpaceForObject(ItemObject itemToInsert)
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
                if (gridItemArray[posX + x, posY + y] != null)
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
    public Color32 PlaceCheckForHighlightColor(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        overlapItem = null;
        //�׸��� ������ ������  ��ġ ���� ����
        if (BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height) == false)
        {
            return HighlightColor.Red;
        }

        //��ġ�� �������� �ΰ� �̻� �ִٸ� overlapItem���� ����. ��ġ ���� ����
        if (OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem) == false)
        {
            //������ üũ���� ��ǥ�� �����۽����� �˻��ϴ� �������� �� ��ǥ �߻����� ���� �߻� �̸� �ذ��Ұ�
            overlapItem = null;
            return HighlightColor.Red;
        }

        //�������� �������� ������ ��ġ�� ������ ���
        if (overlapItem != null)
        {
            //�� ������ �������� ishide�� ��� ��ġ ���� ����
            if (overlapItem.ishide || placeItem.ishide)
            {
                overlapItem = null;
                return HighlightColor.Red;
            }

            //���� �Ҹ�ǰ�� ���
            //�������� ���� �ִ��� ������ġ�� �Ұ�. ��ü ����
            if(overlapItem.itemAmount == 64)
            {
                return HighlightColor.Red;
            }

            //��ġ�Ⱑ �����Ѱ�� ��ġ ���� ����
            if (placeItem.itemData.isItemConsumeable &&
                (placeItem.itemData.itemCode == overlapItem.itemData.itemCode))
            {
                overlapItem = null;
                return HighlightColor.Green;
            }

            return HighlightColor.Red;
        }
        return HighlightColor.Green;
    }
}
