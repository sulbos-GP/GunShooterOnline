using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Random = UnityEngine.Random;
using Google.Protobuf.Protocol;
using UnityEngine.Rendering;
using System.Collections.Generic;


public class InventoryGrid : MonoBehaviour
{
    /*
     * �κ��丮�� ���� UI���� ������ �׸���(�κ�UI�� ���� �÷��̾��� �׸����� ������ �ִٸ� other�� �׸����� �ı���)
     * 
     * �׸����� ������� ��ġ�� �����ϰ� ���ο� ����ִ� �����۵鿡 ���� ������ ������Ʈ�� �����Ͽ� �׸��嵥����.������ �����͸� �Ҵ��Ͽ� ��ġ
     */

    //�׸��� ����
    public GridData gridData; //�κ��丮���� �Ҵ�� �׸����� ������
    public InventoryUI ownInven; //�ش� �׸��带 �����ϴ� �κ��丮
    public ItemObject[,] ItemSlot; //��Ʈ�ѷ����� �������� ������ ����

    //gridWeight ������. ȣ��� gridWeight ������Ʈ �� �κ��丮�� ���Ե� ������Ʈ

    public float GridWeight
    {
        get => gridWeight;
        set
        {
            gridWeight = value;
            
            ownInven.UpdateInvenWeight(); //�κ��丮�� ���� ����
        }
    }
    [SerializeField]protected float gridWeight = 0; //�׸������ �������� ����

    //Ÿ���� ũ�� offset
    public const float WidthOfTile = 100;
    public const float HeightOfTile = 100;

    private RectTransform gridRect; //�ش� �׸����� transform
    private Vector2 mousePosOnGrid = new Vector2(); //�׸��� ���� ���콺 ��ġ
    private Vector2Int tileGridPos = new Vector2Int(); //���콺 �Ʒ��� Ÿ�� ��ġ��ǥ

    //��� ����
    public ItemObject[,] backUpSlot; //��� �迭
    public float backupWeight;

    //������ ����
    public GameObject itemPref; //�������� prefab
    //private bool createRandomItem;


    private void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        
    }

    /// <summary>
    /// �κ��丮���� �׸��带 �����Ҷ� ���
    /// </summary>
    public void GridDataSet()
    {
        if (gridData == null)
        {
            Debug.Log("�ش� �׸��� ������Ʈ�� �׸��� �����Ͱ� ����");
            return;
        }


        if (gridData.gridSize.x <= 0 || gridData.gridSize.y <= 0)
        {
            Debug.Log($"�׸��� ����� �������� ���� : sizeX : {gridData.gridSize.x}, sizeY : {gridData.gridSize.y}");
            return;
        }

        int width = gridData.gridSize.x;
        int height = gridData.gridSize.y;  

        ItemSlot = backUpSlot = new ItemObject[width, height]; 

        Vector2 rectSize = new Vector2(width * WidthOfTile, height * HeightOfTile);
        gridRect.sizeDelta = new UnityEngine.Vector2(rectSize.X, rectSize.Y);
        
        //createRandomItem = gridData.createRandomItem; Ŭ�󿡼� �����ʿ� ����

        GridRectPosSet();
    }

    /// <summary>
    /// �׸����� ��Ʈ�� ����
    /// </summary>
    private void GridRectPosSet()
    {
        //�׸��尡 ���� �� ���� �ʰ� �ణ�� �������� ��
        Vector2 offsetGridPosition = new Vector2(gridData.gridPos.X + InventoryUI.offsetX, gridData.gridPos.Y - InventoryUI.offsetY);
        transform.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(offsetGridPosition.X, offsetGridPosition.Y);
    
        //�׸����� ��ġ�� �Ϸ�Ǹ� �׸����� �����۽����� �����ϰ� ������ ������ ���� ��ġ
        GridItemSet();
    }

    private void GridItemSet()
    {

        if (gridData.itemList.Count != 0)
        {
            foreach (ItemData item in gridData.itemList)
            {
                CreateItemObj(item);
            }
        }

        UpdateBackUpSlot();
        //�������� ���� ����Ʈ�� gridItemArray�� �Ҵ��ϰ�
        //������ �������� ������ �ش� �������� �����͸� �־��ֱ�
        /*if(gridData.itemList.Count == 0)
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
            foreach(ItemData itemData in gridData.itemList)
            {
                CreateItemObj(itemData);
            }
        }*/

    }

    private void CreateItemObj(ItemData itemData)
    {
        ItemObject itemObj = Instantiate(itemPref, transform).GetComponent<ItemObject>();
        itemObj.ItemDataSet(itemData);
        PlaceItem(itemObj, itemData.itemPos.x, itemData.itemPos.y);
        itemObj.curItemGrid = this;

        itemObj.backUpItemPos = itemObj.itemData.itemPos; //���� ��ġ
        itemObj.backUpItemRotate = itemObj.itemData.itemRotate; //���� ȸ��
        itemObj.backUpItemGrid = itemObj.curItemGrid; //���� �׸���

        ownInven.instantItemList.Add(itemObj);
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
                ItemSlot[item.itemData.itemPos.x + x, item.itemData.itemPos.y + y] = null;
            }
        }
    }

    
    /// <summary>
    /// �������� ��ġ �������� ������ üũ �� �迭�� ������ ��ġ
    /// </summary>
    public bool PlaceItemCheck(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        //���� ���õ� �׸��尡 ���ٸ� ��ġ ����
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
        if (ownInven.CheckingInvenWeight(placeItem.itemData.item_weight, overlapItem) == false)
        {
            return false;
        }

        //�������� �������� �ִٸ�
        if (overlapItem != null)
        {
            //���� �Ҹ�ǰ�� ��� ��ġ�� ���� ���� ä�� true ����
            if (placeItem.itemData.isItemConsumeable && 
                (placeItem.itemData.itemCode == overlapItem.itemData.itemCode)&&
                overlapItem.itemData.itemAmount < 64)
            {
                return true;
            }

            overlapItem = null;
            return false;
        }
        
        //������ �迭�� �ش� ������ ��ġ
        PlaceItem(placeItem, posX, posY);
        PrintInvenContents(this, ItemSlot);
        
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
                ItemSlot[posX + x, posY + y] = item;
            }
        }

        GridWeight += item.itemData.item_weight;

        PlaceSprite(item, posX, posY, itemRect);
    }

    /// <summary>
    /// �������� ��Ʈ�� �ش���ġ�� �̵�
    /// </summary>
    public void PlaceSprite(ItemObject inventoryItem, int posX, int posY, RectTransform itemRect)
    {
        inventoryItem.itemData.itemPos = new Vector2Int(posX, posY);
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
        Vector2Int newPos = item.itemData.itemPos;

        // ���� ��ġ�� ������ ����Ʈ���� �������� �����մϴ�.
        RemoveItemFromItemList(item);

        // ���ο� ��ġ�� �������� �߰��մϴ�.
        AddItemToItemList(newPos, item);
    }

    public void AddItemToItemList(Vector2Int pos, ItemObject item)
    {
        // gridData�� itemList���� �������� �߰��մϴ�.
        ItemData itemData = item.itemData;
        if (itemData == null) return;

        // �������� �� ��ġ�� itemData�� �����մϴ�.
        itemData.itemPos = pos;

        // itemList���� �������� �߰��մϴ�.
        gridData.itemList.Add(itemData);
    }

    public void RemoveItemFromItemList(ItemObject item)
    {
        // gridData�� itemList���� �������� �����մϴ�.
        ItemData itemData = item.itemData;
        if (itemData == null) return;

        // itemList���� �ش� �������� �����մϴ�.
        gridData.itemList.Remove(itemData);
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

    public void UpdateBackUpSlot()
    {
        for (int i = 0; i < gridData.gridSize.x; i++)
        {
            for (int j = 0; j < gridData.gridSize.y; j++)
            {
                backUpSlot[i, j] = ItemSlot[i, j];
            }
        }
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
                ItemSlot[i, j] = backUpSlot[i, j];
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
                    //��ġ�� �������� 1�� ��쿡�� ���þ����۰� ��ü �� �̻��̶�� Place �Ұ�
                    if (overlapItem == null)
                    {
                        //�ش� ��ǥ�� �������� ���������������� ����
                        overlapItem = ItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        //�̹� ������ �������� �����Ǿ� �ִµ� �ٸ� �������� �� �������Ǹ� ���� false;
                        if (overlapItem != ItemSlot[posX + x, posY + y])
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
    /// �ش� ��ġ�� grid�� ������ üũ
    /// </summary>
    private bool InsideGridCheck(int posX, int posY)
    {
        if (posX < 0 || posY < 0)
        {
            return false;
        }
        if (posX >= gridData.gridSize.x || posY >= gridData.gridSize.y)
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
            if(overlapItem.itemData.itemAmount == 64)
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

    //���� ������ ������ ������ �����. Ŭ��� �ʿ����, ���� �����͸� �� ���븸 ��Ű���
    /*
    private int RestSlotCheck()
    {
        int restSlot = 0;
        for (int x = 0; x < gridData.gridSize.x; x++)
        {
            for (int y = 0; y < gridData.gridSize.y; y++)
            {
                if (ItemSlot[x, y] == null)
                {
                    restSlot++;
                }
            }
        }

        return restSlot;
    }
    */

    /*
    private void InstantRandomItem()
    {
        //�ӽ�(�����ͺ��̽� ������ ����)
        Debug.Log($"���� ���� {transform.name}");
        //InventoryController.invenInstance.InsertRandomItem(this);
        
        ItemObject randomItem = Instantiate(itemPref).GetComponent<ItemObject>();
        InventoryController.invenInstance.SetSelectedObjectToLastSibling(transform);
        int randomId = Random.Range(0, InventoryController.invenInstance.itemsList.Count);
        randomItem.ItemDataSet(InventoryController.invenInstance.itemsList[randomId]);

        FindPlaceableSlot(randomItem);
        randomItem.curItemGrid = this;

        randomItem.backUpItemPos = randomItem.curItemPos; //���� ��ġ
        randomItem.backUpItemRotate = randomItem.curItemRotate; //���� ȸ��
        randomItem.backUpItemGrid = randomItem.curItemGrid; //���� �׸���
    }*/

    /*
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
    }*/
    /*
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
   }*/
}
