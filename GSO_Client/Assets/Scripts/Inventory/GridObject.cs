using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;


public class GridObject : MonoBehaviour
{
    //Ÿ���� ũ�� offset
    public const float WidthOfTile = 100;
    public const float HeightOfTile = 100;

    public int objectId;

    public Vector2Int gridSize; //�÷��̾� : ������ ũ��, �ڽ� 5*5
    public ItemObject[,] ItemSlot { get; private set; } //��Ʈ�ѷ����� �������� ������ ����

    [SerializeField] protected float gridWeight = 0; //�׸������ �������� ����
    public float GridWeight
    {
        get => gridWeight;
        set
        {
            gridWeight = value;
        }
    }
    public float limitWeight;

    private RectTransform gridRect; //�ش� �׸����� transform
    private Vector2 mousePosOnGrid = new Vector2(); //�׸��� ���� ���콺 ��ġ
    private Vector2Int tileGridPos = new Vector2Int(); //���콺 �Ʒ��� Ÿ�� ��ġ��ǥ

    //��� ����
    public ItemObject[,] BackUpSlot { get; private set; } //��� �迭
    public float BackUpWeight { get; private set; }

    
    private void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        
    }

    /// <summary>
    /// �κ��丮���� �׸��带 �����Ҷ� ���
    /// </summary>
    public void InitializeGrid(Vector2Int _gridSize)
    {
        if (gridRect == null) gridRect = GetComponent<RectTransform>();

        if (_gridSize.x <= 0 || _gridSize.y <= 0)
        {
            Debug.LogError($"Invalid grid size: sizeX: {_gridSize.x}, sizeY: {_gridSize.y}");
            return;
        }
        int width = _gridSize.x;
        int height = _gridSize.y;
        gridSize = _gridSize;
        limitWeight = 20f;

        ItemSlot = BackUpSlot = new ItemObject[width, height];

        Vector2 rectSize = new Vector2(width * WidthOfTile, height * HeightOfTile);
        gridRect.sizeDelta = new UnityEngine.Vector2(rectSize.X, rectSize.Y);

        InventoryController.invenInstance.playerInvenUI.WeightTextSet(GridWeight,limitWeight);
        SetGridPosition();
    }

    /// <summary>
    /// �׸����� ��Ʈ�� ����
    /// </summary>
    private void SetGridPosition()
    {
        //�׸��尡 ���� �� ���� �ʰ� �ణ�� �������� �� -> �����Ұ� : �߾ӿ� ������
        Vector2 offsetGridPosition = new Vector2(InventoryUI.offsetX, InventoryUI.offsetY);
        transform.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(offsetGridPosition.X, offsetGridPosition.Y);
    }

    /// <summary>
    /// ��Ŷ���� ������ ������;
    /// </summary>
    /// <param name="itemData"></param>
    public void PlaceItemInGrid(List<ItemData> itemData)
    {
        if (itemData.Count != 0)
        {
            foreach (ItemData item in itemData)
            {
                CreateItemObj(item);
            }
        }

        UpdateBackUpSlot();
    }

    private void CreateItemObj(ItemData itemData)
    {
        //�ش� �׸����� �ڽ����� �����Ͽ� ����
        ItemObject itemObj = Managers.Resource.Instantiate("UI/ItemUI", transform).GetComponent<ItemObject>();
        //�κ���Ʈ�ѷ����� ������ �����۸���Ʈ�� ���
        InventoryController.invenInstance.instantItemList.Add(itemObj);
        //�ش� �����ۿ� �ο��� �����ͷ� ������ ����
        itemObj.ItemDataSet(itemData);
        //�������� �ش� �׸��忡 ��ġ�ϰ� ������ ��ü�� ��ġ���� �°� ����
        PlaceItem(itemObj, itemData.pos.x, itemData.pos.y);
        itemObj.curItemGrid = this;
        
        //������ ��� ������ ���� ���� ���
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
        if (x < 0 || y < 0 || x > gridSize.x-1 || y> gridSize.y-1)
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

        PrintInvenContents(this, ItemSlot);
        GridWeight += item.itemData.item_weight;
        InventoryController.invenInstance.instantItemList.Add(item);
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
    }

    public void RemoveItemFromItemList(ItemObject item)
    {
        if (item.itemData == null) return;

        InventoryController.invenInstance.instantItemList.Remove(item);
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
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
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
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
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
    public void PrintInvenContents(GridObject grid, ItemObject[,] slots)
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

        //��ġ�� �������� �ִٸ� overlapItem������ �Ҵ�. ���⼭���� ������ ������ ������
        if (OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem))
        {
            
            if (!placeItem.itemData.isItemConsumeable
                || placeItem.itemData.itemId != overlapItem.itemData.itemId
                || overlapItem.itemData.amount >= ItemObject.maxItemMergeAmount)
            {
                //���� ��ġ�� �������� ������ ������ ������ �������� ����
                Debug.Log($"merge error");
                InventoryController.invenInstance.itemPlaceableInGrid = false;
                return HighlightColor.Red;
            }
        }

        //���Կ� ���� ��ġ�� �Ұ����� ��� ���
        if (!InventoryWeightCheck(placeItem.itemData.item_weight, overlapItem))
        {
            Debug.Log("weight error");
            InventoryController.invenInstance.itemPlaceableInGrid = false;
            return HighlightColor.Red;
        }

        InventoryController.invenInstance.itemPlaceableInGrid = true;
        return HighlightColor.Green;
    }


    /// <summary>
    /// �������� ũ�Ⱑ �׸��带 ������������ üũ
    /// </summary>
    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        return posX >= 0 && posY >= 0 && posX + width <= gridSize.x && posY + height <= gridSize.y;
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

    public bool InventoryWeightCheck(float addWeight, ItemObject overlap)
    {
        float curWeight = GridWeight;

        if (overlap != null)
        {
            curWeight -= overlap.itemData.item_weight;
        }

        if (curWeight + addWeight > limitWeight)
        {
            Debug.Log("�κ��丮�� ���԰� �Ѱ踦 ����");
            return false;
        }

        return true;
    }

}
