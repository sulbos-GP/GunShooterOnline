using MathNet.Numerics.Distributions;
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

public class UI_QuckInfo : LobbyUI
{
    protected override ELobbyUI type => ELobbyUI.QuickInfo;

    [SerializeField]
    private Button quickInfoBtn;

    [SerializeField]
    private Button quickResetBtn;

    [SerializeField]
    private GameObject quickInfoUI;

    private bool isProcess;

    private void Awake()
    {
        quickInfoBtn.onClick.AddListener(OnClickQuickInfo);
        quickResetBtn.onClick.AddListener(OnClickQuickReset);
    }

    public override void InitUI()
    {
        isProcess = false;
    }

    public override void UpdateUI()
    {
        quickResetBtn.gameObject.SetActive(true);
        quickInfoUI.SetActive(true);

        List<DB_GearUnit> gears = Managers.Web.Models.Gear;
        if (gears == null)
        {
            return;
        }

        foreach (var unit in gears)
        {
            Managers.SystemLog.Message($"Part : {unit.gear.part}");
        }

        List<DB_ItemUnit> items = Managers.Web.Models.Inventory;
        if (items == null)
        {
            return;
        }

        foreach (var unit in items)
        {
            Managers.SystemLog.Message($"Item : {unit.attributes.item_id}");
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
    /// 현재 착용한 장비를 가져오고 이후에 가방이 존재한다면 저장소에서 아이템을 불러와 채운다
    /// 
    /// 스탯은 나중에 아직 계획 없음
    /// </summary>
    private void OnClickQuickInfo()
    {

        if(true == isProcess)
        {
            return;
        }
        isProcess = true;

        if (true == quickInfoUI.activeSelf)
        {
            quickInfoUI.SetActive(false);
            quickResetBtn.gameObject.SetActive(false);
            return;
        }

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
    /// 장비에 있는 아이템 정보들
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

    /// <summary>
    /// 저장소(가방)에 있는 아이템 정보들
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
