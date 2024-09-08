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
        //�ӽ� �׽�Ʈ
        var tempList = new List<DB_RewardLevel>
        {
            new DB_RewardLevel
            {
                reward_id = 10001,
                level = 1,
                experience = 100,
                name = "1000���",
                icon = "IconS_goldbar"
            },

            new DB_RewardLevel
            {
                reward_id = 10002,
                level = 2,
                experience = 200,
                name = "Ƽ�� 2��",
                icon = "IconS_solidwood"
            },

            new DB_RewardLevel
            {
                reward_id = 10003,
                level = 3,
                experience = 300,
                name = "�̱� 5��",
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
            Debug.Log("���� ������ �������� �ʰų� ������ 0�Դϴ�.");
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
                Debug.Log($"���� [{reward.reward_id}]�� �������� �������� �ʽ��ϴ�.");
            }
        }
    }
}
