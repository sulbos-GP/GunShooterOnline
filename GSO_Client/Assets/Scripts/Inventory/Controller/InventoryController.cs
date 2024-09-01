using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml.Chart;
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

    public EquipSlot Weapon1;
    public EquipSlot Weapon2;
    public EquipSlot Armor;
    public EquipSlot Bag;
    public EquipSlot Consume1;
    public EquipSlot Consume2;
    public EquipSlot Consume3;

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

            selectedRect = isItemSelected ? value.GetComponent<RectTransform>() : null;

            if (!isItemSelected)
            {
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

            if (isItemSelected && isGridSelected)
            {
                selectedItem.parentObjId = value.objectId;
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
                if (isItemSelected)
                {
                    selectedItem.parentObjId = value.slotId;
                }
            }
            else
            {
                isEquipSelected = false;
            }

            
        }
    }

    private ItemObject overlapItem; //�� �����Ӹ��� üũ�� ������ ������ ����

    //UI��Ƽ�� ����
    public bool isActive = false;
   

    //���̶���Ʈ ���� ����
    private InvenHighLight invenHighlight;
    private Vector2Int HighlightPosition; //���̶���Ʈ�� ��ġ

    public bool itemPlaceableInGrid;

    public bool isDivideMode; //divide��尡 ��������
    private bool dontCheckDivide; //�����̰��� true��� ���̻� divideüũ�� ���� ����
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

        SetEquipSlot();

        Button inventoryBtn = GameObject.Find("InventoryBtn").GetComponent<Button>();
        if (inventoryBtn == null) { Debug.Log("��ư�� ã������"); }

        inventoryBtn.onClick.RemoveAllListeners();
        inventoryBtn.onClick.AddListener(InvenBtn);
        rotateBtn.onClick.RemoveAllListeners();
        rotateBtn.onClick.AddListener(RotateBtn);

        instantItemDic = new Dictionary<int, ItemObject>();
        invenHighlight = GetComponent<InvenHighLight>();
    }

    private void SetEquipSlot()
    {
        Transform EquipSector = inventoryUI.transform.GetChild(0);
        Weapon1 = EquipSector.GetChild(0).GetComponent<EquipSlot>();
        Weapon2 = EquipSector.GetChild(1).GetComponent<EquipSlot>();
        Armor = EquipSector.GetChild(2).GetComponent<EquipSlot>();
        Bag = EquipSector.GetChild(3).GetComponent<EquipSlot>();
        Consume1 = EquipSector.GetChild(4).GetComponent<EquipSlot>();
        Consume2 = EquipSector.GetChild(5).GetComponent<EquipSlot>();
        Consume3 = EquipSector.GetChild(6).GetComponent<EquipSlot>();

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
        dontCheckDivide = false;
        dragTime = 0;
        if(itemPreviewInstance != null)
        {
            Managers.Resource.Destroy(itemPreviewInstance);
        }

        Debug.Log("reset");

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


