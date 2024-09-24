using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] private ItemData equipSlot1;
    [SerializeField] private ItemData equipSlot2;

    public Gun usingGun;
    public int loadedAmount1;
    public int loadedAmount2;

    public Button slot1btn;
    public Button slot2btn;

    public int InstanceID { get; private set; }
    public UnitStat unitStat;

    // Property for Slot 1
    public ItemData Slot1
    {
        get => equipSlot1;
        set => UpdateSlot(ref equipSlot1, value, slot1btn, 1);
    }

    // Property for Slot 2
    public ItemData Slot2
    {
        get => equipSlot2;
        set => UpdateSlot(ref equipSlot2, value, slot2btn, 2);
    }

    private void Awake()
    {
        InitializeButtons();
        usingGun = transform.Find("Pivot/Gun").GetComponent<Gun>();
        equipSlot1 = null;
        equipSlot2 = null;
    }

    public void Init()
    {
        InstanceID = gameObject.GetInstanceID();
        unitStat = new UnitStat();
        unitStat.Init();
    }

    private void InitializeButtons()
    {
        Transform quickSlots = GameObject.Find("WQuickSlot").transform;
        slot1btn = quickSlots.GetChild(0).GetComponent<Button>();
        slot2btn = quickSlots.GetChild(1).GetComponent<Button>();

        slot1btn.interactable = false;
        slot2btn.interactable = false;

        slot1btn.onClick.RemoveAllListeners();
        slot2btn.onClick.RemoveAllListeners();

        slot1btn.onClick.AddListener(() => EquipGunSlot(1));
        slot2btn.onClick.AddListener(() => EquipGunSlot(2));
    }

    private void UpdateSlot(ref ItemData equipSlot, ItemData newSlot, Button slotButton, int slotNumber)
    {
        equipSlot = newSlot;
        slotButton.interactable = newSlot != null;

        if (newSlot == null)
        {
            HandleGunUnequip(slotNumber);
        }
        else if (usingGun.CurGunData == null)
        {
            EquipGunSlot(slotNumber);
        }
    }

    private void HandleGunUnequip(int slotNumber)
    {
        if (slotNumber == 1 && equipSlot2 != null && usingGun.curGunEquipSlot == 1)
        {
            EquipGunSlot(2);
        }
        else if (slotNumber == 2 && equipSlot1 != null && usingGun.curGunEquipSlot == 2)
        {
            EquipGunSlot(1);
        }
        else
        {
            usingGun.ResetGun();
        }
    }

    private void EquipGunSlot(int slotNumber)
    {
        ItemData equipSlot = slotNumber == 1 ? equipSlot1 : equipSlot2;
        if (equipSlot == null) return;

        usingGun.SetGunStat(equipSlot);
        usingGun.curGunEquipSlot = slotNumber;
    }
}
