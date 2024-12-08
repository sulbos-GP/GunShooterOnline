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
    private Button quickResetBtn;

    [SerializeField]
    private GameObject quickInfoUI;

    private void Awake()
    {
        quickInfoUI.SetActive(false);
        quickResetBtn.gameObject.SetActive(false);
        quickInfoBtn.onClick.AddListener(OnClickQuickInfo);
        quickResetBtn.onClick.AddListener(OnClickQuickReset);
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
    /// ��� �ִ� ������ ������
    /// </summary>
    private void OnProcessLoadStorage(LoadStorageRes response)
    {

        if(response.error_code != WebErrorCode.None)
        {
            return;
        }

        foreach (var unit in response.gears)
        {
            Debug.Log($"Part : {unit.gear.part}");
        }

        foreach (var unit in response.items)
        {
            Debug.Log($"Item : {unit.attributes.item_id}");
        }

        quickResetBtn.gameObject.SetActive(true);
        quickInfoUI.SetActive(true);    //��� ������ �ҷ��Դٸ� ����â ���ֱ�
    }

    private void OnClickQuickReset()
    {
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
    /// �����(����)�� �ִ� ������ ������
    /// </summary>
    private void OnProcessResetStorage(ResetStorageRes response)
    {
        if (response.error_code != WebErrorCode.None)
        {
            return;
        }

        OnClickQuickInfo();

    }


}
