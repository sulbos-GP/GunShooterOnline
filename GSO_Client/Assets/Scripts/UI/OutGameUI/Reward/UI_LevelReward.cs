using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;


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
        foreach (var reward in Data_master_reward_level.AllData())
        {
            GameObject prefab = Instantiate(rewardPrefab, contentParent);
            prefab.GetComponentInChildren<UI_LevelRewardData>().InitLevelRewardData(reward.Value);
            contents.Add(reward.Key, prefab);
        }
    }

    public override void UpdateUI()
    {
        List<FUserLevelReward> rewards = Managers.Web.Models.LevelReward;
        if (rewards == null || rewards.Count == 0)
        {
            return;
        }

        foreach (FUserLevelReward reward in rewards)
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
