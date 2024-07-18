using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    /*
     * 이 코드는 Grid에 부착되며 실질적인 인벤토리의 부분입니다. 
     * 인벤토리의 타일크기, 내용, 위치 등을 관리합니다.
     * 
     * 1. Init함수는 그리드데이터에서 x,y의 타일수를 받아 인벤토리의 크기를 설정하고 인벤토리
     *    의 내용을 초기화합니다. OtherUISet함수를 통해 다른 UI들도 그리드에 맞춰 설정합니다
     *    
     * 2. MouseToGridPosition함수는 컨트롤러에서 호출되며 마우스의 위치를 그리드의 좌표로 
     *    변형하여 그 좌표를 리턴합니다. Vector2 -> Vector2Int
     *    
     * 3. PickUpItem함수 실질적인 코드중 하나이며 x,y 그리드 좌표를 받아 해당 좌표에 아이템이 
     *    존재하는지 검색하고 있다면 해당 아이템을 반환합니다.
     *    비슷한 코드로 GetItem이 있는데 PickUpItem은 해당 위치에 존재하면 아이템 슬롯의 내용을
     *    지우지만 GetItem은 내용을 지우는것 없이 무슨 아이템이 있는지만 확인하는 용도입니다.
     * 
     * 4. CleanItemReference은 매개변수로 받아온 아이템의 위치와 크기를 통해 inventoryItemSlot
     *    의 내용을 삭제합니다.
     *    
     * 5. PlaceItemCheck은 실질적인 코드중 하나로 해당 위치에 배치가 가능한지InsideGridCheck(해당 
     *    위치가 그리드의 안인지),BoundaryCheck(아이템의 크기가 그리드를 넘어가는지),
     *    CheckAvailableSpace(공간의 크기가아이템의 크기만큼 큰지) 확인하고
     *    
     *    겹치는 아이템이 1개거나 배치가 가능하다면 PlaceItem을 통해 아이템을 배치하고 true를 반환하며
     *    겹치는 아이템이 2개 이상이거나 배치가 불가능하다면 그대로 false를 리턴합니다.
     *    위의 내용은 inventoryItemSlot상의 배치이미 PlaceSprite를 통해 해당 위치로 실제 
     *    아이템의 위치를 이동시킵니다.
     * 
     * 6. CalculatePositionOnGrid은 아이템 객체를 해당 좌표로 이동시키기 위해 그리드 상의 좌표를
     *    월드위치로 바꿔줍니다.
     *    
     * 7. UpdateBackupSlot과 UndoItemSlot은 백업을 담당하며 성공적으로 배치될때 현재 
     *    inventoryItemSlot을 복사한 savedInvenItemSlot을 통해 배치에 성공하면 inventoryItemSlot을
     *    savedInvenItemSlot에 실패했다면 그 반대로 덮어쓰게 됩니다.
     *    
     * 8. PlaceCheckForHighlightColor는 배치 가능여부에 따라 하이라이트 색깔을 반환합니다.
     *    PlaceItemCheck와 유사합니다만 오버랩아이템의 슬롯을 지우지 않아 완전히 같은 기능은 아니니
     *    주의해야합니다.
     */

    public int gridId = 0; // 임의 할당
    public InventoryItem[,] inventoryItemSlot; //인벤토리 내용이 기록될 배열

    //타일의 크기 offset
    public const float tilesizeWidth = 100;
    public const float tilesizeHeight = 100;

    //그리드 설정
    private RectTransform rectTransform; //해당 그리드의 transform
    private Vector2Int gridSize; //그리드의 폭,높이별 타일 개수 입력
    private Vector2 positionOnTheGrid = new Vector2(); //그리드 위의 마우스 위치
    private Vector2Int tileGridPosition = new Vector2Int(); //마우스 아래의 타일 위치좌표
    public GridObjectData gridObjData; // 해당 그리드의 데이터(그리드 크기)

    //백업 관련
    public InventoryItem[,] savedInvenItemSlot; //백업 배열
    [SerializeField]private GameObject inventoryItemPref; //아이템의 prefab

    //다른 해당 인벤토리의 다른 UI
    public GameObject BgPanel;
    public GameObject HeaderPanel;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gridSize = gridObjData.gridSize;
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
        //Init에 인벤토리안의 내용을 받아서 반영하는 코드 짤예정
        inventoryItemSlot = new InventoryItem[width, height];
        savedInvenItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tilesizeWidth, height * tilesizeHeight);
        rectTransform.sizeDelta = size;
        OtherUISet();
    }

    /// <summary>
    /// 배경과 헤더 UI 오브젝트의 위치나 크기 지정.
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

        // 헤더 패널의 위치 조정
        headerRect.sizeDelta = new Vector2(rectTransform.sizeDelta.x, headerRect.sizeDelta.y);
        headerRect.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + headerRect.sizeDelta.y + offsetY);
    }

    /// <summary>
    /// 마우스의 현 위치를 그리드의 타일 위치로 변환
    /// </summary>
    /// <param name="mousePosition">마우스 위치</param>
    public Vector2Int MouseToGridPosition(Vector2 mousePosition)
    {
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;
        tileGridPosition.x = (int)(positionOnTheGrid.x / tilesizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tilesizeHeight);
        return tileGridPosition;
    }

    /// <summary>
    /// 해당 좌표에 있는 아이템 반환
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
    /// 해당 좌표의 타일에 아이템이 있는지 검색 후 해당 아이템을 리턴
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
    /// 아이템 크기에 맞춰 인벤토리 슬롯의 내용을 제거
    /// </summary>
    /// <param name="item">선택된 아이템</param>
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
    /// 아이템이 배치 가능한지 조건을 체크 후 배열상 아이템 배치
    /// </summary>
    public bool PlaceItemCheck(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        //그리드 밖으로 나가는지 체크
        if (BoundaryCheck(posX, posY, inventoryItem.Width, inventoryItem.Height) == false)
        {
            return false;
        }

        //겹치는 아이템이 있다면 overlapItem변수 초기화
        if (OverLapCheck(posX, posY, inventoryItem.Width, inventoryItem.Height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        //오버랩된 아이템이 있다면 해당 오버랩 아이템을 배열에서 제거
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
    /// 오버랩이 없는 PlaceItemCheck. 대상과 위치만으로 아이템 슬롯 지정
    /// </summary>
    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        //해당 아이템의 크기대로 좌표에 저장
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
        //이미지 스프라이트도 고정
        inventoryItem.curItemPos = new Vector2Int(posX, posY);
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
        rectTransform.localPosition = position;
    }

    /// <summary>
    /// 그리드 내의 월드위치 계산. 아이템을 해당 위치에 고정시키기 위함
    /// </summary>
    /// <param name="inventoryItem">해당 아이템</param>
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
    /// 아이템 슬롯을 저장된 슬롯으로 되돌림.
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
        //아이템 슬롯 순회
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //해당 아이템 슬롯이 비어있지 않을 경우 
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    //겹치는 아이템이 1인 경우에는 선택아이템과 교체 그 이상이라면 Place 불가
                    if (overlapItem == null)
                    {
                        //해당 좌표의 아이템을 오버렙아이템으로 지정
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        //이미 오버랩 아이템이 지정되어 있는데 다른 아이템이 또 오버랩되면 리턴 false;
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
    /// InventoryItemSlot을 디버그.
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
    /// 아이템의 크기만큼 들어갈 장소를 찾음
    /// ?를 쓴 이유는 마지막 리턴값에 널값을 허용하기 위함
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
                if (inventoryItemSlot[posX + x, posY + y] != null)
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
    public Color32 PlaceCheckForHighlightColor(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem checkOverlap)
    {
        //그리드 밖으로 나가는지 체크
        if (BoundaryCheck(posX, posY, inventoryItem.Width, inventoryItem.Height) == false)
        {
            return HighlightColor.Red;
        }

        //겹치는 아이템이 두개 이상 있다면 overlapItem변수 초기화후 false 반환
        if (OverLapCheck(posX, posY, inventoryItem.Width, inventoryItem.Height, ref checkOverlap) == false)
        {
            //오버랩 체크에서 좌표의 아이템슬롯을 검색하는 과정에서 널 좌표 발생으로 오류 발생 이를 해결할것
            checkOverlap = null;
            return HighlightColor.Red;
        }

        //오버랩된 아이템이 있지만 배치가 가능할 경우
        if (checkOverlap != null)
        {
            //단 오버랩 아이템이 ishide인 경우 false
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
