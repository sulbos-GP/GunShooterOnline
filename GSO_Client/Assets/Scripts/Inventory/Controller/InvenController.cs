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

    [Header("��������")]
    public Transform deleteUI;
    public Button rotateBtn;

    [Header("����׿�")]
    [SerializeField] private Vector2 mousePosInput; 
    [SerializeField] private Vector2Int updateGridPos; //mousePosInput�� WorldToGridPos�޼���� �׸��� ���� ��ǥ�� ��ȯ�Ѱ�. �׸��尡 �����Ǿ� �־����

    //�̵� ����
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
    public bool isItemSelected; //���� ���õ� ��������

    private ItemObject placeOverlapItem; //�������� ��ġ�Ҷ� üũ�� ������ ������ ����
    private ItemObject checkOverlapItem; //�� �����Ӹ��� üũ�� ������ ������ ����

    private bool isPress = false;

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


    //���̶���Ʈ ���� ����
    private InvenHighLight invenHighlight;
    private Vector2Int HighlightPosition; //���̶���Ʈ�� ��ġ

    private PlayerInput playerInput; //�÷��̾��� ���� ��ǲ

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


    #region PlayerInput �׼�
    private void OnTouchPos(InputAction.CallbackContext context)
    {
        UnityEngine.Vector2 pos = context.ReadValue<UnityEngine.Vector2>();
        mousePosInput = new Vector2(pos.x,pos.y);
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        isPress = true;
        //���⼭ ������ �̺�Ʈ�� �ϴ°� ����Ʈ�̳� ���Ⱑ ����ǰ� ���� �����ӿ� �׸��尡 �Ҵ��.
        //gridInteract�� ��ü�Ͽ� ��ġ��ġ�� UI�� ��� �ڵ尡 ������ �����Ұ�
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
        //�ӽ�
        //���콺 ��Ŭ����. ���õ� ������ ���ο� ���� �� ������ ���� Ȥ�� ������ ȸ��
        RotateItemRight();
    }

    private void InvenUIControlInput(InputAction.CallbackContext context)
    {
        invenUIControl();
    }

    #endregion

    private void Update()
    {
        if (!isActive) //UI�� ��Ȱ��ȭ ��� ����
        {
            return;
        }

        if(isPress  && isGridSelected && !isItemSelected) //Ŭ���� ���°�, �׸��尡 �����Ǿ��� , �������� ��� �մ� ���°� �ƴϸ�
        {
            ItemEvent(); //������ �̺�Ʈ���� �������� ��� ����
        }

        DragObject();

        if (!isGridSelected) //�׸��� �ۿ� ������� 
        {
            if (isOnDelete&&isItemSelected) //���� ĭ�� �ִٸ� ��������̶���Ʈ
            {
                invenHighlight.Show(true);
                invenHighlight.SetColor(HighlightColor.Yellow);
                InvenHighLight.highlightObj.transform.SetParent(deleteUI);
                InvenHighLight.highlightObj.transform.position = selectedItem.transform.position;
                return;
            }
            //�÷��̾� ���Կ� ��ġ�Ҷ��� ó�� �߰� ����


            //�׿ܿ� ���̶���Ʈ ����
            invenHighlight.Show(false);
            return;
        }

        // �׸��尡 ������ ���
        if (isGridSelected)
        {
            if (selectedItem != null)
            {
                updateGridPos = WorldToGridPos();
                Color32 highlightColor = selectedGrid.PlaceCheckForHighlightColor(selectedItem, updateGridPos.x, updateGridPos.y, ref checkOverlapItem);
                invenHighlight.SetColor(highlightColor);
            }

            HandleHighlight(); //�׸��� �ȿ����� ���̶���Ʈ�� �ٷ�
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
        //�̹� isGridSelected�� true�϶��� ȣ���

        if (updateGridPos == null)
        {
            invenHighlight.Show(false);
            return;
        }

        //������ġ�� �ݺ������� ����
        if (HighlightPosition == updateGridPos)
        {
            return;
        }

        HighlightPosition = updateGridPos;

        //�������� ��� ���� ���� ���
        if (isItemSelected)
        {
            //�������� ��� �ִٸ� �� �������� ��ġ�� ���� ���̶�����
            invenHighlight.Show(true);
            invenHighlight.SetSize(selectedItem);
            invenHighlight.SetParent(selectedGrid);
            invenHighlight.SetPositionOnGridByPos(selectedGrid, selectedItem, updateGridPos.x, updateGridPos.y);
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
        if (isItemSelected)
        {
            //������ ĭ�� �������
            if (isOnDelete)
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

            //�׸��� �ۿ� �������� ��ġ�Ұ��
            if (!isGridSelected)
            {
                UndoGridSlot();
                UndoItem();
                Debug.Log("�׸��� ����");
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
                Debug.Log("�׸��尡 �������� ����");
                return;
            }

            updateGridPos = WorldToGridPos();
            ItemObject clickedItem = selectedGrid.GetItem(updateGridPos.x, updateGridPos.y); 
            if (clickedItem == null) { return; }

            //Ŭ���� �������� ������ ��쿡�� ������ �����ϰ� �ƴϸ� �������� ��
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
    /// �������� ���� �õ�.
    /// </summary>
    private void ItemGet(Vector2Int pos)
    {
        if (!isGridSelected) { return; }
        SelectedItem = selectedGrid.PickUpItem(pos.x, pos.y);

        //�������� �׸��忡 �������°��� ����
        SetSelectedObjectToLastSibling(selectedRect);
    }

    /// <summary>
    /// �������� ���� �õ�.
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
    /// ������ ��ġ ����. ���� �� ������ ��ġ ����
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
    /// ������ ������ �����Ҷ� ������ �������� üũ
    /// </summary>
    private bool CheckAbleToMerge(ItemObject item)
    {
        return selectedItem.itemData.isItemConsumeable &&
               selectedItem.itemData.itemCode == placeOverlapItem.itemData.itemCode &&
               placeOverlapItem.itemData.itemAmount < ItemObject.maxItemMergeAmount &&
               !placeOverlapItem.isHide;
    }

    /// <summary>
    /// ������ ���� �ǽ�
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
    /// ������ ��ġ ����
    /// </summary>
    private void CompleteItemPlacement(ItemObject item, Vector2Int pos)
    {
        selectedItem.backUpItemGrid.RemoveItemFromItemList(selectedItem); //���� 
        selectedItem.curItemGrid = SelectedItemGrid;
        selectedItem.curItemGrid.AddItemToItemList(selectedItem.itemData.itemPos, selectedItem);

        SendMoveItemPacket(item, pos); //��� ���� �������� lastItem ������ ���� ����� �Ҵ��

        BackUpItem();
        BackUpGridSlot();

        ResetSelection();
    }

    /// <summary>
    /// C_MoveItem��Ŷ ���� �� ����
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


