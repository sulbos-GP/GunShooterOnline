using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;


public class GridObject : MonoBehaviour
{
    //타일의 크기 offset
    public const float WidthOfTile = 100;
    public const float HeightOfTile = 100;

    public int objectId;

    public Vector2Int gridSize; //플레이어 : 가방의 크기, 박스 5*5
    public ItemObject[,] ItemSlot { get; private set; } //컨트롤러에서 아이템을 저장할 슬롯

    [SerializeField] protected double gridWeight = 0; //그리드안의 아이템의 무게
    public double GridWeight
    {
        get => gridWeight;
        set
        {
            gridWeight = value;
        }
    }
    public double limitWeight;

    private RectTransform gridRect; //해당 그리드의 transform
    private Vector2 mousePosOnGrid = new Vector2(); //그리드 위의 마우스 위치
    private Vector2Int tileGridPos = new Vector2Int(); //마우스 아래의 타일 위치좌표

    //백업 관련
    public ItemObject[,] BackUpSlot { get; private set; } //백업 배열
    public double BackUpWeight { get; private set; }

    
    private void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        
    }

    /// <summary>
    /// 인벤토리에서 그리드를 생성할때 사용
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
    /// 그리드의 렉트를 설정
    /// </summary>
    private void SetGridPosition()
    {
        //그리드가 벽에 딱 붙지 않게 약간의 오프셋을 둠 -> 수정할것 : 중앙에 오도록
        Vector2 offsetGridPosition = new Vector2(InventoryUI.offsetX, InventoryUI.offsetY);
        transform.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(offsetGridPosition.X, offsetGridPosition.Y);
    }

    /// <summary>
    /// 그리드를 보여주기만 할경우에 부모객체에서 사용.
    /// 주의. 그리드를 옮겨야하는 경우에 이걸로 그리드의 크기를 바꾸면 그리드간의 크기차이가 발생
    /// </summary>
    public void SetGridScale(RectTransform parentRect)
    {
        RectTransform childRect = GetComponent<RectTransform>();

        float widthScale = parentRect.rect.width / childRect.rect.width;
        float heightScale = parentRect.rect.height / childRect.rect.height;
        childRect.localScale = new Vector3(widthScale, heightScale, 1);
    }

    /// <summary>
    /// 패킷에서 아템을 넣을것;
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
    /// 받아온 데이터를 토대로 아이템 생성후 해당 그리드에 아이템 배치
    /// </summary>
    /// <param name="itemData"></param>
    public ItemObject CreateItemObjAndPlace(ItemData itemData)
    {
        ItemObject itemObj = ItemObject.CreateNewItem(itemData, transform);

        //아이템을 해당 그리드에 배치하고 아이템 객체의 위치또한 맞게 변경
        PlaceItem(itemObj, itemData.pos.x, itemData.pos.y);

        //아이템 백업 변수에 현재 정보 백업
        ItemObject.BackUpItem(itemObj);
        return itemObj;
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
    /// 컨트롤러의 업데이트마다 실행
    /// 해당 그리드의 위치에 아이템을 배치가 가능한지 여부와 그에 따른 색을 반환
    /// </summary>
    public Color32 PlaceCheckInGridHighLight(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        overlapItem = null;
        InventoryController.invenInstance.itemPlaceableInGrid = false;
        //아이템이 그리드 밖으로 나갈시 취소
        if (!BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height))
        {
            Debug.Log("boundary error");
            return HighlightColor.Red;

        }

        //겹치는 아이템이 있다면 overlapItem변수에 할당. 여기서부터 오버랩 아이템 지정됨
        if (OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem))
        {
            if (!placeItem.itemData.isItemConsumeable
                || placeItem.itemData.itemId != overlapItem.itemData.itemId
                || overlapItem.ItemAmount >= ItemObject.maxItemMergeAmount)
            {
                //현재 겹치는 아이템이 있지만 머지의 기준을 충족하지 못함
                Debug.Log($"merge error");
                return HighlightColor.Red;
            }
        }

        if(objectId == 0) //플레이어의 인벤토리에서만 무게 적용
        {
            //무게에 의해 배치가 불가능할 경우 취소
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
                    ? placeItem.ItemAmount : ItemObject.maxItemMergeAmount - overlapItem.ItemAmount; //머지가 가능한 개수

            int ableAmount = (int)Math.Round((playerGrid.limitWeight - curWeight) / placeItem.itemData.item_weight); //남은 무게에서 채울수 있는 개수

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
            //오버랩 아이템이 존재한다면 이것은 머지가 가능할 경우 + divide모드가 켜져있다면 나누기 모드(일단은 배치가 가능하게 하여 서버에 패킷을 전송해야함)
            itemWeight = placeItem.itemData.item_weight; //최소한의 개수인 1개의 무게가 들어갈수 있다면 성공판정
        }
        else
        {
            //평상시
            itemWeight = placeItem.itemWeight;
        }


        if (placeItem.backUpParentId == 0) //체크
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
    /// 해당 좌표에 있는 아이템 반환
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
        CleanItemSlot(targetItem); //꼭 든 순간 아이템을 슬롯에서 제거해야함!!!

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

        item.parentObjId = objectId;
        UpdateBackUpSlot();
        UpdateItemPosition(item, posX, posY);
    }

    /// <summary>
    /// 아이템의 렉트를 해당위치로 이동
    /// </summary>
    public void UpdateItemPosition(ItemObject inventoryItem, int posX, int posY)
    {
        RectTransform itemRect = inventoryItem.GetComponent<RectTransform>();
        inventoryItem.itemData.pos = new Vector2Int(posX, posY);
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
        itemRect.localPosition = new UnityEngine.Vector2(position.X,position.Y);
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
    /// 아이템 슬롯을 저장된 슬롯으로 되돌림.
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
        Debug.Log($"해당 부모의 남은 아이템 갯수 : {transform.childCount}");
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
                    overlapItem = ItemSlot[posX + x, posY + y];
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// InventoryItemSlot을 출력
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
    /// 아이템의 크기가 그리드를 빠져나가는지 체크
    /// </summary>
    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        return posX >= 0 && posY >= 0 && posX + width <= gridSize.x && posY + height <= gridSize.y;
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
   /// 아이템의 크기만큼 들어갈 장소를 찾음
   /// ?를 쓴 이유는 마지막 리턴값에 널값을 허용하기 위함
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
    /// 가방의 변화로 그리드의 크기를 변경 및 스프라이트 재배치
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
    /// 바뀔 크기를 넘어가는 아이템 존재하는지
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
