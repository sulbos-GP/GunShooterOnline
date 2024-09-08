using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDB;

public class UI_LevelReward : LobbyUI
{
    protected override ELobbyUI type => ELobbyUI.LevelReward;

    private Dictionary<int, GameObject> contents = new Dictionary<int, GameObject>();

    [SerializeField]
    private GameObject rewardPrefab;

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private Button closeButton;

    public void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButton);
    }

    public void OnCloseButton()
    {
        this.gameObject.SetActive(false);
    }

    public override void InitUI()
    {
        //임시 테스트
        var tempList = new List<DB_RewardLevel>
        {
            new DB_RewardLevel
            {
                reward_id = 10001,
                level = 1,
                experience = 100,
                name = "1000골드",
                icon = "IconS_goldbar"
            },

            new DB_RewardLevel
            {
                reward_id = 10002,
                level = 2,
                experience = 200,
                name = "티켓 2장",
                icon = "IconS_solidwood"
            },

            new DB_RewardLevel
            {
                reward_id = 10003,
                level = 3,
                experience = 300,
                name = "뽑기 5장",
                icon = "IconS_battery"
            }
        };
        IEnumerable<DB_RewardLevel> rewards = tempList;

        foreach (DB_RewardLevel reward in rewards)
        {
            GameObject prefab = Instantiate(rewardPrefab, contentParent);
            prefab.GetComponentInChildren<UI_LevelRewardData>().InitLevelRewardData(reward);
            contents.Add(reward.reward_id, prefab);
        }

        this.gameObject.SetActive(false);

    }

    public override void UpdateUI()
    {
        List<UserLevelReward> rewards = Managers.Web.user.LevelReward;
        if (rewards == null || rewards.Count == 0)
        {
            Debug.Log("레벨 보상이 존재하지 않거나 개수가 0입니다.");
            return;
        }

        foreach (UserLevelReward reward in rewards)
        {
            contents.TryGetValue(reward.reward_id, out var prefab);

            if(prefab != null)
            {
                prefab.GetComponentInChildren<UI_LevelRewardData>().UpdateLevelRewardData(reward);
            }
            else
            {
                Debug.Log($"레벨 [{reward.reward_id}]의 프리펩이 존재하지 않습니다.");
            }
        }
    }
}
