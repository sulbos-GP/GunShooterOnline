using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using static AuthorizeResource;
using static GameResource;
using static UnityEngine.UI.CanvasScaler;

public class UI_QuckInfo : MonoBehaviour
{
    [SerializeField]
    private Button quickInfoBtn;

    [SerializeField]
    private GameObject quickInfoUI;

    private void Awake()
    {
        quickInfoUI.SetActive(false);
        quickInfoBtn.onClick.AddListener(OnClickQuickInfo);
    }

    /// <summary>
    /// ���� ������ ��� �������� ���Ŀ� ������ �����Ѵٸ� ����ҿ��� �������� �ҷ��� ä���
    /// 
    /// ������ ���߿� ���� ��ȹ ����
    /// </summary>
    private void OnClickQuickInfo()
    {

        if(true == quickInfoUI.activeSelf)
        {
            quickInfoUI.SetActive(false);
            return;
        }

        ClientCredential crediential = Managers.Web.Credential;
        if (crediential == null)
        {
            return;
        }

        var header = new HeaderVerfiyPlayer
        {
            uid = crediential.uid.ToString(),
            access_token = crediential.access_token,
        };

        var body = new LoadGearReq
        {

        };

        try
        {
            GsoWebService service = new GsoWebService();
            LoadGearRequest request = service.mGameResource.GetLoadGearRequest(header, body);
            request.ExecuteAsync(OnProcessLoadGear);
        }
        catch (Exception ex)
        {

        }

    }

    /// <summary>
    /// ��� �ִ� ������ ������
    /// </summary>
    private void OnProcessLoadGear(LoadGearRes response)
    {

        if(response.error_code != WebErrorCode.None)
        {
            return;
        }

        foreach (var unit in response.gears)
        {
            Debug.Log($"Part : {unit.gear.part}");
            if(unit.gear.part == "backpack")
            {

                ClientCredential crediential = Managers.Web.Credential;
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
                    storage_id = unit.attributes.unit_storage_id.Value,
                };

                try
                {
                    GsoWebService service = new GsoWebService();
                    LoadStorageRequest request = service.mGameResource.GetLoadStorageRequest(header, body);
                    request.ExecuteAsync(OnProcessLoadStorage);
                }
                catch (Exception ex)
                {

                }
            }
        }


    }

    /// <summary>
    /// �����(����)�� �ִ� ������ ������
    /// </summary>
    private void OnProcessLoadStorage(LoadStorageRes response)
    {
        if (response.error_code != WebErrorCode.None)
        {
            return;
        }
        foreach (var unit in response.items)
        {
            Debug.Log($"Item : {unit.attributes.item_id}");
        }

        quickInfoUI.SetActive(true);    //��� ������ �ҷ��Դٸ� ����â ���ֱ�
        

    }


}
