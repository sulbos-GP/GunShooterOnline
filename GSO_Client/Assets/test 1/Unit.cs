using Google.Protobuf.Protocol;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class Unit : MonoBehaviour
{
    public int InstanceID { get; private set; }
    public UnitStat unitStat;

    //�ѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤ�

    [SerializeField] private ItemData equipSlot1; //����ĭ�� ����� ������ ������
    [SerializeField] private ItemData equipSlot2;

    public Gun UsingGun; //�տ� ��� �ִ� �� (����� �� ��)
    public int loadedAmount1; //���� �Ⱦ�
    public int loadedAmount2;

    public Button quickSlotBtn1; //������ ��ư
    public Button quickSlotBtn2;

    
    public ItemData Slot1Equip
    {
        get => equipSlot1;
        set => UpdateSlot(ref equipSlot1, value, quickSlotBtn1, 1);
    }
    public ItemData Slot2Equip
    {
        get => equipSlot2;
        set => UpdateSlot(ref equipSlot2, value, quickSlotBtn2, 2);
    }

    public void Init()
    {
        InitQuickSlotBtn();

        InstanceID = gameObject.GetInstanceID();
        unitStat = new UnitStat();
        unitStat.Init();

        UsingGun = transform.Find("Pivot/Gun").GetComponent<Gun>();

        equipSlot1 = null;
        equipSlot2 = null;

        
    }

    private void InitQuickSlotBtn()
    {
        Transform Canvas = GameObject.Find("Canvas").transform;
        Transform quickSlots = Canvas.Find("WQuickSlot").transform;
        quickSlotBtn1 = quickSlots.GetChild(0).GetComponent<Button>();
        quickSlotBtn2 = quickSlots.GetChild(1).GetComponent<Button>();

        quickSlotBtn1.interactable = false;
        quickSlotBtn2.interactable = false;

        quickSlotBtn1.onClick.RemoveAllListeners();
        quickSlotBtn2.onClick.RemoveAllListeners();

        quickSlotBtn1.onClick.AddListener(() => UseGunInSlot(1));
        quickSlotBtn2.onClick.AddListener(() => UseGunInSlot(2));
    }


    //weaponslot�� item�� ������ �ߵ�
    private void UpdateSlot(ref ItemData equipSlot, ItemData newGunData, Button slotButton, int slotNumber)
    {
        //������ ItemData�� �ش� ���� �����͸� ����(����)
        equipSlot = newGunData; 
        slotButton.interactable = newGunData != null; //������ ������ ���� ��ư�� ���ͷ�Ƽ�� ����

        if (newGunData == null)
        {
            UsingGun.ResetGun();
        }

        //else if (UsingGun.UsingGunData == null) //����,����ĭ�� ��� ��������� �������� ���� ���� �����ϸ� �ڵ����� �տ� ��� �ڵ�
        //{
        //    UseGunInSlot(slotNumber);
        //}
    }

    private void HandleGunUnequip(int slotNumber)
    {
        //if (slotNumber == 1 && equipSlot2 != null && UsingGun.curGunEquipSlot == 1)
        //{
        //    //1���� �������������� 2���� ���� �ִٸ� 2���� �����
        //    UseGunInSlot(2);
        //}
        //else if (slotNumber == 2 && equipSlot1 != null && UsingGun.curGunEquipSlot == 2)
        //{
        //    UseGunInSlot(1);
        //}
        //else
        //{
        //    UsingGun.ResetGun();
        //}

        UsingGun.ResetGun();
    }

    //�ش� ������ ���� ��� => ������ ��ư������.
    private async void UseGunInSlot(int slotNumber)
    {
        await Task.Delay(100);
        ItemData equipptedItem = slotNumber == 1 ? equipSlot1 : equipSlot2;
        if (equipptedItem == null) 
        {
            SendChangeGunPacket(0); //���� ������� ���� ��� 0(�ΰ�) ����
            return;
        }

        UsingGun.SetGunStat(equipptedItem);
        UsingGun.curGunEquipSlot = slotNumber;
        SendChangeGunPacket(equipptedItem.objectId);
    }

    private static void SendChangeGunPacket(int gunObjectId)
    {
        C_ChangeAppearance packet = new C_ChangeAppearance()
        {
            ObjectId = Managers.Object.MyPlayer.Id,
            GunId = gunObjectId
        };

        Managers.Network.Send(packet);
        Debug.Log($"C_ChangeAppearance ���� {packet.ObjectId}, {packet.GunId}");
    }

}
