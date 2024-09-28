using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController : MonoBehaviour
{
    public static InventoryController invenInstance;

    private const int MaxEquipSlots = 7;
    private const int PlayerSlotId = 0;

    public static Dictionary<int, EquipSlot> equipSlotDic { get; private set; } = new Dictionary<int, EquipSlot>();
    public static Dictionary<int, ItemObject> instantItemDic { get; private set; } = new Dictionary<int, ItemObject>();


    //UI
    public GameObject inventoryUI;
    public PlayerInventoryUI playerInvenUI;
    public OtherInventoryUI otherInvenUI;
    public Transform deleteUI;
    public Button rotateBtn;

    //플레이어 조작
    private PlayerInput playerInput;
    private Vector2 mousePosInput;
    [SerializeField] private Vector2Int gridPosition;
    private Vector2Int gridPositionIndex;
    private bool isPress = false; //화면이 눌려진 상태인가?
    public bool isActive = false; //UI가 켜져 있는가?

    //아이템
    [SerializeField] private ItemObject selectedItem;
    private RectTransform selectedRect;
    private ItemObject overlapItem;

    public bool isItemSelected;

    //그리드
    [SerializeField] private GridObject selectedGrid;
    public bool isGridSelected;
    public bool itemPlaceableInGrid; //아이템이 그리드에 들어갈수 있는가?

    //장착칸
    [SerializeField] private EquipSlot selectedEquip;
    private bool isEquipSelected;

    //하이라이트
    private InvenHighLight invenHighlight;
  
    //아이템 삭제
    public bool isOnDelete;

    //아이템 나누기
    private const float maxDragTime = 2f;

    private GameObject itemPreviewInstance;
    private Vector2 lastDragPosition;
    private float dragTime = 0f;

    public bool isDivideMode;
    private bool divideCheckOff;
    public bool isDivideInterfaceOn;

    #region 옵저버
    public ItemObject SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            isItemSelected = selectedItem != null;
            rotateBtn.interactable = isItemSelected;

            selectedRect = isItemSelected ? value.GetComponent<RectTransform>() : null;

            if (!isItemSelected) //아이템을 배치할때 플레이어의 인벤토리 무게 업데이트
            {
                if (playerInvenUI.instantGrid == null)
                {
                    return;
                }
                playerInvenUI.weightText.text = $"WEIGHT \n{playerInvenUI.instantGrid.GridWeight} / {playerInvenUI.instantGrid.limitWeight}";
                playerInvenUI.weightText.color = Color.white;
            }
        }
    }
    public GridObject SelectedGrid
    {
        get => selectedGrid;
        set
        {
            selectedGrid = value;
            isGridSelected = selectedGrid != null;

            invenHighlight?.SetHighlightParent(isGridSelected ? value.gameObject : null);

            if (isItemSelected && isGridSelected)
            {
                selectedItem.parentObjId = value.objectId;
            }
        }
    }
    public bool IsOnDelete
    {
        get => isOnDelete;
        set
        {
            isOnDelete = value;
            deleteUI.GetComponent<DeleteZone>().IsDeleteOn = value;
        }
    }
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
    #endregion

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
        EquipSlotsSet();
        InvenBtnSet();
    }

    private void InvenBtnSet()
    {
        Button inventoryBtn = GameObject.Find("InventoryBtn").GetComponent<Button>();
        if (inventoryBtn != null)
        {
            inventoryBtn.onClick.RemoveAllListeners();
            inventoryBtn.onClick.AddListener(InvenOnOffBtn);
        }

        rotateBtn.onClick.RemoveAllListeners();
        rotateBtn.onClick.AddListener(RotateBtn);
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
        divideCheckOff = false;
        dragTime = 0;

        if (itemPreviewInstance != null)
        {
            Managers.Resource.Destroy(itemPreviewInstance);
        }

        UpdatePlayerWeight();
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

    private void EquipSlotsSet()
    {
        EquipSlot[] slots = inventoryUI.GetComponentsInChildren<EquipSlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            equipSlotDic.Add(i + 1, slots[i]);
            slots[i].Init();
        }
    }

    public void SetSelectedObjectToLastSibling(Transform child)
    {
        if (child == null || child.gameObject.name == "InventoryUI")
            return;

        child.SetAsLastSibling();

        if (child.parent != null)
        {
            SetSelectedObjectToLastSibling(child.parent);
        }
    }



    //static 함수들

    /// <summary>
    /// 해당 아이디가 장착칸의 아이디인지
    /// </summary>
    public static bool IsEquipSlot(int objectId)
    {
        return objectId > 0 && objectId <= 7;
    }

    /// <summary>
    /// 해당 아이디가 플레이어의 아이디인지
    /// </summary>
    public static bool IsPlayerSlot(int objectId)
    {
        return !(objectId > 0 && objectId <= 7) && objectId == 0;
    }

    /// <summary>
    /// 해당 아이디가 나머지(장비칸, 플레이어 제외)의 아이디인지
    /// </summary>
    public static bool IsOtherSlot(int objectId)
    {
        return !(objectId > 0 && objectId <= 7) && objectId != 0;
    }

    /// <summary>
    /// 플레이어 인벤의 무게를 업데이트
    /// </summary>
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

}


