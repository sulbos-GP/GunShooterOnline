using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml.Diagram;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController : MonoBehaviour
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
     *    WorldToGridPos 현재 마우스의 위치를 그리드의 좌표로 변환합니다.
     *    DragObject selectedItem이 있다면 해당 아이템의 위치 = 마우스의 위치
     *    HandleHighlight는 selectedItem이 없을때는 배치된 아이템에 마우스를 가져다 대면 아이템
     *     에 하이라이트를 주고 selectedItem이 있다면 해당 위치에 아이템이 배치가능한지 하이라이트
     *     로 표시합니다
     * 
     * 3. CreateRandomItem : 새로운 아이템을 생성하여 selectedItem으로 지정합니다
     *    InsertRandomItem : 해당 그리드에 바로 생성한 아이템을 집어넣습니다.
     *    SetSelectedObjectToLastSibling : 선택된 아이템이 다른 아이템에 가려지지 않도록 하기위해
     *    같은 부모를 가진 자식 객체중 맨 아래로 이동하며 객체의 부모, 객체의 부모의 부모 등 더이상
     *    부모 객체가 없을때까지 재귀로 반복합니다.
     */

    public static InventoryController invenInstance;
    private PlayerInput playerInput; //플레이어의 조작 인풋
    public GameObject inventoryUI;
    public PlayerInventoryUI playerInvenUI;
    public OtherInventoryUI otherInvenUI;
    //public ItemDB itemdb;
    //public List<ItemData> itemList;

    [Header("수동지정")]
    public Transform deleteUI;
    public Button rotateBtn;

    //우클릭 아이템 생성 예시
    [SerializeField] private GameObject itemPref; //생성할 아이템의 프리펩(임시)

    [Header("디버그용")]
    [SerializeField] private Vector2 mousePosInput; //마우스의 월드 위치
    [SerializeField] private Vector2Int updateGridPos; //그리드상의 좌표 위치

    //그리드의 입력 변화에 따라 아래 변수 업데이트 및 하이라이트 객체의 부모객체로 설정
    public InventoryGrid SelectedItemGrid
    {
        get => selectedGrid;
        set
        {
            selectedGrid = value;
            isGridSelected = selectedGrid != null;
            if (isGridSelected)
            {
                invenHighlight.SetParent(value);
            }
            else
            {
                invenHighlight.SetParent(null);
            }
        }
    }
    [SerializeField] private InventoryGrid selectedGrid; //현재 마우스가 위치한 그리드
    public bool isGridSelected; //그리드가 선택되었는지

    //선택된 아이템의 변화에 따라 아래변수 업데이트
    public ItemObject SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            isItemSelected = selectedItem != null;
            if (isItemSelected)
            {
                selectedRect = value.GetComponent<RectTransform>();
                rotateBtn.interactable = true;
            }
            else
            {
                selectedRect = null;
                rotateBtn.interactable = false;
            }
        }
    }
    [SerializeField] private ItemObject selectedItem; //현재 선택된 아이템
    [SerializeField] private RectTransform selectedRect; //선택된 아이템의 Rect
    public bool isItemSelected; //현재 선택된 상태인지

    private ItemObject placeOverlapItem; //아이템을 배치할때 체크될 오버랩 아이템 변수
    [SerializeField] private ItemObject checkOverlapItem; //매 프레임마다 체크될 오버랩 아이템 변수

    //하이라이트 관련 변수
    private InvenHighLight invenHighlight; //하이라이트 객체에 있는 변수
    private Vector2Int HighlightPosition; //하이라이트의 위치

    //삭제 관련
    public bool isOnDelete;
    public bool IsOnDelete
    {
        get => isOnDelete;
        set
        {
            isOnDelete = value;
            deleteUI.GetComponent<DeleteZone>().IsDeleteOn = value;
        }
    }

    //UI액티브 관련
    public bool isActive = false;

    //백업관련 변수(오버랩 아이템 교체가 없어져서 필요 없음)
    //public List<ItemObject> backUpItemList = new List<ItemObject>(); //백업할 아이템들의 리스트
    //public List<InventoryGrid> backUpGridList = new List<InventoryGrid>(); //백업할 그리드들의 리스트


    /*
    //인벤토리 드래그 관련
    [SerializeField] private InvenData selectedInven; //드래그할 인벤토리
    public bool isInvenSelected; //드래그할 인벤이 선택됨
    public bool isDragging;
    public RectTransform draggedInvenRect;
    public InvenData SelectedInven
    {
        get => selectedInven;
        set
        {
            selectedInven = value;
            isInvenSelected = selectedInven != null;
            //draggedInvenRect 지정및 해제나 dragOffset초기화를 여기에 넣으면
            //마우스를 빠르게 움직일 경우 마우스포인터가 인벤토리를 벗어나 풀려버림
            //드래그 이벤트가 일어날때만 Rect 지정및 해제할것
        }
    }
    private Vector2 dragOffset; //드래그하는 인벤토리 렉트의 중심에서 마우스까지의 거리
    */

    //기타 변수

    

    private void Awake()
    { 
        Debug.Log("invenInstance");
        if (invenInstance == null)
        {
            invenInstance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }


        /*itemdb = new ItemDB();
        foreach (ItemDataInfo data in itemdb.items)
        {
            ItemData newData = new ItemData();
            newData.SetItemData(data);
            itemList.Add(newData);
        }*/

        invenHighlight = GetComponent<InvenHighLight>();
        //Managers.Network.ConnectToGame();
    }

    #region PlayerInput 액션
    private void OnMousePosInput(InputAction.CallbackContext context)
    {
        //마우스의 위치 실시간 업데이트
        UnityEngine.Vector2 pos = context.ReadValue<UnityEngine.Vector2>();
        mousePosInput = new Vector2(pos.x,pos.y);
    }

    private void OnMouseLeftClickStartInput(InputAction.CallbackContext context)
    {
        if(!isItemSelected)
        {
            LeftClickEvent();
        }
    }
    private void OnMouseLeftClickCancelInput(InputAction.CallbackContext context)
    {
        if (isItemSelected)
        {
            LeftClickEvent();
        }
    }

    private void OnMouseRightClickInput(InputAction.CallbackContext context)
    {
        //마우스 우클릭시. 선택된 아이템 여부에 따라 새 아이템 생성 혹은 아이템 회전
        RightClickEvent();
    }
    private void InvenUIControlInput(InputAction.CallbackContext context)
    {
        invenUIControl();
    }
    

    /*
    private void OverlapExchangeInput(InputAction.CallbackContext context)
    {
        Debug.Log("오버랩 작동");
        OverlapExchange();
    }*/

    private void LeftClickEvent()
    {
        /*
         * 플로팅 인벤토리 사용안함
        if (!isGridSelected)
        {
            DragEvent();
            return;
        }*/

        //그리드 내에 있다면 아이템 이벤트
        ItemEvent();
    }

    /*
    /// <summary>
    /// 떠다니는 인벤토리를 잡고 드래그하는 코드 (플로팅 인벤토리 제거로 안씀)
    /// </summary>
    private void DragEvent()
    {
        if (isDragging)
        {
            isDragging = false;
            draggedInvenRect = null;
            dragOffset = Vector2.zero;
        }
        else
        {
            if(!isInvenSelected) { return; }

            isDragging = true;
            draggedInvenRect = selectedInven.GetComponent<RectTransform>();
            // 클릭한 위치를 기준으로 오프셋 계산
            Vector2 clickPosition = mousePosInput;
            Vector2 inventoryCenter = selectedInven.transform.position;
            dragOffset = clickPosition - inventoryCenter;

            SetSelectedObjectToLastSibling(draggedInvenRect);
        }
    }
    */

    private void RightClickEvent()
    {
        /*//아이템을 들고있다면 회전, 없다면 임시로 새로운 아이템 생성
        if (!isItemSelected)
        {
            CreateRandomItem();
        }
        else
        {
            //컴퓨터로 조작시 사용
            RotateItemRight();
        }*/

        RotateItemRight();
    }
    #endregion

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        DragObject();

        if (!isGridSelected)
        {
            if (isOnDelete&&isItemSelected)
            {
                invenHighlight.SetColor(HighlightColor.Yellow);
                InvenHighLight.highlightObj.transform.SetParent(deleteUI);
                InvenHighLight.highlightObj.transform.position = selectedItem.transform.position;

                invenHighlight.Show(true);
                return;
            }
            invenHighlight.Show(false);
            return;
        }

        // 그리드가 존재할 경우
        if (isGridSelected)
        {
            updateGridPos = WorldToGridPos();

            if (selectedItem != null)
            {
                Color32 highlightColor = selectedGrid.PlaceCheckForHighlightColor(selectedItem, updateGridPos.x, updateGridPos.y, ref checkOverlapItem);
                invenHighlight.SetColor(highlightColor);
                
            }
            HandleHighlight();
        }
    }

    /// <summary>
    /// 마우스의 위치를 Grid상의 타일 위치로 변환
    /// </summary>
    private Vector2Int WorldToGridPos()
    {
        Vector2 position = mousePosInput;
        //아이템을 들고있다면
        if (selectedItem != null)
        {
            //아이템이 마우스 중앙에 오도록 보정
            position.X -= (selectedItem.Width - 1) * InventoryGrid.WidthOfTile / 2;
            position.Y += (selectedItem.Height - 1) * InventoryGrid.HeightOfTile / 2;
        }

        return selectedGrid.MouseToGridPosition(position);
    }

    /// <summary>
    /// 아이템을 들고 있거나 드래깅 중이라면 해당 UI의 렉트의 위치를 마우스의 위치로 계속 업데이트
    /// </summary>
    private void DragObject()
    {
        if (isItemSelected)
        {
            selectedRect.position =  new UnityEngine.Vector2(mousePosInput.X, mousePosInput.Y);
        }

        /* 플로팅 인벤토리를 사용하지 않음
        if (isDragging)
        {
            if(draggedInvenRect.gameObject.activeSelf == false)
            {
                isDragging = false;
                draggedInvenRect = null;
                dragOffset = Vector2.zero;
                return;
            }

            //단 인벤토리의 경우 클릭한 위치를 마우스의 중심으로 
            Vector2 newPosition = mousePosInput - dragOffset;
            draggedInvenRect.position = newPosition;
        }
        */
    }

    /// <summary>
    /// 아이템에 마우스를 가져대면 하이라이트 효과
    /// </summary>
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = updateGridPos;

        //그리드 위에 존재하지 않는 다면 하이라이팅 없앰
        if (positionOnGrid == null)
        {
            invenHighlight.Show(false);
            return;
        }

        //같은위치에 반복실행을 막음
        if (HighlightPosition == positionOnGrid)
        {
            return;
        }

        HighlightPosition = positionOnGrid;

        //아이템을 들고 있지 않은 경우
        if (!isItemSelected)
        {
            invenHighlight.SetColor(HighlightColor.Gray);

            //마우스의 위치에 놓여있는 아이템이 있는지 체크
            ItemObject itemToHighlight = selectedGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            
            //해당 아이템이 존재하면 그 아이템의 크기와 위치에 맞게 하이라이트
            if (itemToHighlight != null)
            {
                invenHighlight.Show(true);
                invenHighlight.SetSize(itemToHighlight);
                invenHighlight.SetParent(selectedGrid);
                invenHighlight.SetPositionOnGrid(selectedGrid, itemToHighlight);
            }
            else
            {
                //없으면 하이라이트 제거
                invenHighlight.Show(false);
            }
        }
        else
        {
            //아이템을 들고 있다면 그 아이템이 위치한 곳에 하이라이팅
            invenHighlight.Show(true);
            invenHighlight.SetSize(selectedItem);
            invenHighlight.SetParent(selectedGrid);
            invenHighlight.SetPositionOnGridByPos(selectedGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    /// <summary>
    /// 컨트롤러에서 아이템 우회전 명령
    /// </summary>
    public void RotateItemRight()
    {
        if(!isItemSelected) { return; }
        selectedItem.RotateRight();
    }

    /*
    /// <summary>
    /// 컨트롤러에서 아이템 좌회전 명령 (사용 안함)
    /// </summary>
    public void RotateItemLeft()
    {
        if (!isItemSelected) { return; }
        selectedItem.RotateLeft();
    }*/

    /// <summary>
    /// 좌클릭시 아이템을 집거나 내려 놓기
    /// </summary>
    private void ItemEvent()
    {
        Vector2Int tileGridPosition = updateGridPos; //마우스의 위치에 있는 그리드 좌표 할당
        if(tileGridPosition == null) { return; }

        if (!isItemSelected)
        {
            if (!isGridSelected)
            {
                return;
            }

            ItemObject clickedItem = selectedGrid.GetItem(tileGridPosition.x, tileGridPosition.y); 
            if (clickedItem == null) { return; }

            //클릭한 아이템이 숨겨진 경우에는 숨김을 해제하고 아니면 아이템을 듬
            if (clickedItem.ishide == true)
            {
                clickedItem.UnhideItem();
            }
            else
            {
                ItemGet(tileGridPosition);
            }
        }
        else
        {
            //이미 든 아이템이 있는경우 해당위치에 아이템을 배치
            ItemRelease(selectedItem, tileGridPosition);
        }
    }

    /// <summary>
    /// 아이템을 집는 시도.
    /// </summary>
    private void ItemGet(Vector2Int pos)
    {
        SelectedItem = selectedGrid.PickUpItem(pos.x, pos.y);
        if (!isGridSelected) { return; }
        
        //AddBackUpList(); 오버랩 교체 안씀
        

        //아이템이 그리드에 가려지는것을 방지
        SetSelectedObjectToLastSibling(selectedRect);
    }

    /// <summary>
    /// 아이템을 놓는 시도.
    /// </summary>
    private void ItemRelease(ItemObject item, Vector2Int pos)
    {
        if(isOnDelete)
        {
            //현 아이템의 기존 위치가 플레이어의 인벤토리였을 경우에만 버리기 가능.
            C_DeleteItem packet = new C_DeleteItem();
            packet.ItemId = selectedItem.itemData.itemId;
            packet.PlayerId = Managers.Object.MyPlayer.Id;
            Managers.Network.Send(packet);
            Debug.Log("C_DeleteItem");

            BackUpGridSlot();
            DestroySelectedItem();
            return;
        }
        if (!isGridSelected)
        {
            //아이템 그리드가 아닌 잘못된 위치에 놓을경우 되돌림
            UndoGridSlot();
            UndoItem();
            return;
        }

        bool complete = selectedGrid.PlaceItemCheck(item, pos.x, pos.y, ref placeOverlapItem);

        if (complete)
        {
            //SelectedItem.GetComponent<Image>().raycastTarget = true;
            SelectedItem.curItemGrid = selectedGrid;
            //겹치는 아이템이 있다면 SelectedItem으로 지정
            if (placeOverlapItem != null)
            {
                //오버랩이 있을때 아이템이 소모품이고 배치할 아이템과 종류가 같고
                //오버랩의 양이 64개 이하일 경우 아이템 합치기
                if(selectedItem.itemData.isItemConsumeable &&
                    selectedItem.itemData.itemCode == placeOverlapItem.itemData.itemCode&&
                    placeOverlapItem.itemData.itemAmount < ItemObject.maxItemMergeAmount&&
                    !placeOverlapItem.ishide)
                {
                    //두 아이템의 합이 64거나 낮으면 overlap아이템의 개수를 선택한 아이템의 양만큼 증가
                    //기존 선택한 아이템은 삭제
                    if((selectedItem.itemData.itemAmount + placeOverlapItem.itemData.itemAmount) <= ItemObject.maxItemMergeAmount)
                    {
                        //아이템 개수에 따라 무게가 가증되기로 하면 코드 추가 할것
                        selectedItem.MergeItem(placeOverlapItem, selectedItem.itemData.itemAmount);
                        
                        C_MoveItem packet = new C_MoveItem();
                        packet.PlayerId = Managers.Object.MyPlayer.Id;
                        packet.ItemId = item.itemData.itemId;
                        packet.ItemPosX = pos.x;
                        packet.ItemPosY = pos.y;
                        packet.ItemRotate = item.itemData.itemRotate;
                        packet.InventoryId = item.curItemGrid.ownInven.invenData.inventoryId;
                        packet.GridId = item.curItemGrid.gridData.gridId;
                        packet.LastItemPosX = item.backUpItemPos.x;
                        packet.LastItemPosY = item.backUpItemPos.y;
                        packet.LastItemRotate = item.backUpItemRotate;
                        packet.LastGridId = item.backUpItemGrid.gridData.gridId;
                        Managers.Network.Send(packet);
                        
                        BackUpGridSlot();
                        DestroySelectedItem();
                    }
                    else
                    {
                        int needAmount = ItemObject.maxItemMergeAmount - placeOverlapItem.itemData.itemAmount;
                        selectedItem.MergeItem(placeOverlapItem, needAmount);

                        C_MoveItem packet = new C_MoveItem();
                        packet.PlayerId = Managers.Object.MyPlayer.Id;
                        packet.ItemId = item.itemData.itemId;
                        packet.ItemPosX = pos.x;
                        packet.ItemPosY = pos.y;
                        packet.ItemRotate = item.itemData.itemRotate;
                        packet.InventoryId = item.curItemGrid.ownInven.invenData.inventoryId;
                        packet.GridId = item.curItemGrid.gridData.gridId;
                        packet.LastItemPosX = item.backUpItemPos.x;
                        packet.LastItemPosY = item.backUpItemPos.y;
                        packet.LastItemRotate = item.backUpItemRotate;
                        packet.LastGridId = item.backUpItemGrid.gridData.gridId;
                        Managers.Network.Send(packet);

                        UndoGridSlot();
                        UndoItem();
                    }

                    
                    SelectedItem = null;
                    placeOverlapItem = null;
                    return;
                }

                //병합하는 경우가 아니라면 실패. 아이템과 어레이를 원래 위치로
                placeOverlapItem = null;
                UndoGridSlot();
                UndoItem();
            }
            else
            {
                C_MoveItem packet = new C_MoveItem();
                packet.PlayerId = Managers.Object.MyPlayer.Id;
                packet.ItemId = item.itemData.itemId;
                packet.ItemPosX = pos.x;
                packet.ItemPosY = pos.y;
                packet.ItemRotate = item.itemData.itemRotate;
                packet.InventoryId = item.curItemGrid.ownInven.invenData.inventoryId;
                packet.GridId = item.curItemGrid.gridData.gridId;
                packet.LastItemPosX = item.backUpItemPos.x;
                packet.LastItemPosY = item.backUpItemPos.y;
                packet.LastItemRotate = item.backUpItemRotate;
                packet.LastGridId = item.backUpItemGrid.gridData.gridId;
                Managers.Network.Send(packet);
                selectedItem.backUpItemGrid.RemoveItemFromItemList(selectedItem);
                selectedItem.curItemGrid.AddItemToItemList(selectedItem.itemData.itemPos,selectedItem);
                
                BackUpItem(); //백업한 아이템의 위치를 현재의 위치로 백업
                BackUpGridSlot(); //그리드의 슬롯을 현재의 슬롯으로 백업
                SelectedItem = null;
            }
        }
        else
        {
            //아이템 배치 실패시
            UndoGridSlot(); //인벤그리드의 내용을 되돌림
            UndoItem(); //현재 들고있는 아이템을 원래 위치로 되돌림

            placeOverlapItem = null;
        }
    }

    /*오버랩 스위칭 미사용
    private void OverlapExchange()
    {
        if (checkOverlapItem == null || !isItemSelected) { return; }

        selectedGrid.CleanItemSlot(checkOverlapItem);
        selectedGrid.PlaceItem(selectedItem, checkOverlapItem.curItemPos.x, checkOverlapItem.curItemPos.y);
        
        selectedGrid.GridWeight += SelectedItem.itemDataInfo.item_weight;
        SelectedItem = checkOverlapItem;

        AddBackUpList();
        selectedGrid.GridWeight -= SelectedItem.itemDataInfo.item_weight;

        SelectedItem.GetComponent<Image>().raycastTarget = false;
        checkOverlapItem = null;
        SetSelectedObjectToLastSibling(selectedRect);
    }
    */

    /* 오버랩 스위칭 미사용
    /// <summary>
    /// 아이템과 그리드를 백업리스트에 저장
    /// </summary>
    private void AddBackUpList()
    {
        if(backUpItemList.Contains(selectedItem) == false)
        {
            backUpItemList.Add(selectedItem); //백업시 해당 리스트 안의 아이템 전부 백업
        }
        
        if (backUpGridList.Contains(selectedItem.backUpItemGrid) == false)
        {
            backUpGridList.Add(selectedItem.backUpItemGrid);
        }
    }
    */

    /// <summary>
    /// 아이템 슬롯을 백업함(아이템을 들때 슬롯이 업데이트되기에 백업 필요)
    /// </summary>
    private void BackUpGridSlot()
    {
        selectedItem.curItemGrid.UpdateBackUpSlot();
        selectedItem.curItemGrid.backupWeight = selectedItem.curItemGrid.GridWeight;
        /*오버랩 스위칭 미사용
        //우클릭으로 생성한 아이템은 한번씩 재클릭을 해줘야 적용됨.
        if (backUpItemList.Contains(selectedItem) == false)
        {
            AddBackUpList();
        }

        if (backUpGridList.Count != 0)
        {
            for (int i = 0; i < backUpGridList.Count; i++)
            {
                //해당 그리드의 부모(그리드 모음 오브젝트)의 부모(인벤토리 객체)의 인벤토리 스크립트에서
                //UpdateInvenWeight로 해당 인벤토리의 무게 계산
                backUpGridList[i].UpdateBackUpSlot();
                backUpGridList[i].backupWeight = backUpGridList[i].gridWeight;
            }
            backUpGridList.Clear(); //완료시 리스트 초기화
        }
        */
    }

     
    /// <summary>
    /// 아이템의 상태와 위치를 백업함
    /// </summary>
    private void BackUpItem()
    {
        selectedItem.backUpItemPos = selectedItem.itemData.itemPos; //현재 위치
        selectedItem.backUpItemRotate = selectedItem.itemData.itemRotate; //현재 회전
        selectedItem.backUpItemGrid = selectedItem.curItemGrid; //현재 그리드
        
        /* 오버랩 스위칭 미사용
        //SelectedItem = null;
        if (backUpItemList.Count != 0)
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
        */
    }

    /// <summary>
    /// 아이템 배열을 이전 배열로 되돌림.
    /// </summary>
    private void UndoGridSlot()
    {
        if(selectedItem.curItemGrid == null) { return; }
        if(!isItemSelected) { return; }
        selectedItem.curItemGrid.UndoItemSlot();
        selectedItem.curItemGrid.PrintInvenContents(selectedItem.curItemGrid, selectedItem.curItemGrid.ItemSlot);
        
        /*오버랩 스위칭 미사용
        if (backUpGridList.Count != 0)
        {
            for (int i = 0; i < backUpGridList.Count; i++)
            {
                backUpGridList[i].UndoItemSlot();
                backUpGridList[i].PrintInvenContents(backUpGridList[i], backUpGridList[i].ItemSlot);

                //그리드의 무게를 백업한 무게로 설정하고 인벤토리의 무게를 업데이트
                backUpGridList[i].GridWeight = backUpGridList[i].backupWeight;
            }
            backUpGridList.Clear();
        }*/
    }

    /// <summary>
    /// 아이템을 들었던 위치와 각도로 되돌림 selectedItem 해제되니 주의
    /// </summary>
    private void UndoItem()
    {
        if(!isItemSelected){ return; }

        //현재 아이템 오브젝트의 변수를 백업한 변수의 값으로 롤백
        selectedItem.curItemGrid = selectedItem.backUpItemGrid;
        selectedItem.itemData.itemPos = selectedItem.backUpItemPos;
        selectedItem.itemData.itemRotate = selectedItem.backUpItemRotate;

        //바뀐 변수를 적용. 해당 아이템을 이전상태로 되돌림
        selectedItem.Rotate(selectedItem.itemData.itemRotate);
        selectedItem.curItemGrid.PlaceItem(selectedItem, selectedItem.itemData.itemPos.x, selectedItem.itemData.itemPos.y);
        SelectedItem = null;

        /*오버랩 스위칭 미사용
        //백업 아이템 리스트 안의 내용도 모두 백업
        if (backUpItemList.Count != 0)
        {
            for (int i = 0; i < backUpItemList.Count; i++)
            {
                ItemObject backUpItem = backUpItemList[i];
                Vector2Int backUpPos = backUpItem.backUpItemPos;
                backUpItem.curItemRotate = backUpItem.backUpItemRotate;
                backUpItem.Rotate(backUpItem.backUpItemRotate);
                backUpItem.curItemGrid = backUpItem.backUpItemGrid;
                backUpItem.backUpItemGrid.PlaceItem(backUpItem, backUpPos.x, backUpPos.y);
            }
            backUpItemList.Clear();
        }
        */
    }

    /* 인벤토리 그리드로 옮김
    /// <summary>
    /// 아이템이 생성된 즉시 인벤토리내에 배치 가능한 자리에 배치됨
    /// </summary>
    public void InsertRandomItem(InventoryGrid targetGrid)
    {
        SelectedItemGrid = targetGrid;
        if (!isGridSelected) { return; }

        CreateRandomItem();
        ItemObject itemToInsert = selectedItem;

        SelectedItem = null;
        InsertItem(itemToInsert);
    }*/

    /*
    /// <summary>
    /// 등록된 아이템중 랜덤으로 생성
    /// </summary>
    private void CreateRandomItem()
    {
        //아이템 프리팹을 생성하고 스크립트 로드
        ItemObject invenItem = Instantiate(itemPref).GetComponent<ItemObject>();
        
        //아이템은 캔버스의 자식(UI니까)
        SetSelectedObjectToLastSibling(selectedRect);
        //아이템 리스트 중 하나 지정
        
        int randomId = Random.Range(0, itemList.Count);
        //지정된 아이템 데이터를 아이템 프리팹에 적용
        ItemData randomData = new ItemData();
        randomData.DuplicateItemData(itemList[randomId]);
        invenItem.itemData = randomData;
        invenItem.ItemDataSet(invenItem.itemData);

        SelectedItem = invenItem;
    }
    */

    /* 인벤토리그리드로 옮김
    /// <summary>
    /// insertRandomItem안에서 지정된 아이템을 가능한 자리에 배치
    /// </summary>
    private void InsertItem(ItemObject itemToInsert)
    {
        Vector2Int? posOnGrid = selectedGrid.FindSpaceForObject(itemToInsert);

        if(posOnGrid == null) {
            itemToInsert.RotateRight();
            InsertItem(itemToInsert);
            return;
        }

        selectedGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        SelectedItemGrid = null;
    }
    */

    /// <summary>
    /// 컨트롤러 상에서 삭제 처리
    /// </summary>
    private void DestroySelectedItem()
    {
        selectedItem.curItemGrid.RemoveItemFromItemList(selectedItem);
        selectedItem.DestroyItem();
        SelectedItem = null;
    }

    /// <summary>
    /// 클릭한 오브젝트의 모든 부모에게 LastSibling을 적용함. 항상 지정한 인터렉션한 rect을 가장 앞으로.
    /// </summary>
    public static void SetSelectedObjectToLastSibling(Transform child)
    {
        if (child == null) return;

        if(child.gameObject.name == "InventoryUI") //인벤토리 UI의 자식들 까지만 적용함
        {
            return;
        }
        child.SetAsLastSibling();

        if (child.parent != null)
        {
            SetSelectedObjectToLastSibling(child.parent);
        }
    }
    

    //버튼 전용
    public void RotateBtn()
    {
        RotateItemRight();
    }

    public void invenUIControl()
    {
        isActive = !isActive;

        if (isActive)
        {
            if (playerInput == null)
            {
                playerInput = new PlayerInput();
                playerInput.Player.Disable();
                playerInput.UI.Enable();
                playerInput.UI.MouseMove.performed += OnMousePosInput;
                playerInput.UI.MouseLeftClick.started += OnMouseLeftClickStartInput;
                playerInput.UI.MouseLeftClick.canceled += OnMouseLeftClickCancelInput;
                playerInput.UI.MouseRightClick.performed += OnMouseRightClickInput;
                playerInput.UI.InventoryControl.performed += InvenUIControlInput;
                //playerInput.UI.OverlapChangeAction.performed += OverlapExchangeInput;
            }
            else
            {
                playerInput.Player.Disable();
                playerInput.UI.Enable();
            }
            
        }
        else
        {
            playerInput.UI.Disable();
            playerInput.Player.Enable();
        }

        inventoryUI.SetActive(isActive);
    }
}


