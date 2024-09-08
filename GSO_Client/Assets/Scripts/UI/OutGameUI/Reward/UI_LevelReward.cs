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
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnClickClose);
        }
    }

    public void OnClickClose()
    {
        this.gameObject.SetActive(false);
    }

    public override void InitUI()
    {

        Debug.Log("[UI_LevelReward:InitUI]" + Data_master_reward_base.GetData(10001));
        Debug.Log("[UI_LevelReward:InitUI]" + Data_master_reward_level.GetData(10001));

        if (Data_master_reward_level.AllData().Count == 0)
        {
            Debug.Log("[UI_LevelReward:InitUI] : RewardLevelData is null");
            return;
        }

        foreach (var reward in Data_master_reward_level.AllData())
        {
            GameObject prefab = Instantiate(rewardPrefab, contentParent);
            prefab.GetComponentInChildren<UI_LevelRewardData>().InitLevelRewardData(reward.Value);
            contents.Add(reward.Key, prefab);
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

    public override void OnRegister()
    {
        this.gameObject.SetActive(false);
    }

    public override void OnUnRegister()
    {
        if(closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnClickClose);
        }
    }

}
