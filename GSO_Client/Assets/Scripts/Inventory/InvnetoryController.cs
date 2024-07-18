using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryController : MonoBehaviour
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
     *    GetTileGridPosition ���� ���콺�� ��ġ�� �׸����� ��ǥ�� ��ȯ�մϴ�.
     *    ItemSpriteDrag selectedItem�� �ִٸ� �ش� �������� ��ġ = ���콺�� ��ġ
     *    HandleHighlight�� selectedItem�� �������� ��ġ�� �����ۿ� ���콺�� ������ ��� ������
     *     �� ���̶���Ʈ�� �ְ� selectedItem�� �ִٸ� �ش� ��ġ�� �������� ��ġ�������� ���̶���Ʈ
     *     �� ǥ���մϴ�
     * 
     * 3. CreateRandomItem : ���ο� �������� �����Ͽ� selectedItem���� �����մϴ�
     *    InsertRandomItem : �ش� �׸��忡 �ٷ� ������ �������� ����ֽ��ϴ�.
     *    SetAllParentsAsLastSibling : ���õ� �������� �ٸ� �����ۿ� �������� �ʵ��� �ϱ�����
     *    ���� �θ� ���� �ڽ� ��ü�� �� �Ʒ��� �̵��ϸ� ��ü�� �θ�, ��ü�� �θ��� �θ� �� ���̻�
     *    �θ� ��ü�� ���������� ��ͷ� �ݺ��մϴ�.
     *     
     */

    public static InventoryController invenInstance;
    public int playerId = 1; //�ӽ� �ο�
    
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
    [SerializeField] private List<ItemObjData> items; //�� �׸��忡 �־��� ������ ����Ʈ
    [SerializeField] private GameObject itemPref;
    [SerializeField] private InventoryItem selectedItem;
    [SerializeField] private RectTransform selectedRect;

    private InventoryItem overlapItem; //�������� ������ üũ�� ������������
    private InventoryItem checkOverlapItem; //������Ʈ �޼��忡�� üũ�� ������ ������
    private PlayerInput playerInput;

    //���̶���Ʈ ���� ����
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

    #region PlayerInput �׼�
    private void OnMousePosInput(InputAction.CallbackContext context)
    {
        //���콺�� ��ġ �ǽð� ������Ʈ
        mousePosInput = context.ReadValue<Vector2>();
    }

    private void OnMouseLeftClickInput(InputAction.CallbackContext context)
    {
        //��Ŭ���� �������� ���ų� ���� �̺�Ʈ
        ItemGetOrRelease();
    }
    private void OnMouseRightClickInput(InputAction.CallbackContext context)
    {
        //���콺 ��Ŭ����. ���õ� ������ ���ο� ���� �� ������ ���� Ȥ�� ������ ȸ��
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
        
        //���̶���Ʈ ����
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
    /// ���콺�� ��ġ�� Grid���� Ÿ�� ��ġ�� ��ȯ
    /// </summary>
    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = mousePosInput;
        //�������� ����ִٸ�
        if (selectedItem != null)
        {
            //�������� ���콺 �߾ӿ� ������ ����
            position.x -= (selectedItem.Width - 1) * ItemGrid.tilesizeWidth / 2;
            position.y += (selectedItem.Height - 1) * ItemGrid.tilesizeHeight / 2;
        }

        return selectedItemGrid.MouseToGridPosition(position);
    }

    /// <summary>
    /// ���õ� �������� ������� �ش� �������� ���콺�� ����ٴ�
    /// </summary>
    private void ItemSpriteDrag()
    {
        if (selectedItem != null)
        {
            selectedRect.position = mousePosInput;
        }
    }

    /// <summary>
    /// ��Ʈ�ѷ����� ������ ��ȸ�� ���
    /// </summary>
    public void RotateItemRight()
    {
        if(selectedItem == null) { return; }
        selectedItem.RotateRight();
    }

    /// <summary>
    /// ��Ʈ�ѷ����� ������ ��ȸ�� ���
    /// </summary>
    public void RotateItemLeft()
    {
        if (selectedItem == null) { return; }
        selectedItem.RotateLeft();
    }

    /// <summary>
    /// ��Ŭ���� �������� ���ų� ��������
    /// </summary>
    private void ItemGetOrRelease()
    {
        if (selectedItemGrid == null) { return; }
        Vector2Int tileGridPosition = updateGridPos; //���콺�� ��ġ�� �ִ� �׸��� ��ǥ �Ҵ�
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
    /// �������� ���� �õ�. �����ϸ� selectedRect�� �ش� �������� rect�� ����
    /// </summary>
    private void ItemGet(Vector2Int pos)
    {
        selectedItem = selectedItemGrid.PickUpItem(pos.x, pos.y);
        if (selectedItem != null)
        {
            selectedRect = selectedItem.GetComponent<RectTransform>();
            //������ �ڵ鷯�� �̹����� ���� �������� ���� ����
            selectedItem.GetComponent<Image>().raycastTarget = false;
            //�������� �׸��忡 �������°��� ����
            SetAllParentsAsLastSibling(selectedRect);
        }
    }

    /// <summary>
    /// �������� ���� �õ�.
    /// </summary>
    private void ItemRelease(InventoryItem item,Vector2Int pos)
    {
        bool complete = selectedItemGrid.PlaceItemCheck(item, pos.x, pos.y, ref overlapItem);
        if (complete)
        {
            selectedItem.GetComponent<Image>().raycastTarget = true;
            selectedItem.curItemGrid = selectedItemGrid;
            //��ġ�� �������� �ִٸ� selectedItem���� ����
            if (overlapItem != null)
            {
                backUpItemList.Add(selectedItem); //����� �ش� ����Ʈ ���� ������ ���� ���
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
                //��ġ�� �������� ������� ������� ������Ʈ
                selectedItem.backUpItemPos = pos; //���� ��ġ
                selectedItem.backUpItemRotate = selectedItem.curItemRotate; //���� ȸ��
                selectedItem.backUpItemGrid = selectedItem.curItemGrid; //���� �׸���
                selectedItem = null;

                if(backUpItemList.Count != 0)
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

                selectedItemGrid.UpdateBackupSlot(); //���� ��� ������ �迭 ������Ʈ

                //��Ŷ ����(�κ��丮�� ����)
                //�÷��̾� ���̵�, �κ��丮1�� id�� ����,�κ��丮2�� id�� ����
            }
        }
        else
        {
            //������ ��ġ ���н� �������� ���� ��ġ��
            //������ ���� ����ġ
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

            Debug.Log("�ǵ���");
            selectedItemGrid.UndoItemSlot();
            selectedItemGrid.PrintInventoryContents(selectedItemGrid.inventoryItemSlot);
        }
    }

    /// <summary>
    /// �����ۿ� ���콺�� ������� ���̶���Ʈ ȿ��
    /// ���߿� ���º� ���̶���Ʈ ���� �߰� ����
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
            //���õ� �������� ������� ���콺�� ����Ű�� ��ġ�� �������� �ִٸ� ���̶�����. ������ ��Ȱ��ȭ
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
            //���õ� �������� �ִٸ� �� �������� ��ġ�� ���� ���̶�����
            invenHighlight.Show(selectedItemGrid.BoundaryCheck(positionOnGrid.x,positionOnGrid.y,
                selectedItem.Width,selectedItem.Height));
            invenHighlight.SetSize(selectedItem);
            invenHighlight.SetParent(selectedItemGrid);
            invenHighlight.SetPositionByPos(selectedItemGrid, selectedItem,positionOnGrid.x,positionOnGrid.y);
        }
    }

    /// <summary>
    /// ��ϵ� �������� �������� ����
    /// </summary>
    private void CreateRandomItem()
    {
        //������ �������� �����ϰ� ��ũ��Ʈ �ε�
        InventoryItem invenItem = Instantiate(itemPref).GetComponent<InventoryItem>();
        //���� ���õ� �����۰� ��ƮƮ�������� �̰����� ������
        selectedItem = invenItem;
        selectedRect = invenItem.GetComponent<RectTransform>();
        //�������� ĵ������ �ڽ�(UI�ϱ�)
        SetAllParentsAsLastSibling(selectedRect);
        //������ ����Ʈ �� �ϳ� ����
        int selectedItemId = Random.Range(0, items.Count);
        //������ ������ �����͸� ������ �����տ� ����

        invenItem.Set(items[selectedItemId]);
        selectedItem.GetComponent<Image>().raycastTarget = false;
    }

    /// <summary>
    /// �������� ������ ��� �κ��丮���� ��ġ ������ �ڸ��� ��ġ��
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
    /// insertRandomItem�ȿ��� ������ �������� ������ �ڸ��� ��ġ
    /// </summary>
    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if(posOnGrid == null) { return; }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        selectedItemGrid = null;
    }

    /// <summary>
    /// ��� �θ𿡰� LastSibling�� ������. �׻� ������ ���ͷ����� rect�� ���� ������.
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


