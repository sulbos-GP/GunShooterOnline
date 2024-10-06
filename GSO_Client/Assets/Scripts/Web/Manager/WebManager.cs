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

            //��Ʈ��Ʈ ������
        }
    }

    /// <summary>
    /// ���� Ŭ���̾�Ʈ�� �������� �����ߴ����� �� �������� ������
    /// ��Ʈ��Ʈ�� ���� ���ɼ��� �ֱ� ������
    /// </summary>
    public void OnPuase(bool isPuase)
    {
        //���� ���¸� �� �������� ����

        //��Ʈ��Ʈ ������
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
