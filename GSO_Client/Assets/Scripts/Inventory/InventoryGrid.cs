using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Random = UnityEngine.Random;
using Google.Protobuf.Protocol;


public class InventoryGrid : MonoBehaviour
{
    /*
     * 그리드에 대한 조작을 관리함
     * 
     * 
     */

    //그리드 설정
    public GridData gridData; //인벤토리에서 할당된 그리드의 데이터

    public int gridId = 0; // 임의 할당. 서버에서 받은 그리드 데이터의 id

    public ItemObject[,] gridItemArray; //인벤토리 내용이 기록될 배열
    public Inventory invenScr; //해당 그리드를 포함하는 인벤토리의 스크립트

    public float gridWeight = 0; //그리드안의 아이템의 무게

    //gridWeight 옵저버. 호출시 gridWeight 업데이트 및 인벤토리의 무게도 업데이트
    public float GridWeight
    {
        get => gridWeight;
        set 
        { 
            gridWeight = value;
            invenScr.UpdateInvenWeight();
        }
    }

    //타일의 크기 offset
    public const float WidthOfTile = 100;
    public const float HeightOfTile = 100;

    private RectTransform gridRect; //해당 그리드의 transform
    private Vector2Int gridSize; //그리드의 폭,높이별 타일 개수 입력
    private Vector2 mousePosOnGrid = new Vector2(); //그리드 위의 마우스 위치
    private Vector2Int tileGridPos = new Vector2Int(); //마우스 아래의 타일 위치좌표

    //백업 관련
    public ItemObject[,] gridBackupArray; //백업 배열
    public float backupWeight;

    //아이템 생성
    public GameObject itemPref; //아이템의 prefab
    private bool createRandomItem;

    private void Start()
    {
        gridRect = GetComponent<RectTransform>();
        if(gridData == null)
        {
            Debug.Log("해당 그리드 오브젝트에 그리드 데이터가 없음");
            return;
        }
        gridId = gridData.gridId;
        gridSize = gridData.gridSize;
        if (gridSize.x <= 0 || gridSize.y <= 0)
        {
            //그리드 사이즈를 설정하지 않으면 기본값으로 5,5 할당
            gridSize = new Vector2Int(5, 5);
        }
        
        Init(gridSize.x, gridSize.y);
    }

    /// <summary>
    /// 해당 그리드의 인벤토리 아이템 배열 생성 및 사이즈 할당
    /// </summary>
    /// <param name="width">그리드의 폭</param>
    /// <param name="height">그리드의 높이</param>
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
        //그리드가 벽에 딱 붙지 않게 약간의 오프셋을 둠
        Vector2 offsetGridPosition = new Vector2(gridData.gridPos.X + Inventory.offsetX, gridData.gridPos.Y - Inventory.offsetY);
        transform.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(offsetGridPosition.X, offsetGridPosition.Y);
    
        //그리드의 배치가 완료되면 그리드의 아이템슬롯을 설정하고 아이템 프리팹 또한 배치
        GridItemSet();
    }

    private void GridItemSet()
    {
        //서버에서 받은 리스트를 gridItemArray에 할당하고
        //아이템 프리팹을 생성후 해당 아이템의 데이터를 넣어주기
        if(gridData.itemList.Count == 0)
        {
            //이 그리드에 아이템 데이터가 없을경우 조건에 따라 랜덤한 아이템 배치
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
                Debug.Log("비어있는 그리드");
            }
        }
        else
        {
            //그리드에 아이템 리스트가 있는 경우
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
        //임시(데이터베이스 연동시 수정)
        Debug.Log($"랜덤 삽입 {transform.name}");
        //InventoryController.invenInstance.InsertRandomItem(this);
        
        ItemObject randomItem = Instantiate(itemPref).GetComponent<ItemObject>();
        InventoryController.invenInstance.SetSelectedObjectToLastSibling(transform);
        int randomId = Random.Range(0, InventoryController.invenInstance.itemsList.Count);
        randomItem.Set(InventoryController.invenInstance.itemsList[randomId]);

        FindPlaceableSlot(randomItem);
        randomItem.curItemGrid = this;

        randomItem.backUpItemPos = randomItem.curItemPos; //현재 위치
        randomItem.backUpItemRotate = randomItem.curItemRotate; //현재 회전
        randomItem.backUpItemGrid = randomItem.curItemGrid; //현재 그리드
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

        itemObj.backUpItemPos = itemObj.curItemPos; //현재 위치
        itemObj.backUpItemRotate = itemObj.curItemRotate; //현재 회전
        itemObj.backUpItemGrid = itemObj.curItemGrid; //현재 그리드
    }

    /// <summary>
    /// 마우스의 현 위치를 그리드의 타일 위치로 변환
    /// </summary>
    /// <param name="mousePosition">마우스 위치</param>
    public Vector2Int MouseToGridPosition(Vector2 mousePosition)
    {
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
        if (x < 0 || y < 0)
        {
            return null;
        }
        return gridItemArray[x, y];
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

        ItemObject targetItem = gridItemArray[x, y];
        
        if (targetItem == null) { return null; }

        gridWeight -= targetItem.itemData.item_weight;
        invenScr.UpdateInvenWeight();
        CleanItemSlot(targetItem);
        PrintInvenContents(this,gridItemArray);

        return targetItem;
    }

    /// <summary>
    /// 해당 아이템 크기에 맞춰 인벤토리 슬롯의 내용을 제거
    /// </summary>
    /// <param name="item">선택된 아이템</param>
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
    /// 아이템이 배치 가능한지 조건을 체크 후 배열상 아이템 배치
    /// </summary>
    public bool PlaceItemCheck(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        if(InventoryController.invenInstance.SelectedItemGrid == null)
        {
            return false;
        }

        //아이템이 그리드 밖으로 나갈시 취소
        if (BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height) == false)
        {
            return false;
        }

        //겹치는 아이템이 있다면 overlapItem변수에 할당. 여기서부터 오버랩 아이템 지정됨
        if (OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        //무게에 의해 배치가 불가능할 경우 취소
        if (invenScr.CheckingInvenWeight(placeItem.itemData.item_weight, overlapItem) == false)
        {
            return false;
        }

        //오버랩된 아이템이 있다면
        if (overlapItem != null)
        {
            //같은 소모품의 경우 배치를 하지 않은 채로 true 리턴
            if (placeItem.itemData.isItemConsumeable && 
                (placeItem.itemData.itemCode == overlapItem.itemData.itemCode)&&
                overlapItem.itemAmount < 64)
            {
                return true;
            }

            overlapItem = null;
            return false;
        }
        
        //아이템 배열에 해당 아이템 배치
        PlaceItem(placeItem, posX, posY);
        PrintInvenContents(this, gridItemArray);
        
        return true;
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
                gridItemArray[posX + x, posY + y] = item;
            }
        }

        gridWeight += item.itemData.item_weight;
        invenScr.UpdateInvenWeight();

        PlaceSprite(item, posX, posY, itemRect);
    }

    /// <summary>
    /// 아이템의 렉트를 해당위치로 이동
    /// </summary>
    public void PlaceSprite(ItemObject inventoryItem, int posX, int posY, RectTransform itemRect)
    {
        inventoryItem.curItemPos = new Vector2Int(posX, posY);
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
        itemRect.localPosition = new UnityEngine.Vector2(position.X,position.Y);
    }

    /// <summary>
    /// 그리드 내의 월드위치 계산. 아이템을 해당 위치에 고정시키기 위함
    /// </summary>
    /// <param name="inventoryItem">해당 아이템</param>
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
    /// 아이템 슬롯을 저장된 슬롯으로 되돌림.
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
        //아이템 슬롯 순회
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //해당 아이템 슬롯이 비어있지 않을 경우 
                if (gridItemArray[posX + x, posY + y] != null)
                {
                    //겹치는 아이템이 1인 경우에는 선택아이템과 교체 그 이상이라면 Place 불가
                    if (overlapItem == null)
                    {
                        //해당 좌표의 아이템을 오버렙아이템으로 지정
                        overlapItem = gridItemArray[posX + x, posY + y];
                    }
                    else
                    {
                        //이미 오버랩 아이템이 지정되어 있는데 다른 아이템이 또 오버랩되면 리턴 false;
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
    /// InventoryItemSlot을 출력
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
    /// 해당 위치가 grid의 안인지 체크
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
    /// 아이템의 크기가 그리드를 빠져나가는지 체크
    /// </summary>
    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        if (InsideGridCheck(posX, posY) == false) { return false; }
        if (InsideGridCheck(posX += (width - 1), posY += (height - 1)) == false) { return false; }
        return true;
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
                if (gridItemArray[posX + x, posY + y] != null)
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
    public Color32 PlaceCheckForHighlightColor(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        overlapItem = null;
        //그리드 밖으로 나가면  배치 실패 판정
        if (BoundaryCheck(posX, posY, placeItem.Width, placeItem.Height) == false)
        {
            return HighlightColor.Red;
        }

        //겹치는 아이템이 두개 이상 있다면 overlapItem변수 제거. 배치 실패 판정
        if (OverLapCheck(posX, posY, placeItem.Width, placeItem.Height, ref overlapItem) == false)
        {
            //오버랩 체크에서 좌표의 아이템슬롯을 검색하는 과정에서 널 좌표 발생으로 오류 발생 이를 해결할것
            overlapItem = null;
            return HighlightColor.Red;
        }

        //오버랩된 아이템이 있지만 배치가 가능할 경우
        if (overlapItem != null)
        {
            //단 오버랩 아이템이 ishide인 경우 배치 실패 판정
            if (overlapItem.ishide || placeItem.ishide)
            {
                overlapItem = null;
                return HighlightColor.Red;
            }

            //같은 소모품의 경우
            //아이템의 수가 최대라면 수량합치기 불가. 교체 판정
            if(overlapItem.itemAmount == 64)
            {
                return HighlightColor.Red;
            }

            //합치기가 가능한경우 배치 성공 판정
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
