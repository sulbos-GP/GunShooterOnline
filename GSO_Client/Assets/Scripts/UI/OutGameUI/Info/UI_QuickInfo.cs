using MathNet.Numerics.Distributions;
using MathNet.Numerics.RootFinding;
using NPOI.POIFS.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using Ubiety.Dns.Core;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.Match;
using static AuthorizeResource;
using static GameResource;
using static UnityEngine.UI.CanvasScaler;

public class UI_QuickInfo : LobbyUI
{
    protected override ELobbyUI type => ELobbyUI.QuickInfo;

    [SerializeField]
    private Button quickInfoBtn;

    [SerializeField]
    private Button quickResetBtn;

    [SerializeField]
    private Button quickCloseBtn;

    [SerializeField]
    private GameObject quickInfoUI;

    private bool isProcess;

    public GridObject infoGrid;
    public EquipSlotBase[] slots;
    private Dictionary<int,EquipSlotBase> equipSlots = new Dictionary<int, EquipSlotBase>();
    private List<ItemObject> instantItemObjs = new List<ItemObject>();


    private void Awake()
    {
        quickInfoBtn.onClick.AddListener(OnClickQuickInfo);
        quickResetBtn.onClick.AddListener(OnClickQuickReset);
        quickCloseBtn.onClick.AddListener(OnClickQuickClose);
    }

    public override void InitUI()
    {
        isProcess = false;

        foreach (EquipSlotBase equip in slots)
        {
            equipSlots[equip.slotId] = equip;
            equip.Init();
        }
    }
    
    private int GearStringToInt(string part)
    {
        switch (part)
        {
            case "main_weapon" : return 1;
            case "sub_weapon": return 2;
            case "armor": return 3;
            case "backpack": return 4;
            case "pocket_first": return 5;
            case "pocket_second": return 6;
            case "pocket_third": return 7;
            default: return 0;
        }
    }
    
    public override void UpdateUI()
    {
        //������ �����۵��� �ִٸ� �ʱ�ȭ : �̺κ��� ���� ���� �ݰų� �ʱ�ȭ�� ��ó���� �־ ��
        if (instantItemObjs.Count != 0)
        {
            Managers.SystemLog.Message($"updateUI : ���� ������ ����");
            for (int i = instantItemObjs.Count-1; i >= 0; i--)
            {
                if (instantItemObjs[i] != null)
                {
                    Managers.Resource.Destroy(instantItemObjs[i].gameObject);
                }
                else
                {
                    Managers.SystemLog.Message($"updateUI : Null ��ü �߰�, ���� ��ŵ");
                }
            }

            instantItemObjs.Clear();
            Managers.SystemLog.Message($"Ŭ���� �Ϸ�");
        }

        quickResetBtn.gameObject.SetActive(true);
        quickInfoUI.SetActive(true);

        //���� ������ ����
        List<DB_GearUnit> gears = Managers.Web.Models.Gear;

        if (gears == null)
        {
            Managers.SystemLog.Message($"updateUI : gearList is null");
            return;
        }

        foreach (var unit in gears)
        {
            Managers.SystemLog.Message($"updateUI :  Gear = {unit.attributes.item_id}");

            ItemData itemData = new ItemData();
            itemData.SetItemData(unit);

            ItemObject gearItem = Managers.Resource.Instantiate("UI/InvenUI/ItemUI", transform).GetComponent<ItemObject>();
            gearItem.SetItem(itemData);
            gearItem.RevealItem();
            instantItemObjs.Add(gearItem);

            int gearCode = GearStringToInt(unit.gear.part);
            EquipSlotBase target = equipSlots.GetValueOrDefault(gearCode, null);
            if(target == null)
            {
                Managers.SystemLog.Message($"����ĭ�� �������� ���� : {unit.gear.part}");
            }
            target.SetItemEquip(gearItem, true);
            Managers.SystemLog.Message($"Item = {itemData.itemId} size = {itemData.width},{itemData.height}, amount = {itemData.amount}");
        }

        //�׸��� ���� �� ����
        Vector2Int size = new Vector2Int(3, 2);
        if (equipSlots[4].equipItemObj != null)
        {
            Data_master_item_backpack bagData = Data_master_item_backpack.GetData(equipSlots[4].equipItemObj.itemData.itemId);
            if (bagData == null)
            {
                Managers.SystemLog.Message($"���� üũ : ���� �����͸� ã������");
            }
            size = new Vector2Int(bagData.total_scale_x, bagData.total_scale_y);
            Managers.SystemLog.Message($"���� üũ : ������ ������ = {size.x} , {size.y}");
        }
        else
        {
            Managers.SystemLog.Message($"���� üũ : ���� ���� �⺻�������");
        }

        infoGrid.InstantGrid(new Vector2Int(size.x, size.y), 100, true);
        Managers.SystemLog.Message($"���� gridSize = {size.x} , {size.y}");

        //�κ��丮 ������ ����
        List<DB_ItemUnit> items = Managers.Web.Models.Inventory;
        if (items == null)
        {
            return;
        }
        Managers.SystemLog.Message($"itemsCount = {items.Count}");
        foreach (var unit in items)
        {
            Managers.SystemLog.Message($"itemId = {unit.attributes.item_id}");
            ItemData itemData = new ItemData();
            itemData.SetItemData(unit);

            ItemObject invenItem = Managers.Resource.Instantiate("UI/InvenUI/ItemUI", infoGrid.transform).GetComponent<ItemObject>();
            invenItem.SetItem(itemData);
            invenItem.RevealItem();

            instantItemObjs.Add(invenItem);
            infoGrid.UpdateItemPosition(invenItem, invenItem.itemData.pos.x, invenItem.itemData.pos.y);
            Managers.SystemLog.Message($"pos = {invenItem.itemData.pos.x}, {invenItem.itemData.pos.y}");
            Managers.SystemLog.Message($"Item = {itemData.itemId} size = {itemData.width},{itemData.height}, amount = {itemData.amount}");
        }
    }


    public override void OnRegister()
    {
        quickInfoUI.SetActive(false);
        quickResetBtn.gameObject.SetActive(false);
    }

    public override void OnUnRegister()
    {
        if (quickInfoBtn != null)
        {
            quickInfoBtn.onClick.RemoveListener(OnClickQuickInfo);
        }

        if (quickResetBtn != null)
        {
            quickResetBtn.onClick.RemoveListener(OnClickQuickReset);
        }
    }

    /// <summary>
    /// ���� ������ ��� �������� ���Ŀ� ������ �����Ѵٸ� ����ҿ��� �������� �ҷ��� ä���
    /// 
    /// ������ ���߿� ���� ��ȹ ����
    /// </summary>
    private void OnClickQuickInfo()
    {
        if (true == isProcess)
        {
            return;
        }
        isProcess = true;

        ClientCredential crediential = Managers.Web.Models.Credential;
        if (crediential == null)
        {
            return;
        }

        var header = new HeaderVerfiyPlayer
        {
            uid = crediential.uid.ToString(),
            access_token = crediential.access_token,
        };

        var body = new LoadStorageReq
        {

        };


        GsoWebService service = new GsoWebService();
        LoadStorageRequest request = service.mGameResource.GetLoadStorageRequest(header, body);
        request.ExecuteAsync(OnProcessLoadStorage);
    }

    /// <summary>
    /// ��� �ִ� ������ ������
    /// </summary>
    private void OnProcessLoadStorage(LoadStorageRes response)
    {

        isProcess = false;

        if (response.error_code != WebErrorCode.None)
        {
            return;
        }
        Managers.Web.Models.Gear = response.gears;
        Managers.Web.Models.Inventory = response.items;

        LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.QuickInfo);
    }

    private void OnClickQuickReset()
    {
        if (true == isProcess)
        {
            return;
        }
        isProcess = true;

        ClientCredential crediential = Managers.Web.Models.Credential;
        if (crediential == null)
        {
            return;
        }

        var header = new HeaderVerfiyPlayer
        {
            uid = crediential.uid.ToString(),
            access_token = crediential.access_token,
        };

        var body = new ResetStorageReq
        {
            
        };


        GsoWebService service = new GsoWebService();
        ResetStorageRequest request = service.mGameResource.GetResetStorageRequest(header, body);
        request.ExecuteAsync(OnProcessResetStorage);
    }

    private void OnClickQuickClose()
    {
        if (true == quickInfoUI.activeSelf)
        {
            quickInfoUI.SetActive(false);
            quickResetBtn.gameObject.SetActive(false);
            return;
        }
    }

    /// <summary>
    /// �����(����)�� �ִ� ������ ������
    /// </summary>
    private void OnProcessResetStorage(ResetStorageRes response)
    {

        isProcess = false;

        if (response.error_code != WebErrorCode.None)
        {
            return;
        }

        Managers.Web.Models.Gear = response.gears;
        Managers.Web.Models.Inventory = response.items;

        LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.QuickInfo);

    }


}
