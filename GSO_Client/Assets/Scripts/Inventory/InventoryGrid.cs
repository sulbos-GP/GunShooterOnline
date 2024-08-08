using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Random = UnityEngine.Random;
using Google.Protobuf.Protocol;
using UnityEngine.Rendering;
using System.Collections.Generic;


public class InventoryGrid : MonoBehaviour
{
    /*
     * 인벤토리에 의해 UI에서 생성된 그리드(인벤UI를 끌때 플레이어의 그리드라면 남겨져 있다만 other의 그리드라면 파괴됨)
     * 
     * 그리드의 사이즈와 위치를 설정하고 내부에 들어있는 아이템들에 대해 아이템 오브젝트를 생성하여 그리드데이터.아이템 데이터를 할당하여 배치
     */

    //그리드 설정
    public GridData gridData; //인벤토리에서 할당된 그리드의 데이터
    public InventoryUI ownInven; //해당 그리드를 포함하는 인벤토리
    public ItemObject[,] ItemSlot; //컨트롤러에서 아이템을 저장할 슬롯

    //gridWeight 옵저버. 호출시 gridWeight 업데이트 및 인벤토리의 무게도 업데이트

    public float GridWeight
    {
        get => gridWeight;
        set
        {
            gridWeight = value;
            
            ownInven.UpdateInvenWeight(); //인벤토리의 무게 변경
        }
    }
    [SerializeField]protected float gridWeight = 0; //그리드안의 아이템의 무게

    //타일의 크기 offset
    public const float WidthOfTile = 100;
    public const float HeightOfTile = 100;

    private RectTransform gridRect; //해당 그리드의 transform
    private Vector2 mousePosOnGrid = new Vector2(); //그리드 위의 마우스 위치
    private Vector2Int tileGridPos = new Vector2Int(); //마우스 아래의 타일 위치좌표

    //백업 관련
    public ItemObject[,] backUpSlot; //백업 배열
    public float backupWeight;

    //아이템 생성
    public GameObject itemPref; //아이템의 prefab
    //private bool createRandomItem;


    private void Awake()
    {
        gridRect = GetComponent<RectTransform>();
        
    }

    /// <summary>
    /// 인벤토리에서 그리드를 생성할때 사용
    /// </summary>
    public void GridDataSet()
    {
        if (gridData == null)
        {
            Debug.Log("해당 그리드 오브젝트에 그리드 데이터가 없음");
            return;
        }


        if (gridData.gridSize.x <= 0 || gridData.gridSize.y <= 0)
        {
            Debug.Log($"그리드 사이즈가 적절하지 않음 : sizeX : {gridData.gridSize.x}, sizeY : {gridData.gridSize.y}");
            return;
        }

        int width = gridData.gridSize.x;
        int height = gridData.gridSize.y;  

        ItemSlot = backUpSlot = new ItemObject[width, height]; 

        Vector2 rectSize = new Vector2(width * WidthOfTile, height * HeightOfTile);
        gridRect.sizeDelta = new UnityEngine.Vector2(rectSize.X, rectSize.Y);
        
        //createRandomItem = gridData.createRandomItem; 클라에서 만들필요 없음

        GridRectPosSet();
    }

    /// <summary>
    /// 그리드의 렉트를 설정
    /// </summary>
    private void GridRectPosSet()
    {
        //그리드가 벽에 딱 붙지 않게 약간의 오프셋을 둠
        Vector2 offsetGridPosition = new Vector2(gridData.gridPos.X + InventoryUI.offsetX, gridData.gridPos.Y - InventoryUI.offsetY);
        transform.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(offsetGridPosition.X, offsetGridPosition.Y);
    
        //그리드의 배치가 완료되면 그리드의 아이템슬롯을 설정하고 아이템 프리팹 또한 배치
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
        //서버에서 받은 리스트를 gridItemArray에 할당하고
        //아이템 프리팹을 생성후 해당 아이템의 데이터를 넣어주기
        /*if(gridData.itemList.Count == 0)
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

        itemObj.backUpItemPos = itemObj.itemData.itemPos; //현재 위치
        itemObj.backUpItemRotate = itemObj.itemData.itemRotate; //현재 회전
        itemObj.backUpItemGrid = itemObj.curItemGrid; //현재 그리드

        ownInven.instantItemList.Add(itemObj);
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
                ItemSlot[item.itemData.itemPos.x + x, item.itemData.itemPos.y + y] = null;
            }
        }
    }

    
    /// <summary>
    /// 아이템이 배치 가능한지 조건을 체크 후 배열상 아이템 배치
    /// </summary>
    public bool PlaceItemCheck(ItemObject placeItem, int posX, int posY, ref ItemObject overlapItem)
    {
        //현재 선택된 그리드가 없다면 배치 실패
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
        if (ownInven.CheckingInvenWeight(placeItem.itemData.item_weight, overlapItem) == false)
        {
            return false;
        }

        //오버랩된 아이템이 있다면
        if (overlapItem != null)
        {
            //같은 소모품의 경우 배치를 하지 않은 채로 true 리턴
            if (placeItem.itemData.isItemConsumeable && 
                (placeItem.itemData.itemCode == overlapItem.itemData.itemCode)&&
                overlapItem.itemData.itemAmount < 64)
            {
                return true;
            }

            overlapItem = null;
            return false;
        }
        
        //아이템 배열에 해당 아이템 배치
        PlaceItem(placeItem, posX, posY);
        PrintInvenContents(this, ItemSlot);
        
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
                ItemSlot[posX + x, posY + y] = item;
            }
        }

        GridWeight += item.itemData.item_weight;

        PlaceSprite(item, posX, posY, itemRect);
    }

    /// <summary>
    /// 아이템의 렉트를 해당위치로 이동
    /// </summary>
    public void PlaceSprite(ItemObject inventoryItem, int posX, int posY, RectTransform itemRect)
    {
        inventoryItem.itemData.itemPos = new Vector2Int(posX, posY);
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
        Vector2Int newPos = item.itemData.itemPos;

        // 이전 위치의 아이템 리스트에서 아이템을 제거합니다.
        RemoveItemFromItemList(item);

        // 새로운 위치에 아이템을 추가합니다.
        AddItemToItemList(newPos, item);
    }

    public void AddItemToItemList(Vector2Int pos, ItemObject item)
    {
        // gridData의 itemList에서 아이템을 추가합니다.
        ItemData itemData = item.itemData;
        if (itemData == null) return;

        // 아이템의 새 위치를 itemData에 설정합니다.
        itemData.itemPos = pos;

        // itemList에서 아이템을 추가합니다.
        gridData.itemList.Add(itemData);
    }

    public void RemoveItemFromItemList(ItemObject item)
    {
        // gridData의 itemList에서 아이템을 제거합니다.
        ItemData itemData = item.itemData;
        if (itemData == null) return;

        // itemList에서 해당 아이템을 제거합니다.
        gridData.itemList.Remove(itemData);
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
    /// 아이템 슬롯을 저장된 슬롯으로 되돌림.
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
                    //겹치는 아이템이 1인 경우에는 선택아이템과 교체 그 이상이라면 Place 불가
                    if (overlapItem == null)
                    {
                        //해당 좌표의 아이템을 오버렙아이템으로 지정
                        overlapItem = ItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        //이미 오버랩 아이템이 지정되어 있는데 다른 아이템이 또 오버랩되면 리턴 false;
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
    /// 해당 위치가 grid의 안인지 체크
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
            if(overlapItem.itemData.itemAmount == 64)
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

    //랜덤 아이템 생성은 서버가 담당함. 클라는 필요없음, 받은 데이터를 잘 적용만 시키면됨
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
        //임시(데이터베이스 연동시 수정)
        Debug.Log($"랜덤 삽입 {transform.name}");
        //InventoryController.invenInstance.InsertRandomItem(this);
        
        ItemObject randomItem = Instantiate(itemPref).GetComponent<ItemObject>();
        InventoryController.invenInstance.SetSelectedObjectToLastSibling(transform);
        int randomId = Random.Range(0, InventoryController.invenInstance.itemsList.Count);
        randomItem.ItemDataSet(InventoryController.invenInstance.itemsList[randomId]);

        FindPlaceableSlot(randomItem);
        randomItem.curItemGrid = this;

        randomItem.backUpItemPos = randomItem.curItemPos; //현재 위치
        randomItem.backUpItemRotate = randomItem.curItemRotate; //현재 회전
        randomItem.backUpItemGrid = randomItem.curItemGrid; //현재 그리드
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
   }*/
}
