using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UserSkill : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI rating;

    [SerializeField]
    private TextMeshProUGUI deviation;

    [SerializeField]
    private TextMeshProUGUI volatility;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UserSkillInfo info = Managers.Web.mUserInfo.SkillInfo;
        //rating.text = info.rating.ToString();
        //deviation.text = info.deviation.ToString();
        //volatility.text = info.volatility.ToString();
    }
}
