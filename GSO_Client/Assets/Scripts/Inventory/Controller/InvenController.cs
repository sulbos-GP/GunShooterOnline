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
    private PlayerInput playerInput; //�÷��̾��� ���� ��ǲ
    public GameObject inventoryUI;
    public PlayerInventoryUI playerInvenUI;
    public OtherInventoryUI otherInvenUI;
    public Camera uiCamera; // UI ī�޶� (Canvas�� Render Mode�� Screen Space - Camera�� ��)
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    [Header("��������")]
    public Transform deleteUI;
    public Button rotateBtn;

    //��Ŭ�� ������ ���� ����
    [SerializeField] private GameObject itemPref; //������ �������� ������(�ӽ�)

    [Header("����׿�")]
    [SerializeField] private Vector2 mousePosInput; //���콺�� ���� ��ġ
    [SerializeField] private Vector2Int updateGridPos; //�׸������ ��ǥ ��ġ

    //�׸����� �Է� ��ȭ�� ���� �Ʒ� ���� ������Ʈ �� ���̶���Ʈ ��ü�� �θ�ü�� ����
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
    [SerializeField] private InventoryGrid selectedGrid; //���� ���콺�� ��ġ�� �׸���
    public bool isGridSelected; //�׸��尡 ���õǾ�����

    //���õ� �������� ��ȭ�� ���� �Ʒ����� ������Ʈ
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
    [SerializeField] private ItemObject selectedItem; //���� ���õ� ������
    [SerializeField] private RectTransform selectedRect; //���õ� �������� Rect
    public bool isItemSelected; //���� ���õ� ��������

    private ItemObject placeOverlapItem; //�������� ��ġ�Ҷ� üũ�� ������ ������ ����
    [SerializeField] private ItemObject checkOverlapItem; //�� �����Ӹ��� üũ�� ������ ������ ����

    //���̶���Ʈ ���� ����
    private InvenHighLight invenHighlight;
    private Vector2Int HighlightPosition; //���̶���Ʈ�� ��ġ

    //���� ����
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

    //UI��Ƽ�� ����
    public bool isActive = false;
    public bool isPress = false;

    

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

        graphicRaycaster = GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        invenHighlight = GetComponent<InvenHighLight>();
        //Managers.Network.ConnectToGame();
    }

    private void OnDisable()
    {
        SelectedItem = null;
        SelectedItemGrid = null;
    }


    #region PlayerInput �׼�
    private void OnTouchPos(InputAction.CallbackContext context)
    {
        UnityEngine.Vector2 pos = context.ReadValue<UnityEngine.Vector2>();
        mousePosInput = new Vector2(pos.x,pos.y);
        Debug.Log(mousePosInput);
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        isPress = true;
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
        //���콺 ��Ŭ����. ���õ� ������ ���ο� ���� �� ������ ���� Ȥ�� ������ ȸ��
        RightClickEvent();
    }

    private void InvenUIControlInput(InputAction.CallbackContext context)
    {
        invenUIControl();
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

        if(isPress  && isGridSelected && !isItemSelected)
        {
            ItemEvent();
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

        // �׸��尡 ������ ���
        if (isGridSelected)
        {
            if (selectedItem != null)
            {
                Color32 highlightColor = selectedGrid.PlaceCheckForHighlightColor(selectedItem, updateGridPos.x, updateGridPos.y, ref checkOverlapItem);
                invenHighlight.SetColor(highlightColor);
                
            }
            HandleHighlight();
        }
    }

    /// <summary>
    /// ���콺�� ��ġ�� Grid���� Ÿ�� ��ġ�� ��ȯ
    /// </summary>
    private Vector2Int WorldToGridPos()
    {
        Vector2 position = mousePosInput;
        //�������� ����ִٸ�
        if (selectedItem != null)
        {
            //�������� ���콺 �߾ӿ� ������ ����
            position.X -= (selectedItem.Width - 1) * InventoryGrid.WidthOfTile / 2;
            position.Y += (selectedItem.Height - 1) * InventoryGrid.HeightOfTile / 2;
        }

        return selectedGrid.MouseToGridPosition(position);
    }

    /// <summary>
    /// �������� ��� �ְų� �巡�� ���̶�� �ش� UI�� ��Ʈ�� ��ġ�� ���콺�� ��ġ�� ��� ������Ʈ
    /// </summary>
    private void DragObject()
    {
        if (isItemSelected)
        {
            selectedRect.position =  new UnityEngine.Vector2(mousePosInput.X, mousePosInput.Y);
        }

    }

    /// <summary>
    /// �����ۿ� ���콺�� ������� ���̶���Ʈ ȿ��
    /// </summary>
    private void HandleHighlight()
    {
        updateGridPos = WorldToGridPos();
        Vector2Int positionOnGrid = updateGridPos;

        //�׸��� ���� �������� �ʴ� �ٸ� ���̶����� ����
        if (positionOnGrid == null)
        {
            invenHighlight.Show(false);
            return;
        }

        //������ġ�� �ݺ������� ����
        if (HighlightPosition == positionOnGrid)
        {
            return;
        }

        HighlightPosition = positionOnGrid;

        //�������� ��� ���� ���� ���
        if (!isItemSelected)
        {
            invenHighlight.SetColor(HighlightColor.Gray);

            //���콺�� ��ġ�� �����ִ� �������� �ִ��� üũ
            ItemObject itemToHighlight = selectedGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            
            //�ش� �������� �����ϸ� �� �������� ũ��� ��ġ�� �°� ���̶���Ʈ
            if (itemToHighlight != null)
            {
                invenHighlight.Show(true);
                invenHighlight.SetSize(itemToHighlight);
                invenHighlight.SetParent(selectedGrid);
                invenHighlight.SetPositionOnGrid(selectedGrid, itemToHighlight);
            }
            else
            {
                //������ ���̶���Ʈ ����
                invenHighlight.Show(false);
            }
        }
        else
        {
            //�������� ��� �ִٸ� �� �������� ��ġ�� ���� ���̶�����
            invenHighlight.Show(true);
            invenHighlight.SetSize(selectedItem);
            invenHighlight.SetParent(selectedGrid);
            invenHighlight.SetPositionOnGridByPos(selectedGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    /// <summary>
    /// ��Ʈ�ѷ����� ������ ��ȸ�� ���
    /// </summary>
    public void RotateItemRight()
    {
        if(!isItemSelected) { return; }
        selectedItem.RotateRight();
    }

    /// <summary>
    /// ��Ŭ���� �������� ���ų� ���� ����
    /// </summary>
    private void ItemEvent()
    {
        if (isItemSelected && isOnDelete)
        {
            //�� �������� ���� ��ġ�� �÷��̾��� �κ��丮���� ��쿡�� ������ ����.
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
            Debug.Log("�׸��� ����");
            return;
        }
        updateGridPos = WorldToGridPos();
        Debug.Log("�׸��� ���� ����");
        Vector2Int tileGridPosition = updateGridPos; //���콺�� ��ġ�� �ִ� �׸��� ��ǥ �Ҵ�

        if(tileGridPosition == null) { Debug.Log("tilegridposition = null"); return; }

        if (!isItemSelected)
        {
            ItemObject clickedItem = selectedGrid.GetItem(tileGridPosition.x, tileGridPosition.y); 
            if (clickedItem == null) { return; }

            //Ŭ���� �������� ������ ��쿡�� ������ �����ϰ� �ƴϸ� �������� ��
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
            //�̹� �� �������� �ִ°�� �ش���ġ�� �������� ��ġ
            ItemRelease(selectedItem, tileGridPosition);
        }
    }

    /// <summary>
    /// �������� ���� �õ�.
    /// </summary>
    private void ItemGet(Vector2Int pos)
    {
        SelectedItem = selectedGrid.PickUpItem(pos.x, pos.y);
        if (!isGridSelected) { return; }
        
        //AddBackUpList(); ������ ��ü �Ⱦ�
        

        //�������� �׸��忡 �������°��� ����
        SetSelectedObjectToLastSibling(selectedRect);
    }

    /// <summary>
    /// �������� ���� �õ�.
    /// </summary>
    private void ItemRelease(ItemObject item, Vector2Int pos)
    {
        if (!isGridSelected)
        {
            //������ �׸��尡 �ƴ� �߸��� ��ġ�� ������� �ǵ���
            UndoGridSlot();
            UndoItem();
            return;
        }

        bool complete = selectedGrid.PlaceItemCheck(item, pos.x, pos.y, ref placeOverlapItem);

        if (complete)
        {
            //SelectedItem.GetComponent<Image>().raycastTarget = true;
            SelectedItem.curItemGrid = selectedGrid;
            //��ġ�� �������� �ִٸ� SelectedItem���� ����
            if (placeOverlapItem != null)
            {
                //�������� ������ �������� �Ҹ�ǰ�̰� ��ġ�� �����۰� ������ ����
                //�������� ���� 64�� ������ ��� ������ ��ġ��
                if(selectedItem.itemData.isItemConsumeable &&
                    selectedItem.itemData.itemCode == placeOverlapItem.itemData.itemCode&&
                    placeOverlapItem.itemData.itemAmount < ItemObject.maxItemMergeAmount&&
                    !placeOverlapItem.isHide)
                {
                    //�� �������� ���� 64�ų� ������ overlap�������� ������ ������ �������� �縸ŭ ����
                    //���� ������ �������� ����
                    if((selectedItem.itemData.itemAmount + placeOverlapItem.itemData.itemAmount) <= ItemObject.maxItemMergeAmount)
                    {
                        //������ ������ ���� ���԰� �����Ǳ�� �ϸ� �ڵ� �߰� �Ұ�
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

                //�����ϴ� ��찡 �ƴ϶�� ����. �����۰� ��̸� ���� ��ġ��
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
                
                BackUpItem(); //����� �������� ��ġ�� ������ ��ġ�� ���
                BackUpGridSlot(); //�׸����� ������ ������ �������� ���
                SelectedItem = null;
            }
        }
        else
        {
            //������ ��ġ ���н�
            UndoGridSlot(); //�κ��׸����� ������ �ǵ���
            UndoItem(); //���� ����ִ� �������� ���� ��ġ�� �ǵ���

            placeOverlapItem = null;
        }
    }

    /// <summary>
    /// ������ ������ �����(�������� �鶧 ������ ������Ʈ�Ǳ⿡ ��� �ʿ�)
    /// </summary>
    private void BackUpGridSlot()
    {
        selectedItem.curItemGrid.UpdateBackUpSlot();
        selectedItem.curItemGrid.backupWeight = selectedItem.curItemGrid.GridWeight;
        
    }

     
    /// <summary>
    /// �������� ���¿� ��ġ�� �����
    /// </summary>
    private void BackUpItem()
    {
        selectedItem.backUpItemPos = selectedItem.itemData.itemPos; //���� ��ġ
        selectedItem.backUpItemRotate = selectedItem.itemData.itemRotate; //���� ȸ��
        selectedItem.backUpItemGrid = selectedItem.curItemGrid; //���� �׸���
        
    }

    /// <summary>
    /// ������ �迭�� ���� �迭�� �ǵ���.
    /// </summary>
    private void UndoGridSlot()
    {
        if(selectedItem.curItemGrid == null) { return; }
        if(!isItemSelected) { return; }
        selectedItem.curItemGrid.UndoItemSlot();
        selectedItem.curItemGrid.PrintInvenContents(selectedItem.curItemGrid, selectedItem.curItemGrid.ItemSlot);
        
    }

    /// <summary>
    /// �������� ����� ��ġ�� ������ �ǵ��� selectedItem �����Ǵ� ����
    /// </summary>
    private void UndoItem()
    {
        if(!isItemSelected){ return; }

        //���� ������ ������Ʈ�� ������ ����� ������ ������ �ѹ�
        selectedItem.curItemGrid = selectedItem.backUpItemGrid;
        selectedItem.itemData.itemPos = selectedItem.backUpItemPos;
        selectedItem.itemData.itemRotate = selectedItem.backUpItemRotate;

        //�ٲ� ������ ����. �ش� �������� �������·� �ǵ���
        selectedItem.Rotate(selectedItem.itemData.itemRotate);
        selectedItem.curItemGrid.PlaceItem(selectedItem, selectedItem.itemData.itemPos.x, selectedItem.itemData.itemPos.y);
        SelectedItem = null;

    }


    /// <summary>
    /// ��Ʈ�ѷ� �󿡼� ���� ó��
    /// </summary>
    private void DestroySelectedItem()
    {
        selectedItem.curItemGrid.RemoveItemFromItemList(selectedItem);
        selectedItem.DestroyItem();
        SelectedItem = null;
    }

    /// <summary>
    /// Ŭ���� ������Ʈ�� ��� �θ𿡰� LastSibling�� ������. �׻� ������ ���ͷ����� rect�� ���� ������.
    /// </summary>
    public static void SetSelectedObjectToLastSibling(Transform child)
    {
        if (child == null) return;

        if(child.gameObject.name == "InventoryUI") //�κ��丮 UI�� �ڽĵ� ������ ������
        {
            return;
        }
        child.SetAsLastSibling();

        if (child.parent != null)
        {
            SetSelectedObjectToLastSibling(child.parent);
        }
    }
    

    //��ư ����
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


