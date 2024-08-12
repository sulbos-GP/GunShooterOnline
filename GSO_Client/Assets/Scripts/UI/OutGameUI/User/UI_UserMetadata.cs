using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UserMetadata : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI total_games;

    [SerializeField]
    private TextMeshProUGUI kills;

    [SerializeField]
    private TextMeshProUGUI deaths;

    [SerializeField]
    private TextMeshProUGUI damage;

    [SerializeField]
    private TextMeshProUGUI farming;

    [SerializeField]
    private TextMeshProUGUI escape;

    [SerializeField]
    private TextMeshProUGUI survival_time;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UserMetadataInfo info = Managers.Web.mUserInfo.MetadataInfo;
        total_games.text = info.total_games.ToString();
        kills.text = info.kills.ToString();
        deaths.text = info.deaths.ToString();
        damage.text = info.damage.ToString();
        farming.text = info.farming.ToString();
        escape.text = info.escape.ToString();
        survival_time.text = info.survival_time.ToString();
    }
}
