using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml.Diagram;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController : MonoBehaviour
{
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
    private InvenHighLight invenHighlight;
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


        invenHighlight = GetComponent<InvenHighLight>();
        //Managers.Network.ConnectToGame();
    }

    private void OnDisable()
    {
        SelectedItem = null;
        SelectedItemGrid = null;
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
   
    private void LeftClickEvent()
    {
        

        //그리드 내에 있다면 아이템 이벤트
        ItemEvent();
    }

    private void RightClickEvent()
    {
        
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
                invenHighlight.Show(true);
                invenHighlight.SetColor(HighlightColor.Yellow);
                InvenHighLight.highlightObj.transform.SetParent(deleteUI);
                InvenHighLight.highlightObj.transform.position = selectedItem.transform.position;
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
            if (clickedItem.isHide == true)
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
                    !placeOverlapItem.isHide)
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

    /// <summary>
    /// 아이템 슬롯을 백업함(아이템을 들때 슬롯이 업데이트되기에 백업 필요)
    /// </summary>
    private void BackUpGridSlot()
    {
        selectedItem.curItemGrid.UpdateBackUpSlot();
        selectedItem.curItemGrid.backupWeight = selectedItem.curItemGrid.GridWeight;
        
    }

     
    /// <summary>
    /// 아이템의 상태와 위치를 백업함
    /// </summary>
    private void BackUpItem()
    {
        selectedItem.backUpItemPos = selectedItem.itemData.itemPos; //현재 위치
        selectedItem.backUpItemRotate = selectedItem.itemData.itemRotate; //현재 회전
        selectedItem.backUpItemGrid = selectedItem.curItemGrid; //현재 그리드
        
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

    }


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
            if(InvenHighLight.highlightObj == null)
            {
                invenHighlight.InstantHighlighter();
            }

            if (playerInput == null)
            {
                playerInput = Managers.Object.MyPlayer.playerInput;
                playerInput.Player.Disable();
                playerInput.UI.Enable();
                playerInput.UI.MouseMove.performed += OnMousePosInput;
                playerInput.UI.MouseLeftClick.started += OnMouseLeftClickStartInput;
                playerInput.UI.MouseLeftClick.canceled += OnMouseLeftClickCancelInput;
                playerInput.UI.MouseRightClick.performed += OnMouseRightClickInput;
                playerInput.UI.InventoryControl.performed += InvenUIControlInput;
            }
            else
            {
                playerInput.Player.Disable();
                playerInput.UI.Enable();
            }
            
        }
        else
        {
            if (InvenHighLight.highlightObj != null)
            {
                invenHighlight.DestroyHighlighter();
            }
            playerInput.UI.Disable();
            playerInput.Player.Enable();
        }

        inventoryUI.SetActive(isActive);
    }
}


