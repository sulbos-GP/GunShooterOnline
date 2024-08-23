using UnityEngine;
using Vector2 = System.Numerics.Vector2;


public class InventoryGrid : MonoBehaviour
{
    //타일의 크기 offset
    public const float WidthOfTile = 100;
    public const float HeightOfTile = 100;


    //그리드 설정
    public GridData gridData; //인벤토리에서 할당된 그리드의 데이터
    public InventoryUI ownInven; //해당 그리드를 포함하는 인벤토리
    public ItemObject[,] ItemSlot { get; private set; } //컨트롤러에서 아이템을 저장할 슬롯


    [SerializeField] protected float gridWeight = 0; //그리드안의 아이템의 무게
    public float GridWeight
    {
        get => gridWeight;
        set
        {
            gridWeight = value;
            ownInven.UpdateInvenWeight(); //인벤토리의 무게 변경
        }
    }

    private RectTransform gridRect; //해당 그리드의 transform
    private Vector2 mousePosOnGrid = new Vector2(); //그리드 위의 마우스 위치
    private Vector2Int tileGridPos = new Vector2Int(); //마우스 아래의 타일 위치좌표

    //백업 관련
    public ItemObject[,] BackUpSlot { get; private set; } //백업 배열
    public float BackUpWeight { get; private set; }


    //아이템 생성
    public GameObject itemPref; //아이템의 prefab


    private void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        
    }

    /// <summary>
    /// 인벤토리에서 그리드를 생성할때 사용
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
    /// 그리드의 렉트를 설정
    /// </summary>
    private void SetGridPosition()
    {
        //그리드가 벽에 딱 붙지 않게 약간의 오프셋을 둠
        Vector2 offsetGridPosition = new Vector2(InventoryUI.offsetX, InventoryUI.offsetY);
        transform.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(offsetGridPosition.X, offsetGridPosition.Y);
    
        //그리드의 배치가 완료되면 그리드의 아이템슬롯을 설정하고 아이템 프리팹 또한 배치
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

        itemObj.backUpItemPos = itemObj.itemData.pos; //현재 위치
        itemObj.backUpItemRotate = itemObj.itemData.rotate; //현재 회전
        itemObj.backUpItemGrid = itemObj.curItemGrid; //현재 그리드

    }

    /// <summary>
    /// 마우스의 현 위치를 그리드의 타일 위치로 변환
    /// </summary>
    /// <param name="mousePosition">마우스 위치</param>
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
    /// 해당 좌표에 있는 아이템 반환
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
    /// 해당 좌표의 타일에 아이템을 리턴하며 배열상에서도 제거
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
    /// 해당 아이템의 좌표에서 크기만큼 인벤토리 슬롯의 내용을 제거
    /// </summary>
    /// <param name="item">선택된 아이템</param>
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
    /// 아이템이 배치 가능한지 조건을 체크 후 배열상 아이템 배치
    /// </summary>
    public bool CanPlaceItem(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        //아이템이 그리드 밖으로 나갈시 취소
        if (!BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height))
        {
            Debug.Log("boundary error");
            overlapItem = null;
            return false;
            
        }

        //무게에 의해 배치가 불가능할 경우 취소
        if (!ownInven.CanAffordWeight(placeItem.itemData.item_weight, overlapItem))
        {
            Debug.Log("weight error");
            return false;
        }

        //겹치는 아이템이 있다면 overlapItem변수에 할당. 여기서부터 오버랩 아이템 지정됨
        if (!OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem))
        {
            Debug.Log("overlap error");
            overlapItem = null;
            return false;
        }

        //오버랩된 아이템이 있다면
        if (overlapItem != null)
        {

            //같은 소모품의 경우 배치를 하지 않은 채로 true 리턴
            return overlapItem == null 
                || placeItem.itemData.isItemConsumeable 
                && placeItem.itemData.itemId == overlapItem.itemData.itemId 
                && overlapItem.itemData.amount < 64;
        }
        
        //아이템 배열에 해당 아이템 배치
        PlaceItem(placeItem, posX, posY);
        PrintInvenContents(this, ItemSlot);
        
        return true;
    }*/

    /// <summary>
    /// 배열에 아이템 저장 및 해당 아이템 UI 객체 이동
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
    /// 아이템의 렉트를 해당위치로 이동
    /// </summary>
    public void UpdateItemPosition(ItemObject inventoryItem, int posX, int posY, RectTransform itemRect)
    {
        inventoryItem.itemData.pos = new Vector2Int(posX, posY);
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
        itemRect.localPosition = new UnityEngine.Vector2(position.X,position.Y);
    }

    /// <summary>
    /// 아이템 이동시 그리드 데이터 수정
    /// </summary>
    /// <param name="item"></param>
    public void UpdateItemInGridData(ItemObject item)
    {
        // 아이템 데이터에서 현재 위치를 가져옵니다.
        Vector2Int oldPos = item.backUpItemPos;
        Vector2Int newPos = item.itemData.pos;

        // 이전 위치의 아이템 리스트에서 아이템을 제거합니다.
        RemoveItemFromItemList(item);

        // 새로운 위치에 아이템을 추가합니다.
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
    /// 그리드 내의 월드위치 계산. 아이템을 해당 위치에 고정시키기 위함
    /// </summary>
    /// <param name="inventoryItem">해당 아이템</param>
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
    /// 아이템 슬롯을 저장된 슬롯으로 되돌림.
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
    /// 머지아이템 기능에 사용, 놓으려는 아이템의 자리에 아이템이 있는지 체크함
    /// </summary>
    public bool OverLapCheck(int posX, int posY, int width, int height, ref ItemObject overlapItem)
    {
        //아이템 슬롯 순회
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //해당 아이템 슬롯이 비어있지 않을 경우 
                if (ItemSlot[posX + x, posY + y] != null)
                {
                    
                    if (ItemSlot[posX + x, posY + y].itemData.isItemConsumeable)
                    {
                        //아이템이 오버랩 가능할때만 해당 좌표의 아이템을 오버렙아이템으로 지정
                        overlapItem = ItemSlot[posX + x, posY + y];
                        return true;
                    }
                    
                }
            }
        }

        return false;
    }

    /// <summary>
    /// InventoryItemSlot을 출력
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
    /// 아이템의 크기가 그리드를 빠져나가는지 체크
    /// </summary>
    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        return posX >= 0 && posY >= 0 && posX + width <= gridData.gridSize.x && posY + height <= gridData.gridSize.y;
    }


    /// <summary>
    /// 해당 공간안에 아이템의 크기만큼의 공간이 되는지 확인
    /// </summary>
    public bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        //아이템 슬롯 순회
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //해당 아이템 슬롯이 비어있지 않을 경우 
                if (ItemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 컨트롤러의 업데이트마다 실행
    /// 해당 그리드 셀에 아이템을 배치가 가능한지 여부에 따른 색을 반환
    /// </summary>
    public Color32 PlaceCheckInGridHighLight(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        overlapItem = null;

        //아이템이 그리드 밖으로 나갈시 취소
        if (!BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height))
        {
            Debug.Log("boundary error");
            InventoryController.invenInstance.itemPlaceableInGrid = false;
            return HighlightColor.Red;

        }

        //무게에 의해 배치가 불가능할 경우 취소
        if (!ownInven.CanAffordWeight(placeItem.itemData.item_weight, overlapItem))
        {
            Debug.Log("weight error");
            InventoryController.invenInstance.itemPlaceableInGrid = false;
            return HighlightColor.Red;
        }

        //겹치는 아이템이 있다면 overlapItem변수에 할당. 여기서부터 오버랩 아이템 지정됨
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
   /// 아이템의 크기만큼 들어갈 장소를 찾음
   /// ?를 쓴 이유는 마지막 리턴값에 널값을 허용하기 위함
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
