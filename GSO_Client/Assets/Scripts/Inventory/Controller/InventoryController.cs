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

    private PlayerInput playerInput; //플레이어의 조작 인풋

    public GameObject inventoryUI;
    public PlayerInventoryUI playerInvenUI;
    public OtherInventoryUI otherInvenUI;
    public Dictionary<int,ItemObject> instantItemDic;

    [Header("수동지정")]
    public Transform deleteUI;
    public Button rotateBtn;

    [Header("디버그용")]
    [SerializeField] private Vector2 mousePosInput; 
    [SerializeField] private Vector2Int gridPosition; //mousePosInput을 WorldToGridPos메서드로 그리드 내의 좌표로 변환한것. 그리드가 지정되어 있어야함
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
    public bool isItemSelected; //현재 선택된 상태인지

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

    //삭제 관련
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

    //장착 관련
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

    [SerializeField] private ItemObject overlapItem; //매 프레임마다 체크될 오버랩 아이템 변수

    //UI액티브 관련
    public bool isActive = false;


    //하이라이트 관련 변수
    private InvenHighLight invenHighlight;
    private Vector2Int HighlightPosition; //하이라이트의 위치

    public bool itemPlaceableInGrid;


    private bool isDivideMode;
    public bool isDivideInterfaceOn;
    public GameObject itemPreviewInstance; // 현재 아이템 미리보기 인스턴스
    private Vector2 lastDragPosition; // 마지막 드래그 위치
    private float dragTime = 0f; // 드래그 시간이 경과한 시간
    private const float maxDragTime = 2f; // 최대 드래그 대기 시간 (초)

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
        if (inventoryBtn == null) { Debug.Log("버튼을 찾지못함"); }
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
    /// 클릭한 오브젝트의 모든 부모에게 LastSibling을 적용함. 항상 지정한 인터렉션한 rect을 가장 앞으로.
    /// </summary>
    public void SetSelectedObjectToLastSibling(Transform child)
    {
        if (child == null) return;

        if(child.gameObject.name == "InventoryUI") //인벤토리 UI의 자식들 까지만 적용함
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


