using Google.Protobuf.Protocol;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using Vector2 = System.Numerics.Vector2;


public partial class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    private const int MaxEquipSlots = 7;
    private const int PlayerSlotId = 0;

    private Dictionary<int, ItemData> EquipDict = new Dictionary<int, ItemData>();
    /*앞으로 사용할 기어 딕셔너리. 장착을 조작할때는 이 딕셔너리를 참조하여 변경
    * 1번 주무기
    * 2번 보조무기
    * 3번 방어구
    * 4번 가방
    * 5~7번 소모품
    */
    //딕셔너리의 아이템 검색
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
                return UIManager.Instance.SetWQuickSlot(slotId, 0);
            case ItemType.Defensive:
                //아직 없음
                break;
            case ItemType.Bag:
                playerInvenUI.ResetInventoryGrid();
                break;
            case ItemType.Recovery:
                return UIManager.Instance.SetIQuickSlot(slotId, null);
            default:
                Debug.Log("장착불가능한 아이템입니다.");
                return false;
        }

        EquipDict[slotId] = null;
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
    public ItemData GetItemInDictByItemObjId(int objId)
    {
        ItemData findData = EquipDict.Values.FirstOrDefault(data => data.objectId == objId);
        if(findData == null)
        {
            return null;
        }

        return findData;
    }

    public static Dictionary<int, EquipSlotBase> equipSlotDic { get; private set; } = new Dictionary<int, EquipSlotBase>();
    public static Dictionary<int, ItemObject> instantItemDic { get; private set; } = new Dictionary<int, ItemObject>(); //인벤토리를 닫으면 초기화됨


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
    [SerializeField] private EquipSlotBase selectedEquip;
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
        EquipSlotBase[] slots = inventoryUI.GetComponentsInChildren<EquipSlotBase>();

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
        if (Instance.playerInvenUI.instantGrid == null)
        {
            return;
        }
        Instance.playerInvenUI.instantGrid.UpdateGridWeight();
        Instance.playerInvenUI.WeightTextSet(
            Instance.playerInvenUI.instantGrid.GridWeight,
            Instance.playerInvenUI.instantGrid.limitWeight);
    }

}


