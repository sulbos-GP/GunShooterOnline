using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EHubConnection
{
    Matching = 0,
}

public class WebManager : MonoBehaviour
{
    private static WebManager instance = null;

    public WebClientCredential mCredential;

    //юс╫ц
    public DataLoadUserInfo mUserInfo;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static WebManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
    }

}
