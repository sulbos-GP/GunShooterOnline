using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Match : MonoBehaviour
{
    [SerializeField]
    private Button mMatchButton;

    [SerializeField]
    private TextMeshProUGUI mMatchText;

    private bool mIsJoin = false;

    private void Awake()
    {
        mMatchButton.onClick.AddListener(OnMatchJoin);
        mMatchText.text = "시작하기";
    }

    void Start()
    {
        //임시
        //나중에 로비에 입장할떄마다 연결 할 수 있도록



    }

    private void OnDestroy()
    {
        //임시
        //로비에 나갈때마다 나가지게 할 수 있도록 해야함

    }

    private void OnMatchJoin()
    {
        mMatchText.text = "취소하기";




    }

    private void OnMatchCancle()
    {
        mMatchText.text = "시작하기";
    }

}
