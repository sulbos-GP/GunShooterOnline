using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AuthorizeResource;
using static GameResource;

public class UI_QuckInfo : MonoBehaviour
{
    [SerializeField]
    private Button quickInfoBtn;

    [SerializeField]
    private GameObject quickInfoUI;

    private void Awake()
    {
        quickInfoUI.SetActive(false);
        //quickInfoBtn.onClick.AddListener(OnClickQuickInfo);
    }

    //private void OnClickQuickInfo()
    //{
    //    var header = new HeaderVerfiyPlayer
    //    {
    //        uid = Managers.Web.mCredential.uid,
    //        access_token = Managers.Web.mCredential.access_token,
    //    };

    //    var packet = new QuickInfoReq()
    //    {

    //    };

    //    GsoWebService service = new GsoWebService();
    //    QuickInfoRequest request = service.mGameResource.GetQuickInfoRequest(header, packet);
    //    request.ExecuteAsync(OnProcessQuickInfo);
    //}

    //private void OnProcessQuickInfo(QuickInfoRes response)
    //{
    //    if (response == null)
    //    {
    //        return;
    //    }

    //    if (response.error_code == 0)
    //    {
    //        quickInfoUI.SetActive(true);



    //    }

    //}


}
