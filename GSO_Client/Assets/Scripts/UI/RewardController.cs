using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Google.Protobuf.Protocol;

public enum RewardType
{
    LevelUp = 0,
    RoomClear = 1,
    KillPlayer = 2,
}


public class RewardController : MonoBehaviour
{
    [SerializeField]
    private RewardType _rewardType;

    [SerializeField] 
    private List<GameObject> Rewards;
    

    public void SetReward(RewardType rewardType)
    {
        _rewardType = rewardType;
        GameObject go = Rewards.Find(r => r.name.Contains(nameof(rewardType)));
        if (go == null)
        {
            Debug.LogError("리스트 할당안함");
            return;
        }
            
        go.SetActive(true);
        go.GetComponent<RectTransform>().position = Vector3.zero;
        


    }

    


}
