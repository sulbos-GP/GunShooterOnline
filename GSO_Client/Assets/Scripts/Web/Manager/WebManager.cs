using System.Collections;
using UnityEngine;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using static AuthorizeResource;

public class WebManager
{
    private WebModels models = new WebModels();
    private ClientCredential credential = new ClientCredential();
    private DataLoadUserInfo user = new DataLoadUserInfo();

    private int heartbeatDelay = 30;

    private void Awake()
    {

    }

    public WebModels Models
    {
        get
        {
            return models;
        }
        set
        {
            models = value;
        }
    }

    public void StartHearbeat()
    {
        //Managers.Instance.StartCoroutine("PostHearbeat");
    }

    public void StopHearbeat()
    {
        //Managers.Instance.StopCoroutine("PostHearbeat");
    }

    private IEnumerator PostHearbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(heartbeatDelay);

            //하트비트 보내기
        }
    }

    /// <summary>
    /// 현재 클라이언트가 나갔는지 복귀했는지를 웹 서버에게 보낸다
    /// 하트비트가 멈출 가능성이 있기 때문에
    /// </summary>
    public void OnPuase(bool isPuase)
    {
        //현재 상태를 웹 서버에게 전달

        //하트비트 보내기
        if(isPuase)
        {
            StopHearbeat();
        }
        else
        {
            StartHearbeat();
        }
    }

}
