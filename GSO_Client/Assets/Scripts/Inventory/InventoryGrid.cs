using UnityEngine;
using Vector2 = System.Numerics.Vector2;


public class InventoryGrid : MonoBehaviour
{
    //Ÿ���� ũ�� offset
    public const float WidthOfTile = 100;
    public const float HeightOfTile = 100;


    //�׸��� ����
    public GridData gridData; //�κ��丮���� �Ҵ�� �׸����� ������
    public InventoryUI ownInven; //�ش� �׸��带 �����ϴ� �κ��丮
    public ItemObject[,] ItemSlot { get; private set; } //��Ʈ�ѷ����� �������� ������ ����


    [SerializeField] protected float gridWeight = 0; //�׸������ �������� ����
    public float GridWeight
    {
        get => gridWeight;
        set
        {
            gridWeight = value;
            ownInven.UpdateInvenWeight(); //�κ��丮�� ���� ����
        }
    }

    private RectTransform gridRect; //�ش� �׸����� transform
    private Vector2 mousePosOnGrid = new Vector2(); //�׸��� ���� ���콺 ��ġ
    private Vector2Int tileGridPos = new Vector2Int(); //���콺 �Ʒ��� Ÿ�� ��ġ��ǥ

    //��� ����
    public ItemObject[,] BackUpSlot { get; private set; } //��� �迭
    public float BackUpWeight { get; private set; }


    //������ ����
    public GameObject itemPref; //�������� prefab


    private void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        
    }

    /// <summary>
    /// �κ��丮���� �׸��带 �����Ҷ� ���
    /// </summary>
    public void InitializeGrid()
    {
        if (gridRect == null) gridRect = GetComponent<RectTransform>();

        if (gridData == null)
        {
            Debug.LogError("Grid data is missing.");
            return;
        }

        if (gridData.gridSize.x <= 0 || gridData.gridSize.y <= 0)
        {
            Debug.LogError($"Invalid grid size: sizeX: {gridData.gridSize.x}, sizeY: {gridData.gridSize.y}");
            return;
        }

        int width = gridData.gridSize.x;
        int height = gridData.gridSize.y;

        ItemSlot = BackUpSlot = new ItemObject[width, height];

        Vector2 rectSize = new Vector2(width * WidthOfTile, height * HeightOfTile);
        gridRect.sizeDelta = new UnityEngine.Vector2(rectSize.X, rectSize.Y);

        SetGridPosition();
    }

    /// <summary>
    /// �׸����� ��Ʈ�� ����
    /// </summary>
    private void SetGridPosition()
    {
        //�׸��尡 ���� �� ���� �ʰ� �ణ�� �������� ��
        Vector2 offsetGridPosition = new Vector2(InventoryUI.offsetX, InventoryUI.offsetY);
        transform.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(offsetGridPosition.X, offsetGridPosition.Y);
    
        //�׸����� ��ġ�� �Ϸ�Ǹ� �׸����� �����۽����� �����ϰ� ������ ������ ���� ��ġ
        PlaceItemInGrid();
    }

    private void PlaceItemInGrid()
    {

        if (gridData.itemList.Count != 0)
        {
            foreach (ItemData item in gridData.itemList)
            {
                CreateItemObj(item);
            }
        }

        UpdateBackUpSlot();

    }

    private void CreateItemObj(ItemData itemData)
    {
        ItemObject itemObj = Instantiate(itemPref, transform).GetComponent<ItemObject>();
        InventoryController.invenInstance.instantItemList.Add(itemObj);
        itemObj.ItemDataSet(itemData);
        PlaceItem(itemObj, itemData.pos.x, itemData.pos.y);
        itemObj.curItemGrid = this;

        itemObj.backUpItemPos = itemObj.itemData.pos; //���� ��ġ
        itemObj.backUpItemRotate = itemObj.itemData.rotate; //���� ȸ��
        itemObj.backUpItemGrid = itemObj.curItemGrid; //���� �׸���

    }

    /// <summary>
    /// ���콺�� �� ��ġ�� �׸����� Ÿ�� ��ġ�� ��ȯ
    /// </summary>
    /// <param name="mousePosition">���콺 ��ġ</param>
    public Vector2Int MouseToGridPosition(Vector2 mousePosition)
    {
        if (gridRect == null) return Vector2Int.zero;
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
        if (x < 0 || y < 0 || x > gridData.gridSize.x-1 || y> gridData.gridSize.y-1)
        {
            return null;
        }
        return ItemSlot[x, y];
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

        ItemObject targetItem = ItemSlot[x, y];
        
        if (targetItem == null) { return null; }

        GridWeight -= targetItem.itemData.item_weight;

        CleanItemSlot(targetItem);
        PrintInvenContents(this,ItemSlot);

        return targetItem;
    }

    /// <summary>
    /// �ش� �������� ��ǥ���� ũ�⸸ŭ �κ��丮 ������ ������ ����
    /// </summary>
    /// <param name="item">���õ� ������</param>
    public void CleanItemSlot(ItemObject item)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                ItemSlot[item.itemData.pos.x + x, item.itemData.pos.y + y] = null;
            }
        }
    }

    
    /*/// <summary>
    /// �������� ��ġ �������� ������ üũ �� �迭�� ������ ��ġ
    /// </summary>
    public bool CanPlaceItem(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        //�������� �׸��� ������ ������ ���
        if (!BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height))
        {
            Debug.Log("boundary error");
            overlapItem = null;
            return false;
            
        }

        //���Կ� ���� ��ġ�� �Ұ����� ��� ���
        if (!ownInven.CanAffordWeight(placeItem.itemData.item_weight, overlapItem))
        {
            Debug.Log("weight error");
            return false;
        }

        //��ġ�� �������� �ִٸ� overlapItem������ �Ҵ�. ���⼭���� ������ ������ ������
        if (!OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem))
        {
            Debug.Log("overlap error");
            overlapItem = null;
            return false;
        }

        //�������� �������� �ִٸ�
        if (overlapItem != null)
        {

            //���� �Ҹ�ǰ�� ��� ��ġ�� ���� ���� ä�� true ����
            return overlapItem == null 
                || placeItem.itemData.isItemConsumeable 
                && placeItem.itemData.itemId == overlapItem.itemData.itemId 
                && overlapItem.itemData.amount < 64;
        }
        
        //������ �迭�� �ش� ������ ��ġ
        PlaceItem(placeItem, posX, posY);
        PrintInvenContents(this, ItemSlot);
        
        return true;
    }*/

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
                ItemSlot[posX + x, posY + y] = item;
            }
        }

        GridWeight += item.itemData.item_weight;

        UpdateItemPosition(item, posX, posY, itemRect);
    }

    /// <summary>
    /// �������� ��Ʈ�� �ش���ġ�� �̵�
    /// </summary>
    public void UpdateItemPosition(ItemObject inventoryItem, int posX, int posY, RectTransform itemRect)
    {
        inventoryItem.itemData.pos = new Vector2Int(posX, posY);
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
        itemRect.localPosition = new UnityEngine.Vector2(position.X,position.Y);
    }

    /// <summary>
    /// ������ �̵��� �׸��� ������ ����
    /// </summary>
    /// <param name="item"></param>
    public void UpdateItemInGridData(ItemObject item)
    {
        // ������ �����Ϳ��� ���� ��ġ�� �����ɴϴ�.
        Vector2Int oldPos = item.backUpItemPos;
        Vector2Int newPos = item.itemData.pos;

        // ���� ��ġ�� ������ ����Ʈ���� �������� �����մϴ�.
        RemoveItemFromItemList(item);

        // ���ο� ��ġ�� �������� �߰��մϴ�.
        AddItemToItemList(newPos, item);
    }

    public void AddItemToItemList(Vector2Int pos, ItemObject item)
    {
        if (item.itemData == null) return;

        item.itemData.pos = pos;
        item.curItemGrid = this;
        InventoryController.invenInstance.instantItemList.Add(item);

        if (!gridData.itemList.Contains(item.itemData))
        {
            gridData.itemList.Add(item.itemData);
        }
    }

    public void RemoveItemFromItemList(ItemObject item)
    {
        if (item.itemData == null) return;

        InventoryController.invenInstance.instantItemList.Remove(item);
        if (gridData.itemList.Contains(item.itemData))
        {
            gridData.itemList.Remove(item.itemData);
        }
       
    }


    /// <summary>
    /// �׸��� ���� ������ġ ���. �������� �ش� ��ġ�� ������Ű�� ����
    /// </summary>
    /// <param name="inventoryItem">�ش� ������</param>
    public Vector2 CalculatePositionOnGrid(ItemObject inventoryItem, int posX, int posY)
    {
        return new Vector2(
            posX * WidthOfTile + WidthOfTile * inventoryItem.Width / 2,
            -(posY * HeightOfTile + HeightOfTile * inventoryItem.Height / 2)
        );
    }

    public void UpdateBackUpSlot()
    {
        for (int i = 0; i < gridData.gridSize.x; i++)
        {
            for (int j = 0; j < gridData.gridSize.y; j++)
            {
                BackUpSlot[i, j] = ItemSlot[i, j];
            }
        }

        BackUpWeight = GridWeight;
    }

    /// <summary>
    /// ������ ������ ����� �������� �ǵ���.
    /// </summary>
    public void UndoItemSlot()
    {
        for (int i = 0; i < gridData.gridSize.x; i++)
        {
            for (int j = 0; j < gridData.gridSize.y; j++)
            {
                ItemSlot[i, j] = BackUpSlot[i, j];
            }
        }
    }

   
    /// <summary>
    /// ���������� ��ɿ� ���, �������� �������� �ڸ��� �������� �ִ��� üũ��
    /// </summary>
    public bool OverLapCheck(int posX, int posY, int width, int height, ref ItemObject overlapItem)
    {
        //������ ���� ��ȸ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //�ش� ������ ������ ������� ���� ��� 
                if (ItemSlot[posX + x, posY + y] != null)
                {
                    
                    if (ItemSlot[posX + x, posY + y].itemData.isItemConsumeable)
                    {
                        //�������� ������ �����Ҷ��� �ش� ��ǥ�� �������� ���������������� ����
                        overlapItem = ItemSlot[posX + x, posY + y];
                        return true;
                    }
                    
                }
            }
        }

        return false;
    }

    /// <summary>
    /// InventoryItemSlot�� ���
    /// </summary>
    public void PrintInvenContents(InventoryGrid grid, ItemObject[,] slots)
    {
        string content = grid.gameObject.name + "\n";
        for (int i = 0; i < ItemSlot.GetLength(1); i++)
        {
            for (int j = 0; j < ItemSlot.GetLength(0); j++)
            {
                ItemObject item = ItemSlot[j, i];
                if (item != null)
                {
                    content += $"| {item.itemData.item_name} |";
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
    /// �������� ũ�Ⱑ �׸��带 ������������ üũ
    /// </summary>
    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        return posX >= 0 && posY >= 0 && posX + width <= gridData.gridSize.x && posY + height <= gridData.gridSize.y;
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
                if (ItemSlot[posX + x, posY + y] != null)
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
    public Color32 PlaceCheckInGridHighLight(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        overlapItem = null;

        //�������� �׸��� ������ ������ ���
        if (!BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height))
        {
            Debug.Log("boundary error");
            InventoryController.invenInstance.itemPlaceableInGrid = false;
            return HighlightColor.Red;

        }

        //���Կ� ���� ��ġ�� �Ұ����� ��� ���
        if (!ownInven.CanAffordWeight(placeItem.itemData.item_weight, overlapItem))
        {
            Debug.Log("weight error");
            InventoryController.invenInstance.itemPlaceableInGrid = false;
            return HighlightColor.Red;
        }

        //��ġ�� �������� �ִٸ� overlapItem������ �Ҵ�. ���⼭���� ������ ������ ������
        if (OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem))
        {
            if (!placeItem.itemData.isItemConsumeable
                || placeItem.itemData.itemId != overlapItem.itemData.itemId
                || overlapItem.itemData.amount >= 64)
            {
                Debug.Log($"merge error");
                InventoryController.invenInstance.itemPlaceableInGrid = false;
                return HighlightColor.Red;
            }
        }

        InventoryController.invenInstance.itemPlaceableInGrid = true;
        return HighlightColor.Green;
    }

   /// <summary>
   /// �������� ũ�⸸ŭ �� ��Ҹ� ã��
   /// ?�� �� ������ ������ ���ϰ��� �ΰ��� ����ϱ� ����
   /// </summary>
   public Vector2Int? FindSpaceForObject(ItemObject itemToInsert)
   {
       int width = gridData.gridSize.x - (itemToInsert.Width - 1);
       int height = gridData.gridSize.y - (itemToInsert.Height - 1);

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
}
