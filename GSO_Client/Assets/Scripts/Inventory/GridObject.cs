using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
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

    [SerializeField] protected double gridWeight = 0; //�׸������ �������� ����
    public double GridWeight
    {
        get => gridWeight;
        set
        {
            gridWeight = value;
        }
    }
    public double limitWeight;

    private RectTransform gridRect; //�ش� �׸����� transform
    private Vector2 mousePosOnGrid = new Vector2(); //�׸��� ���� ���콺 ��ġ
    private Vector2Int tileGridPos = new Vector2Int(); //���콺 �Ʒ��� Ÿ�� ��ġ��ǥ

    //��� ����
    public ItemObject[,] BackUpSlot { get; private set; } //��� �迭
    public double BackUpWeight { get; private set; }

    
    private void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        
    }

    /// <summary>
    /// �κ��丮���� �׸��带 �����Ҷ� ���
    /// </summary>
    public void InitializeGrid(Vector2Int _gridSize, double gridWeigth = 30f)
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
        limitWeight = gridWeigth;

        ItemSlot = new ItemObject[width, height];
        BackUpSlot = new ItemObject[width, height];
        Vector2 rectSize = new Vector2(width * WidthOfTile, height * HeightOfTile);
        gridRect.sizeDelta = new UnityEngine.Vector2(rectSize.X, rectSize.Y);

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
    /// �׸��带 �����ֱ⸸ �Ұ�쿡 �θ�ü���� ���.
    /// ����. �׸��带 �Űܾ��ϴ� ��쿡 �̰ɷ� �׸����� ũ�⸦ �ٲٸ� �׸��尣�� ũ�����̰� �߻�
    /// </summary>
    public void SetGridScale(RectTransform parentRect)
    {
        RectTransform childRect = GetComponent<RectTransform>();

        float widthScale = parentRect.rect.width / childRect.rect.width;
        float heightScale = parentRect.rect.height / childRect.rect.height;
        childRect.localScale = new Vector3(widthScale, heightScale, 1);
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
                CreateItemObjAndPlace(item);
            }
        }

        UpdateBackUpSlot();
    }

    /// <summary>
    /// �޾ƿ� �����͸� ���� ������ ������ �ش� �׸��忡 ������ ��ġ
    /// </summary>
    /// <param name="itemData"></param>
    public ItemObject CreateItemObjAndPlace(ItemData itemData)
    {
        ItemObject itemObj = ItemObject.CreateNewItem(itemData, transform);

        //�������� �ش� �׸��忡 ��ġ�ϰ� ������ ��ü�� ��ġ���� �°� ����
        PlaceItem(itemObj, itemData.pos.x, itemData.pos.y);

        //������ ��� ������ ���� ���� ���
        ItemObject.BackUpItem(itemObj);
        return itemObj;
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
    /// ��Ʈ�ѷ��� ������Ʈ���� ����
    /// �ش� �׸����� ��ġ�� �������� ��ġ�� �������� ���ο� �׿� ���� ���� ��ȯ
    /// </summary>
    public Color32 PlaceCheckInGridHighLight(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        overlapItem = null;
        InventoryController.invenInstance.itemPlaceableInGrid = false;
        //�������� �׸��� ������ ������ ���
        if (!BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height))
        {
            Debug.Log("boundary error");
            return HighlightColor.Red;

        }

        //��ġ�� �������� �ִٸ� overlapItem������ �Ҵ�. ���⼭���� ������ ������ ������
        if (OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem))
        {
            if (!placeItem.itemData.isItemConsumeable
                || placeItem.itemData.itemId != overlapItem.itemData.itemId
                || overlapItem.ItemAmount >= ItemObject.maxItemMergeAmount)
            {
                //���� ��ġ�� �������� ������ ������ ������ �������� ����
                Debug.Log($"merge error");
                return HighlightColor.Red;
            }
        }

        if(objectId == 0) //�÷��̾��� �κ��丮������ ���� ����
        {
            //���Կ� ���� ��ġ�� �Ұ����� ��� ���
            if (!CheckGridWeight(placeItem, overlapItem))
            {
                Debug.Log("weight error");
                return HighlightColor.Red;
            }
        }
        

        InventoryController.invenInstance.itemPlaceableInGrid = true;
        return HighlightColor.Green;
    }

    private bool CheckGridWeight(ItemObject placeItem, ItemObject overlapItem)
    {
        GridObject playerGrid = InventoryController.invenInstance.playerInvenUI.instantGrid;
        double curWeight = playerGrid.GridWeight;
        double itemWeight = 0;
        if (overlapItem != null) 
        {
            int giveAmount = placeItem.ItemAmount + overlapItem.ItemAmount <= ItemObject.maxItemMergeAmount
                    ? placeItem.ItemAmount : ItemObject.maxItemMergeAmount - overlapItem.ItemAmount; //������ ������ ����

            int ableAmount = (int)Math.Round((playerGrid.limitWeight - curWeight) / placeItem.itemData.item_weight); //���� ���Կ��� ä��� �ִ� ����

            if (giveAmount > ableAmount) { 
                itemWeight = placeItem.itemData.item_weight * ableAmount;
            }
            else
            {
                itemWeight = placeItem.itemData.item_weight * giveAmount;
            }

            itemWeight = giveAmount > ableAmount ? placeItem.itemData.item_weight * ableAmount : itemWeight = placeItem.itemData.item_weight * giveAmount;
        }
        else if ( InventoryController.invenInstance.isDivideMode)
        {
            //������ �������� �����Ѵٸ� �̰��� ������ ������ ��� + divide��尡 �����ִٸ� ������ ���(�ϴ��� ��ġ�� �����ϰ� �Ͽ� ������ ��Ŷ�� �����ؾ���)
            itemWeight = placeItem.itemData.item_weight; //�ּ����� ������ 1���� ���԰� ���� �ִٸ� ��������
        }
        else
        {
            //����
            itemWeight = placeItem.itemWeight;
        }


        if (placeItem.backUpParentId == 0) //üũ
        {
            itemWeight = 0;
        }
        
        double result = Math.Round(curWeight + itemWeight,2);

        if(InventoryController.invenInstance.SelectedGrid.objectId == 0)
        {
            InventoryController.invenInstance.playerInvenUI.weightText.text = $"WEIGHT \n{result} / {playerGrid.limitWeight}";

            if (result > playerGrid.limitWeight)
            {
                InventoryController.invenInstance.playerInvenUI.weightText.color = Color.red;
            }
            else
            {
                InventoryController.invenInstance.playerInvenUI.weightText.color = Color.white;
            }
        }
        
        if (result > playerGrid.limitWeight)
        {
            return false;
        }

        return true;
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
        CleanItemSlot(targetItem); //�� �� ���� �������� ���Կ��� �����ؾ���!!!

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

        item.parentObjId = objectId;
        UpdateBackUpSlot();
        UpdateItemPosition(item, posX, posY);
    }

    /// <summary>
    /// �������� ��Ʈ�� �ش���ġ�� �̵�
    /// </summary>
    public void UpdateItemPosition(ItemObject inventoryItem, int posX, int posY)
    {
        RectTransform itemRect = inventoryItem.GetComponent<RectTransform>();
        inventoryItem.itemData.pos = new Vector2Int(posX, posY);
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
        itemRect.localPosition = new UnityEngine.Vector2(position.X,position.Y);
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

        GridWeight = BackUpWeight;
    }

    public void UpdateGridWeight()
    {
        Debug.Log($"�ش� �θ��� ���� ������ ���� : {transform.childCount}");
        double weightIndex = 0;

        foreach(ItemObject item in InventoryController.instantItemDic.Values)
        {
            if(item.backUpParentId == 0)
            {
                double roundedWeight = Math.Round(item.itemWeight, 2);
                weightIndex += roundedWeight;
            }
        }
        
        GridWeight = Math.Round(weightIndex, 2);
        Debug.Log($"Grid weight updated: {GridWeight}");
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
                    overlapItem = ItemSlot[posX + x, posY + y];
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// InventoryItemSlot�� ���
    /// </summary>
    public void PrintInvenContents()
    {
        string content = gameObject.name + " slot\n";
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

        content += "\n\n";

        content += gameObject.name + " backUp slot\n";
        for (int i = 0; i < BackUpSlot.GetLength(1); i++)
        {
            for (int j = 0; j < BackUpSlot.GetLength(0); j++)
            {
                ItemObject item = BackUpSlot[j, i];
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


    /// <summary>
    /// ������ ��ȭ�� �׸����� ũ�⸦ ���� �� ��������Ʈ ���ġ
    /// </summary>
    public void UpdateGridObject(Vector2Int size, double Weight)
    {
        if (gridRect == null) gridRect = GetComponent<RectTransform>();

        if (size.x <= 0 || size.y <= 0)
        {
            Debug.LogError($"Invalid grid size: sizeX: {size.x}, sizeY: {size.y}");
            return;
        }
        int width = size.x;
        int height = size.y;
        gridSize = size;
        limitWeight = Weight;

        ItemSlot = new ItemObject[width, height];
        BackUpSlot = new ItemObject[width, height];
        Vector2 rectSize = new Vector2(width * WidthOfTile, height * HeightOfTile);
        gridRect.sizeDelta = new UnityEngine.Vector2(rectSize.X, rectSize.Y);

        InventoryController.UpdatePlayerWeight();

        foreach (ItemObject item in InventoryController.instantItemDic.Values)
        {
            if(item.parentObjId != 0)
            {
                continue;
            }

            PlaceItem(item, item.itemData.pos.x, item.itemData.pos.y);
        }
    }

    /// <summary>
    /// �ٲ� ũ�⸦ �Ѿ�� ������ �����ϴ���
    /// </summary>
    public bool CheckAvailableToChange(Vector2Int changeSize)
    {
        ItemObject[,] testSlot = new ItemObject[changeSize.x, changeSize.y];

        foreach (ItemObject item in InventoryController.instantItemDic.Values)
        {
            if (item.backUpParentId != 0)
            {
                continue;
            }

            if(!(item.backUpItemPos.x >= 0 && item.backUpItemPos.y >= 0 && item.backUpItemPos.x + item.Width <= changeSize.x && item.backUpItemPos.y + item.Height <= changeSize.y))
            {
                return false;
            }
        }

        return true;
    }
}
