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
     * �� �ڵ�� �κ��丮 �Ŵ����� �����Ǹ� �÷��̾��� ���ۿ� ���� �Լ��� ȣ���մϴ�.
     * 
     * 1. PlayerInput���� ���콺�� ��ġ, �¿�Ŭ�� �̺�Ʈ�� �߻��մϴ�.
     *    ��Ŭ���� ItemGetOrRelease�Լ��� ���� selectedItem������ ���� ���ο� ���� �������� 
     *     ���ų� ��ġ�մϴ�
     *    ��Ŭ����(�ӽ�) selectedItem�� �����ϴ��� ���ο� ���� ȸ���ϰų� ���ο� �������� ����
     *     �մϴ�
     * 
     * 2. ������Ʈ���� ȣ��Ǵ� �Լ�
     *    WorldToGridPos ���� ���콺�� ��ġ�� �׸����� ��ǥ�� ��ȯ�մϴ�.
     *    DragObject selectedItem�� �ִٸ� �ش� �������� ��ġ = ���콺�� ��ġ
     *    HandleHighlight�� selectedItem�� �������� ��ġ�� �����ۿ� ���콺�� ������ ��� ������
     *     �� ���̶���Ʈ�� �ְ� selectedItem�� �ִٸ� �ش� ��ġ�� �������� ��ġ�������� ���̶���Ʈ
     *     �� ǥ���մϴ�
     * 
     * 3. CreateRandomItem : ���ο� �������� �����Ͽ� selectedItem���� �����մϴ�
     *    InsertRandomItem : �ش� �׸��忡 �ٷ� ������ �������� ����ֽ��ϴ�.
     *    SetSelectedObjectToLastSibling : ���õ� �������� �ٸ� �����ۿ� �������� �ʵ��� �ϱ�����
     *    ���� �θ� ���� �ڽ� ��ü�� �� �Ʒ��� �̵��ϸ� ��ü�� �θ�, ��ü�� �θ��� �θ� �� ���̻�
     *    �θ� ��ü�� ���������� ��ͷ� �ݺ��մϴ�.
     */

    public static InventoryController invenInstance;
    private PlayerInput playerInput; //�÷��̾��� ���� ��ǲ
    public GameObject inventoryUI;
    public PlayerInventoryUI playerInvenUI;
    public OtherInventoryUI otherInvenUI;
    //public ItemDB itemdb;
    //public List<ItemData> itemList;

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
    private InvenHighLight invenHighlight; //���̶���Ʈ ��ü�� �ִ� ����
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

    //������� ����(������ ������ ��ü�� �������� �ʿ� ����)
    //public List<ItemObject> backUpItemList = new List<ItemObject>(); //����� �����۵��� ����Ʈ
    //public List<InventoryGrid> backUpGridList = new List<InventoryGrid>(); //����� �׸������ ����Ʈ


    /*
    //�κ��丮 �巡�� ����
    [SerializeField] private InvenData selectedInven; //�巡���� �κ��丮
    public bool isInvenSelected; //�巡���� �κ��� ���õ�
    public bool isDragging;
    public RectTransform draggedInvenRect;
    public InvenData SelectedInven
    {
        get => selectedInven;
        set
        {
            selectedInven = value;
            isInvenSelected = selectedInven != null;
            //draggedInvenRect ������ ������ dragOffset�ʱ�ȭ�� ���⿡ ������
            //���콺�� ������ ������ ��� ���콺�����Ͱ� �κ��丮�� ��� Ǯ������
            //�巡�� �̺�Ʈ�� �Ͼ���� Rect ������ �����Ұ�
        }
    }
    private Vector2 dragOffset; //�巡���ϴ� �κ��丮 ��Ʈ�� �߽ɿ��� ���콺������ �Ÿ�
    */

    //��Ÿ ����

    

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

    #region PlayerInput �׼�
    private void OnMousePosInput(InputAction.CallbackContext context)
    {
        //���콺�� ��ġ �ǽð� ������Ʈ
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
        //���콺 ��Ŭ����. ���õ� ������ ���ο� ���� �� ������ ���� Ȥ�� ������ ȸ��
        RightClickEvent();
    }
    private void InvenUIControlInput(InputAction.CallbackContext context)
    {
        invenUIControl();
    }
    

    /*
    private void OverlapExchangeInput(InputAction.CallbackContext context)
    {
        Debug.Log("������ �۵�");
        OverlapExchange();
    }*/

    private void LeftClickEvent()
    {
        /*
         * �÷��� �κ��丮 ������
        if (!isGridSelected)
        {
            DragEvent();
            return;
        }*/

        //�׸��� ���� �ִٸ� ������ �̺�Ʈ
        ItemEvent();
    }

    /*
    /// <summary>
    /// ���ٴϴ� �κ��丮�� ��� �巡���ϴ� �ڵ� (�÷��� �κ��丮 ���ŷ� �Ⱦ�)
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
            // Ŭ���� ��ġ�� �������� ������ ���
            Vector2 clickPosition = mousePosInput;
            Vector2 inventoryCenter = selectedInven.transform.position;
            dragOffset = clickPosition - inventoryCenter;

            SetSelectedObjectToLastSibling(draggedInvenRect);
        }
    }
    */

    private void RightClickEvent()
    {
        /*//�������� ����ִٸ� ȸ��, ���ٸ� �ӽ÷� ���ο� ������ ����
        if (!isItemSelected)
        {
            CreateRandomItem();
        }
        else
        {
            //��ǻ�ͷ� ���۽� ���
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

        // �׸��尡 ������ ���
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

        /* �÷��� �κ��丮�� ������� ����
        if (isDragging)
        {
            if(draggedInvenRect.gameObject.activeSelf == false)
            {
                isDragging = false;
                draggedInvenRect = null;
                dragOffset = Vector2.zero;
                return;
            }

            //�� �κ��丮�� ��� Ŭ���� ��ġ�� ���콺�� �߽����� 
            Vector2 newPosition = mousePosInput - dragOffset;
            draggedInvenRect.position = newPosition;
        }
        */
    }

    /// <summary>
    /// �����ۿ� ���콺�� ������� ���̶���Ʈ ȿ��
    /// </summary>
    private void HandleHighlight()
    {
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

    /*
    /// <summary>
    /// ��Ʈ�ѷ����� ������ ��ȸ�� ��� (��� ����)
    /// </summary>
    public void RotateItemLeft()
    {
        if (!isItemSelected) { return; }
        selectedItem.RotateLeft();
    }*/

    /// <summary>
    /// ��Ŭ���� �������� ���ų� ���� ����
    /// </summary>
    private void ItemEvent()
    {
        Vector2Int tileGridPosition = updateGridPos; //���콺�� ��ġ�� �ִ� �׸��� ��ǥ �Ҵ�
        if(tileGridPosition == null) { return; }

        if (!isItemSelected)
        {
            if (!isGridSelected)
            {
                return;
            }

            ItemObject clickedItem = selectedGrid.GetItem(tileGridPosition.x, tileGridPosition.y); 
            if (clickedItem == null) { return; }

            //Ŭ���� �������� ������ ��쿡�� ������ �����ϰ� �ƴϸ� �������� ��
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
        if(isOnDelete)
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
                    !placeOverlapItem.ishide)
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

    /*������ ����Ī �̻��
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

    /* ������ ����Ī �̻��
    /// <summary>
    /// �����۰� �׸��带 �������Ʈ�� ����
    /// </summary>
    private void AddBackUpList()
    {
        if(backUpItemList.Contains(selectedItem) == false)
        {
            backUpItemList.Add(selectedItem); //����� �ش� ����Ʈ ���� ������ ���� ���
        }
        
        if (backUpGridList.Contains(selectedItem.backUpItemGrid) == false)
        {
            backUpGridList.Add(selectedItem.backUpItemGrid);
        }
    }
    */

    /// <summary>
    /// ������ ������ �����(�������� �鶧 ������ ������Ʈ�Ǳ⿡ ��� �ʿ�)
    /// </summary>
    private void BackUpGridSlot()
    {
        selectedItem.curItemGrid.UpdateBackUpSlot();
        selectedItem.curItemGrid.backupWeight = selectedItem.curItemGrid.GridWeight;
        /*������ ����Ī �̻��
        //��Ŭ������ ������ �������� �ѹ��� ��Ŭ���� ����� �����.
        if (backUpItemList.Contains(selectedItem) == false)
        {
            AddBackUpList();
        }

        if (backUpGridList.Count != 0)
        {
            for (int i = 0; i < backUpGridList.Count; i++)
            {
                //�ش� �׸����� �θ�(�׸��� ���� ������Ʈ)�� �θ�(�κ��丮 ��ü)�� �κ��丮 ��ũ��Ʈ����
                //UpdateInvenWeight�� �ش� �κ��丮�� ���� ���
                backUpGridList[i].UpdateBackUpSlot();
                backUpGridList[i].backupWeight = backUpGridList[i].gridWeight;
            }
            backUpGridList.Clear(); //�Ϸ�� ����Ʈ �ʱ�ȭ
        }
        */
    }

     
    /// <summary>
    /// �������� ���¿� ��ġ�� �����
    /// </summary>
    private void BackUpItem()
    {
        selectedItem.backUpItemPos = selectedItem.itemData.itemPos; //���� ��ġ
        selectedItem.backUpItemRotate = selectedItem.itemData.itemRotate; //���� ȸ��
        selectedItem.backUpItemGrid = selectedItem.curItemGrid; //���� �׸���
        
        /* ������ ����Ī �̻��
        //SelectedItem = null;
        if (backUpItemList.Count != 0)
        {
            //��� ����Ʈ ���� ��� ������ ���� ��� ������Ʈ
            for (int i = 0; i < backUpItemList.Count; i++)
            {
                backUpItemList[i].backUpItemRotate = backUpItemList[i].curItemRotate;
                backUpItemList[i].backUpItemPos = backUpItemList[i].curItemPos;
                backUpItemList[i].backUpItemGrid = backUpItemList[i].curItemGrid;
            }
            backUpItemList.Clear(); //�Ϸ�� ����Ʈ �ʱ�ȭ
        }
        */
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
        
        /*������ ����Ī �̻��
        if (backUpGridList.Count != 0)
        {
            for (int i = 0; i < backUpGridList.Count; i++)
            {
                backUpGridList[i].UndoItemSlot();
                backUpGridList[i].PrintInvenContents(backUpGridList[i], backUpGridList[i].ItemSlot);

                //�׸����� ���Ը� ����� ���Է� �����ϰ� �κ��丮�� ���Ը� ������Ʈ
                backUpGridList[i].GridWeight = backUpGridList[i].backupWeight;
            }
            backUpGridList.Clear();
        }*/
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

        /*������ ����Ī �̻��
        //��� ������ ����Ʈ ���� ���뵵 ��� ���
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

    /* �κ��丮 �׸���� �ű�
    /// <summary>
    /// �������� ������ ��� �κ��丮���� ��ġ ������ �ڸ��� ��ġ��
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
    /// ��ϵ� �������� �������� ����
    /// </summary>
    private void CreateRandomItem()
    {
        //������ �������� �����ϰ� ��ũ��Ʈ �ε�
        ItemObject invenItem = Instantiate(itemPref).GetComponent<ItemObject>();
        
        //�������� ĵ������ �ڽ�(UI�ϱ�)
        SetSelectedObjectToLastSibling(selectedRect);
        //������ ����Ʈ �� �ϳ� ����
        
        int randomId = Random.Range(0, itemList.Count);
        //������ ������ �����͸� ������ �����տ� ����
        ItemData randomData = new ItemData();
        randomData.DuplicateItemData(itemList[randomId]);
        invenItem.itemData = randomData;
        invenItem.ItemDataSet(invenItem.itemData);

        SelectedItem = invenItem;
    }
    */

    /* �κ��丮�׸���� �ű�
    /// <summary>
    /// insertRandomItem�ȿ��� ������ �������� ������ �ڸ��� ��ġ
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


