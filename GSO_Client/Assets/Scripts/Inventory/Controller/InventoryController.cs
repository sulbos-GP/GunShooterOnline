using Google.Protobuf.Protocol;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    public const int MaxEquipSlots = 7;
    public const int PlayerSlotId = 0;

    private Dictionary<int, ItemData> EquipDict = new Dictionary<int, ItemData>(); //장착아이템 딕셔너리
    public bool SetEquipItem(int slotId, ItemData item)
    {
        if(item == null)
        {
            return false;
        }

        EquipDict[slotId] = item;

        switch (item.item_type)
        {
            case ItemType.Weapon:
                UIManager.Instance.SetWQuickSlot(slotId, item.itemId);
                break;
            case ItemType.Defensive:
                //아직 없음
                break;
            case ItemType.Bag:
                    return playerInvenUI.SetInventoryGrid(item.itemId);
            case ItemType.Recovery:
                UIManager.Instance.SetIQuickSlot(slotId, item);
                break;
            default:
                Debug.Log("장착불가능한 아이템입니다.");
                break;
        }

        if (Managers.Object.MyPlayer.usingGun != null && slotId == Managers.Object.MyPlayer.usingGun.curGunEquipSlot)
        {
            Managers.Object.MyPlayer.ChangeUseGun(slotId);
        }

        return true;
    }
    public bool UnsetEquipItem(int slotId)
    {
        if (EquipDict[slotId] == null)
        {
            return false;
        }

        switch (EquipDict[slotId].item_type)
        {
            case ItemType.Weapon:
                 UIManager.Instance.SetWQuickSlot(slotId, 0); break;
            case ItemType.Defensive:
                break;
            case ItemType.Bag:
                if(!playerInvenUI.ResetInventoryGrid())
                {
                    Debug.Log("실패");
                    UndoItem(selectedItem);
                    ResetSelection();
                    return false;
                }
                break;
            case ItemType.Recovery:
                UIManager.Instance.SetIQuickSlot(slotId, null); break;
            default:
                Debug.Log("장착불가능한 아이템입니다.");
                return false;
        }

        EquipDict[slotId] = null;


        if (slotId == Managers.Object.MyPlayer.usingGun.curGunEquipSlot)
        {
            Managers.Object.MyPlayer.ChangeUseGun(slotId);
        }
        return true;
    }
    public ItemData GetItemInDictByGearCode(int GearCode)
    {
        if (EquipDict[GearCode] == null)
        {
            return null;
        }

        return EquipDict[GearCode];
    }

    public static Dictionary<int, EquipSlotBase> equipSlotDic { get; private set; } = new Dictionary<int, EquipSlotBase>(); //장착칸을 아이디별로 저장
    public static Dictionary<int, ItemObject> instantItemDic { get; private set; } = new Dictionary<int, ItemObject>(); //생성된 아이템. 인벤토리를 닫으면 클리어

    [Header("UI Component")]
    public GameObject inventoryUI;
    public PlayerInventoryUI playerInvenUI;
    public OtherInventoryUI otherInvenUI;
    public Transform deleteUI;
    public Button rotateBtn;
    public bool OnWaitSwitchPacket;


    [Header("PlayerInput")]
    public bool isActive = false; //UI가 켜져 있는가?
    [SerializeField] private Vector2Int gridPosition;
    private PlayerInput playerInput;
    private Vector2 mousePosInput;
    private Vector2Int gridPositionIndex;
    private bool isPress = false; //화면이 눌려진 상태인가?
    

    [Header("Interact Item")]
    [SerializeField] private ItemObject selectedItem;
    private RectTransform selectedRect;
    public bool isItemSelected;
    private ItemObject overlapItem;
    

    [Header("Interact grid")]
    [SerializeField] private GridObject selectedGrid;
    public bool isGridSelected;
    public bool itemPlaceableInGrid; //아이템이 그리드에 들어갈수 있는가?

    [Header("Interact equipslot")]
    [SerializeField] private EquipSlotBase selectedEquip;
    private bool isEquipSelected;

    [Header("highlight")]
    private InvenHighLight invenHighlight;

    [Header("Divide&Merge Variable")]
    public bool isOnDelete;
    public bool isDivideMode;
    public bool isDivideInterfaceOn;

    private const float maxDragTime = 2f;
    private GameObject itemPreviewInstance;
    private Vector2 lastDragPosition;
    private float dragTime = 0f;
    private bool divideCheckOff;
    

    #region Observer
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
    public EquipSlotBase SelectedEquip
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
        Debug.Log("Instance");
        if (Instance == null)
        {
            Instance = this;

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
            inventoryBtn.onClick.AddListener(InventoryActiveBtn);
        }

        rotateBtn.onClick.RemoveAllListeners();
        rotateBtn.onClick.AddListener(RotateBtn);
    }

    /// <summary>
    /// 아이템 인터렉트 종료 후 초기화
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

        UpdateInvenWeight();
        UpdateInvenWeight(false);
    }

    

    private void EquipSlotsSet()
    {
        EquipSlotBase[] slots = inventoryUI.GetComponentsInChildren<EquipSlotBase>();

        for (int i = 0; i < slots.Length; i++)
        {
            equipSlotDic[i + 1] = slots[i];
            slots[i].Init();
        }
    }

    public void SetSelectedObjectToLastSibling(Transform child)
    {
        if (child == null || child.parent.name == "Canvas")
            return;

        child.SetAsLastSibling();

        if (child.parent != null)
        {
            SetSelectedObjectToLastSibling(child.parent);
        }
    }


    /// <summary>
    /// 해당 아이디가 장착칸의 아이디인지
    /// </summary>
    public static bool IsEquipSlot(int objectId)
    {
        return objectId > PlayerSlotId && objectId <= MaxEquipSlots;
    }

    /// <summary>
    /// 해당 아이디가 플레이어의 아이디인지
    /// </summary>
    public static bool IsPlayerSlot(int objectId)
    {
        return !(objectId > PlayerSlotId && objectId <= MaxEquipSlots) && objectId == PlayerSlotId;
    }

    /// <summary>
    /// 해당 아이디가 나머지(장비칸, 플레이어 제외)의 아이디인지
    /// </summary>
    public static bool IsOtherSlot(int objectId)
    {
        return !(objectId > PlayerSlotId && objectId <= MaxEquipSlots) && objectId != PlayerSlotId;
    }

    public static void UpdateInvenWeight(bool isPlayerInven = true)
    {
        InventoryUI targetUI;
        if (isPlayerInven) 
        {
            targetUI = Instance.playerInvenUI;
        }
        else
        {
            targetUI= Instance.otherInvenUI;
        }

        if (targetUI.instantGrid == null)
        {
            targetUI.SetWeightText(0, 0, false);
            return;
        }
        targetUI.instantGrid.UpdateGridWeight();
        targetUI.SetWeightText(
            targetUI.instantGrid.GridWeight,
            targetUI.instantGrid.limitWeight);
    }

    /// <summary>
    /// 아이템의 무게에 따른 증감 후 텍스트 적용
    /// </summary>
    public static void AdjustWeight(InventoryController inven,int parentId, double weight, bool isAdding)
    {
        double curWeight, resultWeight;

        if (parentId == 0 && inven.playerInvenUI?.instantGrid != null)
        {
            curWeight = inven.playerInvenUI.instantGrid.GridWeight;
            resultWeight = isAdding ? curWeight + weight : curWeight - weight;
            inven.playerInvenUI.SetWeightText(resultWeight, inven.playerInvenUI.instantGrid.limitWeight);
        }
        else if (parentId > 7 && inven.otherInvenUI?.instantGrid != null)
        {
            curWeight = inven.otherInvenUI.instantGrid.GridWeight;
            resultWeight = isAdding ? curWeight + weight : curWeight - weight;
            inven.otherInvenUI.SetWeightText(resultWeight, inven.otherInvenUI.instantGrid.limitWeight);
        }
        else
        {
            Debug.Log("장비칸이거나 유효하지 않은 아이디입니다.");
        }
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
}


