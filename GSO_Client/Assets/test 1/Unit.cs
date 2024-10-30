using Google.Protobuf.Protocol;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class Unit : MonoBehaviour
{
    public int InstanceID { get; private set; }
    public UnitStat unitStat;

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ

    [SerializeField] private ItemData equipSlot1; //장착칸에 저장된 아이템 데이터
    [SerializeField] private ItemData equipSlot2;

    public Gun UsingGun; //손에 들고 있는 총 (사격을 할 총)
    public int loadedAmount1; //아직 안씀
    public int loadedAmount2;

    public Button quickSlotBtn1; //퀵슬롯 버튼
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


    //weaponslot에 item이 들어오면 발동
    private void UpdateSlot(ref ItemData equipSlot, ItemData newGunData, Button slotButton, int slotNumber)
    {
        //지정된 ItemData에 해당 총의 데이터를 저장(변경)
        equipSlot = newGunData; 
        slotButton.interactable = newGunData != null; //데이터 유무에 따라 버튼의 인터렉티브 조절

        if (newGunData == null)
        {
            UsingGun.ResetGun();
        }

        //else if (UsingGun.UsingGunData == null) //메인,서브칸이 모두 비어있을때 아이템을 둘중 어디든 장착하면 자동으로 손에 드는 코드
        //{
        //    UseGunInSlot(slotNumber);
        //}
    }

    private void HandleGunUnequip(int slotNumber)
    {
        //if (slotNumber == 1 && equipSlot2 != null && UsingGun.curGunEquipSlot == 1)
        //{
        //    //1번을 장착해제했을때 2번에 총이 있다면 2번을 사용함
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

    //해당 슬롯의 총을 사용 => 퀵슬롯 버튼리스터.
    private async void UseGunInSlot(int slotNumber)
    {
        await Task.Delay(100);
        ItemData equipptedItem = slotNumber == 1 ? equipSlot1 : equipSlot2;
        if (equipptedItem == null) 
        {
            SendChangeGunPacket(0); //총을 들고있지 않을 경우 0(널값) 전송
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
        Debug.Log($"C_ChangeAppearance 전송 {packet.ObjectId}, {packet.GunId}");
    }

}
