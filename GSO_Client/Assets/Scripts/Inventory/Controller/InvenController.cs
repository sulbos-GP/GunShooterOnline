using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml.Diagram;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController : MonoBehaviour
{
    public static InventoryController invenInstance;

    
    public GameObject inventoryUI;
    public PlayerInventoryUI playerInvenUI;
    public OtherInventoryUI otherInvenUI;

    [Header("수동지정")]
    public Transform deleteUI;
    public Button rotateBtn;

    [Header("디버그용")]
    [SerializeField] private Vector2 mousePosInput; 
    [SerializeField] private Vector2Int updateGridPos; //mousePosInput을 WorldToGridPos메서드로 그리드 내의 좌표로 변환한것. 그리드가 지정되어 있어야함

    //이동 관련
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
    [SerializeField] private InventoryGrid selectedGrid;
    public bool isGridSelected; 

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

                float addedWeight = playerInvenUI.invenWeight + selectedItem.itemData.item_weight;
                playerInvenUI.weightText.text = $"WEIGHT \n{addedWeight} / {playerInvenUI.invenData.limitWeight}";
                if(addedWeight > playerInvenUI.invenData.limitWeight)
                {
                    playerInvenUI.weightText.color = Color.red;
                }
            }
            else
            {
                selectedRect = null;
                rotateBtn.interactable = false;

                playerInvenUI.weightText.text = $"WEIGHT \n{playerInvenUI.invenWeight} / {playerInvenUI.invenData.limitWeight}";
                playerInvenUI.weightText.color = Color.white;
            }
        }
    }
    [SerializeField] private ItemObject selectedItem;
    [SerializeField] private RectTransform selectedRect;
    public bool isItemSelected; //현재 선택된 상태인지

    private ItemObject placeOverlapItem; //아이템을 배치할때 체크될 오버랩 아이템 변수
    private ItemObject checkOverlapItem; //매 프레임마다 체크될 오버랩 아이템 변수

    private bool isPress = false;

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


    //하이라이트 관련 변수
    private InvenHighLight invenHighlight;
    private Vector2Int HighlightPosition; //하이라이트의 위치

    private PlayerInput playerInput; //플레이어의 조작 인풋

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
    }

    private void OnDisable()
    {
        SelectedItem = null;
        SelectedItemGrid = null;
    }


    #region PlayerInput 액션
    private void OnTouchPos(InputAction.CallbackContext context)
    {
        UnityEngine.Vector2 pos = context.ReadValue<UnityEngine.Vector2>();
        mousePosInput = new Vector2(pos.x,pos.y);
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        isPress = true;
        //여기서 아이템 이벤트를 하는게 베스트이나 여기가 실행되고 다음 프레임에 그리드가 할당됨.
        //gridInteract를 대체하여 터치위치에 UI를 얻는 코드가 있을시 수정할것
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        isPress = false;
        if (isItemSelected)
        {
            ItemEvent();
        }
    }


    private void OnMouseRightClickInput(InputAction.CallbackContext context)
    {
        //임시
        //마우스 우클릭시. 선택된 아이템 여부에 따라 새 아이템 생성 혹은 아이템 회전
        RotateItemRight();
    }

    private void InvenUIControlInput(InputAction.CallbackContext context)
    {
        invenUIControl();
    }

    #endregion

    private void Update()
    {
        if (!isActive) //UI가 비활성화 라면 리턴
        {
            return;
        }

        if(isPress  && isGridSelected && !isItemSelected) //클릭한 상태고, 그리드가 설정되었고 , 아이템을 들고 잇는 상태가 아니면
        {
            ItemEvent(); //아이템 이벤트에서 아이템을 들기 실행
        }

        DragObject();

        if (!isGridSelected) //그리드 밖에 있을경우 
        {
            if (isOnDelete&&isItemSelected) //삭제 칸에 있다면 노란색하이라이트
            {
                invenHighlight.Show(true);
                invenHighlight.SetColor(HighlightColor.Yellow);
                InvenHighLight.highlightObj.transform.SetParent(deleteUI);
                InvenHighLight.highlightObj.transform.position = selectedItem.transform.position;
                return;
            }
            //플레이어 슬롯에 배치할때의 처리 추가 예정


            //그외엔 하이라이트 없앰
            invenHighlight.Show(false);
            return;
        }

        // 그리드가 존재할 경우
        if (isGridSelected)
        {
            if (selectedItem != null)
            {
                updateGridPos = WorldToGridPos();
                Color32 highlightColor = selectedGrid.PlaceCheckForHighlightColor(selectedItem, updateGridPos.x, updateGridPos.y, ref checkOverlapItem);
                invenHighlight.SetColor(highlightColor);
            }

            HandleHighlight(); //그리드 안에서만 하이라이트를 다룸
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
        //이미 isGridSelected가 true일때만 호출됨

        if (updateGridPos == null)
        {
            invenHighlight.Show(false);
            return;
        }

        //같은위치에 반복실행을 막음
        if (HighlightPosition == updateGridPos)
        {
            return;
        }

        HighlightPosition = updateGridPos;

        //아이템을 들고 있지 않은 경우
        if (isItemSelected)
        {
            //아이템을 들고 있다면 그 아이템이 위치한 곳에 하이라이팅
            invenHighlight.Show(true);
            invenHighlight.SetSize(selectedItem);
            invenHighlight.SetParent(selectedGrid);
            invenHighlight.SetPositionOnGridByPos(selectedGrid, selectedItem, updateGridPos.x, updateGridPos.y);
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
        if (isItemSelected)
        {
            //휴지통 칸에 있을경우
            if (isOnDelete)
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

            //그리드 밖에 아이템을 배치할경우
            if (!isGridSelected)
            {
                UndoGridSlot();
                UndoItem();
                Debug.Log("그리드 없음");
                return;
            }
            else
            {
                updateGridPos = WorldToGridPos();
                ItemRelease(selectedItem, updateGridPos);
            }
        }
        else
        {
            if (!isGridSelected)
            {
                Debug.Log("그리드가 지정되지 않음");
                return;
            }

            updateGridPos = WorldToGridPos();
            ItemObject clickedItem = selectedGrid.GetItem(updateGridPos.x, updateGridPos.y); 
            if (clickedItem == null) { return; }

            //클릭한 아이템이 숨겨진 경우에는 숨김을 해제하고 아니면 아이템을 듬
            if (clickedItem.isHide == true)
            {
                clickedItem.UnhideItem();
            }
            else
            {
                ItemGet(updateGridPos);
            }
        }
        
    }

    /// <summary>
    /// 아이템을 집는 시도.
    /// </summary>
    private void ItemGet(Vector2Int pos)
    {
        if (!isGridSelected) { return; }
        SelectedItem = selectedGrid.PickUpItem(pos.x, pos.y);

        //아이템이 그리드에 가려지는것을 방지
        SetSelectedObjectToLastSibling(selectedRect);
    }

    /// <summary>
    /// 아이템을 놓는 시도.
    /// </summary>
    private void ItemRelease(ItemObject item, Vector2Int pos)
    {
        bool complete = selectedGrid.PlaceItemCheck(item, pos.x, pos.y, ref placeOverlapItem);

        if (complete)
        {
            HandleItemPlacement(item, pos);
        }
        else
        {
            UndoGridSlot();
            UndoItem();
        }
    }

    /// <summary>
    /// 아이템 배치 성공. 병합 및 아이템 배치 실행
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pos"></param>
    private void HandleItemPlacement(ItemObject item, Vector2Int pos)
    {
        if (placeOverlapItem != null)
        {
            if (CheckAbleToMerge(item))
            {
                MergeItems(item, pos);
            }
            else
            {
                UndoGridSlot();
                UndoItem();
            }
        }
        else
        {
            CompleteItemPlacement(item, pos);
        }
    }

    /// <summary>
    /// 오버랩 아이템 존재할때 머지가 가능한지 체크
    /// </summary>
    private bool CheckAbleToMerge(ItemObject item)
    {
        return selectedItem.itemData.isItemConsumeable &&
               selectedItem.itemData.itemCode == placeOverlapItem.itemData.itemCode &&
               placeOverlapItem.itemData.itemAmount < ItemObject.maxItemMergeAmount &&
               !placeOverlapItem.isHide;
    }

    /// <summary>
    /// 아이템 병합 실시
    /// </summary>
    private void MergeItems(ItemObject item, Vector2Int pos)
    {
        int totalAmount = selectedItem.itemData.itemAmount + placeOverlapItem.itemData.itemAmount;

        if (totalAmount <= ItemObject.maxItemMergeAmount)
        {
            selectedItem.MergeItem(placeOverlapItem, selectedItem.itemData.itemAmount);
            selectedItem.curItemGrid = SelectedItemGrid;
            SendMoveItemPacket(item, pos);
            BackUpGridSlot();
            DestroySelectedItem();
        }
        else
        {
            int needAmount = ItemObject.maxItemMergeAmount - placeOverlapItem.itemData.itemAmount;
            selectedItem.MergeItem(placeOverlapItem, needAmount);
            selectedItem.curItemGrid = SelectedItemGrid;
            SendMoveItemPacket(item, pos);
            UndoGridSlot();
            UndoItem();
        }

        ResetSelection();
    }

    /// <summary>
    /// 아이템 배치 성공
    /// </summary>
    private void CompleteItemPlacement(ItemObject item, Vector2Int pos)
    {
        selectedItem.backUpItemGrid.RemoveItemFromItemList(selectedItem); //이전 
        selectedItem.curItemGrid = SelectedItemGrid;
        selectedItem.curItemGrid.AddItemToItemList(selectedItem.itemData.itemPos, selectedItem);

        SendMoveItemPacket(item, pos); //백업 전에 내보내야 lastItem 변수에 값이 제대로 할당됨

        BackUpItem();
        BackUpGridSlot();

        ResetSelection();
    }

    /// <summary>
    /// C_MoveItem패킷 생성 및 전송
    /// </summary>
    private void SendMoveItemPacket(ItemObject item, Vector2Int pos)
    {
        C_MoveItem packet = new C_MoveItem
        {
            PlayerId = Managers.Object.MyPlayer.Id,
            ItemId = item.itemData.itemId,
            ItemPosX = pos.x,
            ItemPosY = pos.y,
            ItemRotate = item.itemData.itemRotate,
            InventoryId = item.curItemGrid.ownInven.invenData.inventoryId,
            GridId = item.curItemGrid.gridData.gridId,
            LastItemPosX = item.backUpItemPos.x,
            LastItemPosY = item.backUpItemPos.y,
            LastItemRotate = item.backUpItemRotate,
            LastGridId = item.backUpItemGrid.gridData.gridId
        };
        Managers.Network.Send(packet);
    }

    private void ResetSelection()
    {
        SelectedItem = null;
        placeOverlapItem = null;
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

            Debug.Log("touchinput On");
            playerInput = Managers.Object.MyPlayer.playerInput;
            playerInput.Player.Disable();
            playerInput.UI.Enable();
            playerInput.UI.TouchPosition.performed += OnTouchPos;
            playerInput.UI.TouchPress.started += OnTouchStart;
            playerInput.UI.TouchPress.canceled += OnTouchEnd;
            playerInput.UI.MouseRightClick.performed += OnMouseRightClickInput;
            playerInput.UI.InventoryControl.performed += InvenUIControlInput;

        }
        else
        {
            if (InvenHighLight.highlightObj != null)
            {
                invenHighlight.DestroyHighlighter();
            }

            Debug.Log("touchinput Off");
            playerInput.UI.TouchPosition.performed -= OnTouchPos;
            playerInput.UI.TouchPress.started -= OnTouchStart;
            playerInput.UI.TouchPress.canceled -= OnTouchEnd;
            playerInput.UI.MouseRightClick.performed -= OnMouseRightClickInput;
            playerInput.UI.InventoryControl.performed -= InvenUIControlInput;
            playerInput.UI.Disable();
            playerInput.Player.Enable();
        }

        inventoryUI.SetActive(isActive);
    }

    
}


