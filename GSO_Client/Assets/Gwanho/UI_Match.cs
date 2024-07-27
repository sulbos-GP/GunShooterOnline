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
        mMatchText.text = "�����ϱ�";
    }

    void Start()
    {
        //�ӽ�
        //���߿� �κ� �����ҋ����� ���� �� �� �ֵ���



    }

    private void OnDestroy()
    {
        //�ӽ�
        //�κ� ���������� �������� �� �� �ֵ��� �ؾ���

    }

    private void OnMatchJoin()
    {
        mMatchText.text = "����ϱ�";




    }

    private void OnMatchCancle()
    {
        mMatchText.text = "�����ϱ�";
    }

}
