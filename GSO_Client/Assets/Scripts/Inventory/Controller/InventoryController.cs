using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Eval;
using Org.BouncyCastle.Asn1.Crmf;
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
    public static Dictionary<int, EquipSlot> equipSlotDic = new Dictionary<int, EquipSlot>();
    public static Dictionary<int, ItemObject> instantItemDic = new Dictionary<int, ItemObject>();
    public static bool IsEquipSlot(int objectId)
    {
        return objectId > 0 && objectId <= 7;
    }

    public static bool IsPlayerSlot(int objectId)
    {
        return !(objectId > 0 && objectId <= 7) && objectId == 0;
    }

    public static bool IsOtherSlot(int objectId)
    {
        return !(objectId > 0 && objectId <= 7) && objectId != 0;
    }

    public static void UpdatePlayerWeight()
    {
        if (invenInstance.playerInvenUI.instantGrid == null)
        {
            return;
        }
        invenInstance.playerInvenUI.instantGrid.UpdateGridWeight();
        invenInstance.playerInvenUI.WeightTextSet(
            invenInstance.playerInvenUI.instantGrid.GridWeight,
            invenInstance.playerInvenUI.instantGrid.limitWeight);
    }

    private PlayerInput playerInput;

    public GameObject inventoryUI;
    public PlayerInventoryUI playerInvenUI;
    public OtherInventoryUI otherInvenUI;


    public Transform deleteUI;
    public Button rotateBtn;

    [SerializeField] private Vector2 mousePosInput;
    [SerializeField] private Vector2Int gridPosition;
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
    public bool isItemSelected;

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


    [SerializeField] private EquipSlot selectedEquip;
    [SerializeField] private bool isEquipSelected;
    public EquipSlot SelectedEquip
    {
        get => selectedEquip;
        set
        {
            selectedEquip = value;
            if (selectedEquip != null)
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

    private ItemObject overlapItem;


    public bool isActive = false;



    private InvenHighLight invenHighlight;
    private Vector2Int HighlightPosition;

    public bool itemPlaceableInGrid;

    public bool isDivideMode;
    private bool dontCheckDivide;
    public bool isDivideInterfaceOn;
    public GameObject itemPreviewInstance;
    private Vector2 lastDragPosition;
    private float dragTime = 0f;
    private const float maxDragTime = 2f;


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
        if (inventoryBtn == null) { Debug.Log("인벤토리 버튼이 없음"); }

        inventoryBtn.onClick.RemoveAllListeners();
        inventoryBtn.onClick.AddListener(InvenBtn);
        rotateBtn.onClick.RemoveAllListeners();
        rotateBtn.onClick.AddListener(RotateBtn);

        invenHighlight = GetComponent<InvenHighLight>();
    }

    private void SetEquipSlot()
    {
        Transform EquipSector = inventoryUI.transform.GetChild(0);

        for (int i = 0; i < EquipSector.childCount; i++) {
            if (EquipSector.GetChild(i).GetComponent<EquipSlot>() == null)
            {
                continue;
            }
            EquipSlot equip = EquipSector.GetChild(i).GetComponent<EquipSlot>();
            equipSlotDic.Add(i + 1, equip);
            equip.Init();
        }
    }

    private void OnDisable()
    {
        ResetSelection();
    }

    /// <summary>
    /// 모든 셀렉션 초기화
    /// </summary>
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
        if (itemPreviewInstance != null)
        {
            Managers.Resource.Destroy(itemPreviewInstance);
        }

        UpdatePlayerWeight();
        Debug.Log("reset");

    }

    [ContextMenu("아이템 보기")]
    public void DebugDic()
    {
        Debug.Log($"itemDic");
        foreach (ItemObject item in instantItemDic.Values)
        {
            Debug.Log("Key: " + item.itemData.objectId + ", Value: " + item.itemData.item_name);
        }
    }

    public void SetSelectedObjectToLastSibling(Transform child)
    {
        if (child == null) return;

        if(child.gameObject.name == "InventoryUI")
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


