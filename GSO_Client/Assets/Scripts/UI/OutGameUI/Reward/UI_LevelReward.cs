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
        this.gameObject.SetActive(false);
        closeButton.onClick.AddListener(OnCloseButton);
    }

    public void OnCloseButton()
    {
        this.gameObject.SetActive(false);
    }

    public override void InitUI()
    {
        foreach (DB_RewardLevel reward in rewards)
        {
            GameObject prefab = Instantiate(rewardPrefab, contentParent);
            prefab.GetComponentInChildren<UI_LevelRewardData>().InitLevelRewardData(reward);
            contents.Add();
        }

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
            prefab.GetComponentInChildren<UI_LevelRewardData>().UpdateLevelRewardData(reward);
        }
    }
}
