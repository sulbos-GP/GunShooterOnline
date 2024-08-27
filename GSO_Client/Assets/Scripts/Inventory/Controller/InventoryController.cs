using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Eval;
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
    public Dictionary<int,ItemObject> instantItemDic;

    [Header("��������")]
    public Transform deleteUI;
    public Button rotateBtn;

    [Header("����׿�")]
    [SerializeField] private Vector2 mousePosInput; 
    [SerializeField] private Vector2Int gridPosition; //mousePosInput�� WorldToGridPos�޼���� �׸��� ���� ��ǥ�� ��ȯ�Ѱ�. �׸��尡 �����Ǿ� �־����
    [SerializeField] private Vector2Int gridPositionIndex;
    public ItemObject SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            isItemSelected = selectedItem != null;
            rotateBtn.interactable = isItemSelected;

            if (isItemSelected)
            {
                selectedRect = value.GetComponent<RectTransform>();

                float addedWeight = playerInvenUI.instantGrid.GridWeight + selectedItem.itemWeight;
                playerInvenUI.weightText.text = $"WEIGHT \n{addedWeight} / {playerInvenUI.instantGrid.limitWeight}";
                if (addedWeight > playerInvenUI.instantGrid.limitWeight)
                {
                    playerInvenUI.weightText.color = Color.red;
                }
            }
            else
            {
                selectedRect = null;

                playerInvenUI.weightText.text = $"WEIGHT \n{playerInvenUI.instantGrid.GridWeight} / {playerInvenUI.instantGrid.limitWeight}";
                playerInvenUI.weightText.color = Color.white;
            }
        }
    }

    [SerializeField] private ItemObject selectedItem;
    [SerializeField] private RectTransform selectedRect;
    public bool isItemSelected; //���� ���õ� ��������

    public GridObject SelectedGrid
    {
        get => selectedGrid;
        set
        {
            selectedGrid = value;
            isGridSelected = selectedGrid != null;
            invenHighlight.SetParent(isGridSelected ? value : null);

            if (isItemSelected)
            {
                selectedItem.curItemGrid = value;
            }
        }
    }

    [SerializeField] private GridObject selectedGrid;
    public bool isGridSelected; 

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

    //���� ����
    [SerializeField] private EquipSlot selectedEquip;
    [SerializeField] private bool isEquipSelected;
    public EquipSlot SelectedEquip
    {
        get => selectedEquip;
        set
        {
            selectedEquip = value;
            if(selectedEquip != null)
            {
                isEquipSelected = true;
            }
            else
            {
                isEquipSelected = false;
            }

            if (isItemSelected) {
                selectedItem.curEquipSlot = value;
            }
        }
    }

    [SerializeField] private ItemObject overlapItem; //�� �����Ӹ��� üũ�� ������ ������ ����

    //UI��Ƽ�� ����
    public bool isActive = false;


    //���̶���Ʈ ���� ����
    private InvenHighLight invenHighlight;
    private Vector2Int HighlightPosition; //���̶���Ʈ�� ��ġ

    public bool itemPlaceableInGrid;


    private bool isDivideMode;
    public bool isDivideInterfaceOn;
    public GameObject itemPreviewInstance; // ���� ������ �̸����� �ν��Ͻ�
    private Vector2 lastDragPosition; // ������ �巡�� ��ġ
    private float dragTime = 0f; // �巡�� �ð��� ����� �ð�
    private const float maxDragTime = 2f; // �ִ� �巡�� ��� �ð� (��)

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

        Button inventoryBtn = GameObject.Find("InventoryBtn").GetComponent<Button>();
        if (inventoryBtn == null) { Debug.Log("��ư�� ã������"); }
        inventoryBtn.onClick.AddListener(InvenBtn);

        rotateBtn.onClick.AddListener(RotateBtn);

        instantItemDic = new Dictionary<int, ItemObject>();
        invenHighlight = GetComponent<InvenHighLight>();
    }

    private void OnDisable()
    {
        ResetSelection();
    }


    public void ResetSelection()
    {
        SelectedItem = null;
        SelectedGrid = null;
        SelectedEquip = null;
        overlapItem = null;
        isPress = false;
        isOnDelete = false;
        itemPlaceableInGrid = false;
        isDivideMode = false;
        dragTime = 0;
        if(itemPreviewInstance != null)
        {
            Managers.Resource.Destroy(itemPreviewInstance);
        }
    }

    /// <summary>
    /// Ŭ���� ������Ʈ�� ��� �θ𿡰� LastSibling�� ������. �׻� ������ ���ͷ����� rect�� ���� ������.
    /// </summary>
    public void SetSelectedObjectToLastSibling(Transform child)
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
    

    
}


