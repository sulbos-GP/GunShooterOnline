using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryController : MonoBehaviour
{
    /*
     * 이 코드는 인벤토리 매니저에 부착되며 플레이어의 조작에 따른 함수를 호출합니다.
     * 
     * 1. PlayerInput으로 마우스의 위치, 좌우클릭 이벤트를 발생합니다.
     *    좌클릭시 ItemGetOrRelease함수를 통해 selectedItem변수의 존재 여부에 따라 아이템을 
     *     집거나 배치합니다
     *    우클릭시(임시) selectedItem이 존재하는지 여부에 따라 회전하거나 새로운 아이템을 생성
     *     합니다
     * 
     * 2. 업데이트에서 호출되는 함수
     *    GetTileGridPosition 현재 마우스의 위치를 그리드의 좌표로 변환합니다.
     *    ItemSpriteDrag selectedItem이 있다면 해당 아이템의 위치 = 마우스의 위치
     *    HandleHighlight는 selectedItem이 없을때는 배치된 아이템에 마우스를 가져다 대면 아이템
     *     에 하이라이트를 주고 selectedItem이 있다면 해당 위치에 아이템이 배치가능한지 하이라이트
     *     로 표시합니다
     * 
     * 3. CreateRandomItem : 새로운 아이템을 생성하여 selectedItem으로 지정합니다
     *    InsertRandomItem : 해당 그리드에 바로 생성한 아이템을 집어넣습니다.
     *    SetAllParentsAsLastSibling : 선택된 아이템이 다른 아이템에 가려지지 않도록 하기위해
     *    같은 부모를 가진 자식 객체중 맨 아래로 이동하며 객체의 부모, 객체의 부모의 부모 등 더이상
     *    부모 객체가 없을때까지 재귀로 반복합니다.
     *     
     */

    public static InventoryController invenInstance;
    public int playerId = 1; //임시 부여
    
    public ItemGrid SelectedItemGrid {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            invenHighlight.SetParent(value);
        }
    }

    [SerializeField] private Vector2 mousePosInput;
    [SerializeField] private Vector2Int updateGridPos;
    [SerializeField] private ItemGrid selectedItemGrid;
    [SerializeField] private List<ItemObjData> items; //이 그리드에 넣어질 아이템 리스트
    [SerializeField] private GameObject itemPref;
    [SerializeField] private InventoryItem selectedItem;
    [SerializeField] private RectTransform selectedRect;

    private InventoryItem overlapItem; //아이템을 놓을때 체크된 오버랩아이템
    private InventoryItem checkOverlapItem; //업데이트 메서드에서 체크된 오버랩 아이템
    private PlayerInput playerInput;

    //하이라이트 관련 변수
    private InvenHighLight invenHighlight;
    private InventoryItem itemToHighlight;
    private Color32 highlightColor;
    private Vector2Int HighlightPosition;

    public List<InventoryItem> backUpItemList = new List<InventoryItem>();
    public List<ItemGrid> backUpGridList = new List<ItemGrid>();

    private void Awake()
    {
        if (invenInstance == null)
        {
            invenInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        invenHighlight = GetComponent<InvenHighLight>();
    }

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.UI.Enable();
        playerInput.UI.MouseMove.performed += OnMousePosInput;
        playerInput.UI.MouseLeftClick.performed += OnMouseLeftClickInput;
        playerInput.UI.MouseRightClick.performed += OnMouseRightClickInput;
    }

    private void OnDisable()
    {
        playerInput = new PlayerInput();
        playerInput.UI.MouseMove.performed -= OnMousePosInput;
        playerInput.UI.MouseLeftClick.performed -= OnMouseLeftClickInput;
        playerInput.UI.MouseRightClick.performed -= OnMouseRightClickInput;
        playerInput.UI.Disable();
    }

    #region PlayerInput 액션
    private void OnMousePosInput(InputAction.CallbackContext context)
    {
        //마우스의 위치 실시간 업데이트
        mousePosInput = context.ReadValue<Vector2>();
    }

    private void OnMouseLeftClickInput(InputAction.CallbackContext context)
    {
        //좌클릭시 아이템을 집거나 놓는 이벤트
        ItemGetOrRelease();
    }
    private void OnMouseRightClickInput(InputAction.CallbackContext context)
    {
        //마우스 우클릭시. 선택된 아이템 여부에 따라 새 아이템 생성 혹은 아이템 회전
        if (selectedItem == null)
        {
            CreateRandomItem();
        }
        else
        {
            RotateItemRight();
        }
    }
    #endregion

    private void Update()
    {
        ItemSpriteDrag();
        
        //하이라이트 연출
        if (selectedItemGrid == null)
        {
            invenHighlight.Show(false);
            return;
        }

        updateGridPos =  GetTileGridPosition();
        //Debug.Log(updateGridPos);
        if (selectedItem != null)
        {
            highlightColor = selectedItemGrid.PlaceCheckForHighlightColor(selectedItem, updateGridPos.x, updateGridPos.y, ref checkOverlapItem);
            invenHighlight.SetColor(highlightColor);
            checkOverlapItem = null;
        }
        HandleHighlight();
    }

    /// <summary>
    /// 마우스의 위치를 Grid상의 타일 위치로 변환
    /// </summary>
    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = mousePosInput;
        //아이템을 들고있다면
        if (selectedItem != null)
        {
            //아이템이 마우스 중앙에 오도록 보정
            position.x -= (selectedItem.Width - 1) * ItemGrid.tilesizeWidth / 2;
            position.y += (selectedItem.Height - 1) * ItemGrid.tilesizeHeight / 2;
        }

        return selectedItemGrid.MouseToGridPosition(position);
    }

    /// <summary>
    /// 선택된 아이템이 있을경우 해당 아이템은 마우스를 따라다님
    /// </summary>
    private void ItemSpriteDrag()
    {
        if (selectedItem != null)
        {
            selectedRect.position = mousePosInput;
        }
    }

    /// <summary>
    /// 컨트롤러에서 아이템 유회전 명령
    /// </summary>
    public void RotateItemRight()
    {
        if(selectedItem == null) { return; }
        selectedItem.RotateRight();
    }

    /// <summary>
    /// 컨트롤러에서 아이템 좌회전 명령
    /// </summary>
    public void RotateItemLeft()
    {
        if (selectedItem == null) { return; }
        selectedItem.RotateLeft();
    }

    /// <summary>
    /// 좌클릭시 아이템을 집거나 내려놓음
    /// </summary>
    private void ItemGetOrRelease()
    {
        if (selectedItemGrid == null) { return; }
        Vector2Int tileGridPosition = updateGridPos; //마우스의 위치에 있는 그리드 좌표 할당
        if (selectedItem == null)
        {
            InventoryItem clickedItem = selectedItemGrid.GetItem(tileGridPosition.x, tileGridPosition.y);
            if (clickedItem == null) { return; }
            if (clickedItem.ishide == true)
            {
                clickedItem.SearchingItem();
            }
            else
            {
                ItemGet(tileGridPosition);
            }
        }
        else
        {
            ItemRelease(selectedItem, tileGridPosition);
        }
    }

    /// <summary>
    /// 아이템을 집는 시도. 성공하면 selectedRect를 해당 아이템의 rect로 지정
    /// </summary>
    private void ItemGet(Vector2Int pos)
    {
        selectedItem = selectedItemGrid.PickUpItem(pos.x, pos.y);
        if (selectedItem != null)
        {
            selectedRect = selectedItem.GetComponent<RectTransform>();
            //포인터 핸들러가 이미지에 의해 가려지는 것을 방지
            selectedItem.GetComponent<Image>().raycastTarget = false;
            //아이템이 그리드에 가려지는것을 방지
            SetAllParentsAsLastSibling(selectedRect);
        }
    }

    /// <summary>
    /// 아이템을 놓는 시도.
    /// </summary>
    private void ItemRelease(InventoryItem item,Vector2Int pos)
    {
        bool complete = selectedItemGrid.PlaceItemCheck(item, pos.x, pos.y, ref overlapItem);
        if (complete)
        {
            selectedItem.GetComponent<Image>().raycastTarget = true;
            selectedItem.curItemGrid = selectedItemGrid;
            //겹치는 아이템이 있다면 selectedItem으로 지정
            if (overlapItem != null)
            {
                backUpItemList.Add(selectedItem); //백업시 해당 리스트 안의 아이템 전부 백업
                if(backUpGridList.Contains(selectedItem.backUpItemGrid) == true)
                {
                    backUpGridList.Add(selectedItem.backUpItemGrid);
                }

                selectedItem = overlapItem;
                selectedRect = selectedItem.GetComponent<RectTransform>();
                selectedItem.GetComponent<Image>().raycastTarget = false;
                overlapItem = null;
                SetAllParentsAsLastSibling(selectedRect);
            }
            else
            {
                //겹치는 아이템이 없을경우 백업변수 업데이트
                selectedItem.backUpItemPos = pos; //현재 위치
                selectedItem.backUpItemRotate = selectedItem.curItemRotate; //현재 회전
                selectedItem.backUpItemGrid = selectedItem.curItemGrid; //현재 그리드
                selectedItem = null;

                if(backUpItemList.Count != 0)
                {
                    //백업 리스트 안의 모든 아이템 또한 백업 업데이트
                    for (int i = 0; i < backUpItemList.Count; i++)
                    {
                        backUpItemList[i].backUpItemRotate = backUpItemList[i].curItemRotate;
                        backUpItemList[i].backUpItemPos = backUpItemList[i].curItemPos;
                        backUpItemList[i].backUpItemGrid = backUpItemList[i].curItemGrid;
                    }
                    backUpItemList.Clear(); //완료시 리스트 초기화
                }

                selectedItemGrid.UpdateBackupSlot(); //현재 백업 아이템 배열 업데이트

                //패킷 전송(인벤토리의 내용)
                //플레이어 아이디, 인벤토리1의 id와 내용,인벤토리2의 id와 내용
            }
        }
        else
        {
            //아이템 배치 실패시 아이템을 원래 위치로
            //아이템 슬롯 원위치
            Vector2Int backUpPos = selectedItem.backUpItemPos;
            selectedItem.curItemRotate = selectedItem.backUpItemRotate;
            selectedItem.Rotate(selectedItem.backUpItemRotate);
            selectedItemGrid.PlaceSprite(selectedItem, backUpPos.x, backUpPos.y, selectedRect);
            
            selectedItem = null;
            selectedRect = null;

            if(backUpItemList.Count != 0)
            {
                for (int i = 0; i < backUpItemList.Count; i++)
                {
                    InventoryItem backUpItem = backUpItemList[i];
                    RectTransform backUpRect = backUpItem.GetComponent<RectTransform>();
                    backUpPos = backUpItem.backUpItemPos;
                    if (backUpItem.curItemRotate != backUpItem.backUpItemRotate)
                    {
                        backUpItem.curItemRotate = backUpItem.backUpItemRotate;
                        backUpItem.Rotate(backUpItem.backUpItemRotate);
                    }
                    if (backUpPos != backUpItem.curItemPos || backUpItem.backUpItemGrid != backUpItem.curItemGrid)
                    {
                        backUpItem.backUpItemGrid.PlaceSprite(backUpItem, backUpPos.x, backUpPos.y, backUpRect);
                    }
                    
                }
                backUpItemList.Clear();
            }

            Debug.Log("되돌림");
            selectedItemGrid.UndoItemSlot();
            selectedItemGrid.PrintInventoryContents(selectedItemGrid.inventoryItemSlot);
        }
    }

    /// <summary>
    /// 아이템에 마우스를 가져대면 하이라이트 효과
    /// 나중에 상태별 하이라이트 색깔 추가 예정
    /// </summary>
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = updateGridPos;
        if(positionOnGrid == null)
        {
            invenHighlight.Show(false);
            return;
        }
        if(HighlightPosition == positionOnGrid)
        {
            return;
        }
        HighlightPosition = positionOnGrid;

        if (selectedItem == null)
        {
            invenHighlight.SetColor(HighlightColor.Gray);
            //선택된 아이템이 없을경우 마우스가 가리키는 위치의 아이템이 있다면 하이라이팅. 없으면 비활성화
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x,positionOnGrid.y);
            if(itemToHighlight != null)
            {
                invenHighlight.Show(true);
                invenHighlight.SetSize(itemToHighlight);
                invenHighlight.SetParent(selectedItemGrid);
                invenHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                invenHighlight.Show(false);
            }
        }
        else
        {
            //선택된 아이템이 있다면 그 아이템이 위치한 곳에 하이라이팅
            invenHighlight.Show(selectedItemGrid.BoundaryCheck(positionOnGrid.x,positionOnGrid.y,
                selectedItem.Width,selectedItem.Height));
            invenHighlight.SetSize(selectedItem);
            invenHighlight.SetParent(selectedItemGrid);
            invenHighlight.SetPositionByPos(selectedItemGrid, selectedItem,positionOnGrid.x,positionOnGrid.y);
        }
    }

    /// <summary>
    /// 등록된 아이템중 랜덤으로 생성
    /// </summary>
    private void CreateRandomItem()
    {
        //아이템 프리팹을 생성하고 스크립트 로드
        InventoryItem invenItem = Instantiate(itemPref).GetComponent<InventoryItem>();
        //현재 선택된 아이템과 렉트트랜스폼을 이것으로 지정함
        selectedItem = invenItem;
        selectedRect = invenItem.GetComponent<RectTransform>();
        //아이템은 캔버스의 자식(UI니까)
        SetAllParentsAsLastSibling(selectedRect);
        //아이템 리스트 중 하나 지정
        int selectedItemId = Random.Range(0, items.Count);
        //지정된 아이템 데이터를 아이템 프리팹에 적용

        invenItem.Set(items[selectedItemId]);
        selectedItem.GetComponent<Image>().raycastTarget = false;
    }

    /// <summary>
    /// 아이템이 생성된 즉시 인벤토리내에 배치 가능한 자리에 배치됨
    /// </summary>
    public void InsertRandomItem(ItemGrid targetGrid)
    {
        selectedItemGrid = targetGrid;
        if (selectedItemGrid == null) { return; }

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    /// <summary>
    /// insertRandomItem안에서 지정된 아이템을 가능한 자리에 배치
    /// </summary>
    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if(posOnGrid == null) { return; }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        selectedItemGrid = null;
    }

    /// <summary>
    /// 모든 부모에게 LastSibling을 적용함. 항상 지정한 인터렉션한 rect을 가장 앞으로.
    /// </summary>
    private void SetAllParentsAsLastSibling(Transform child)
    {
        if (child == null) return;

        child.SetAsLastSibling();

        if (child.parent != null)
        {
            SetAllParentsAsLastSibling(child.parent);
        }
    }
}


