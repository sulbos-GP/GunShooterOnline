using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Models.GameDatabase;
using static UserResource;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Error;
using Google.Protobuf.Protocol;
using System.Linq;

public class UI_Quest : MonoBehaviour
{

    private Dictionary<int, GameObject> contents = new Dictionary<int, GameObject>();

    [SerializeField]
    private GameObject questContentPrefab;

    [SerializeField]
    private Transform contentParent;

    public void Awake()
    {
        
    }

    public void InitQuest(List<PS_RegisterQuest> quests)
    {
        contents.Clear();
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (PS_RegisterQuest quest in quests)
        {
            GameObject prefab = Instantiate(questContentPrefab, contentParent);

            var questContent = prefab.GetComponentInChildren<UI_QuestContent>();
            if (questContent == null)
            {
                return;
            }
            questContent.UpdateQuestData(quest.Id, quest.Progress, quest.Completed);

            contents.Add(quest.Id, prefab);
        }
    }

    public void UpdateQuest(int quest_id, int progress, bool complete)
    {

        {
            contents.TryGetValue(quest_id, out var prefab);
            if (prefab == null)
            {
                return;
            }

            var questContent = prefab.GetComponentInChildren<UI_QuestContent>();
            if (questContent == null)
            {
                return;
            }
            questContent.UpdateQuestData(quest_id, progress, complete);
        }

        {
            var quests = Managers.Web.Models.DailyQuestData;
            if (quests == null)
            {
                return;
            }

            var registerQuest = quests.FirstOrDefault(quest => quest.quest_id == quest_id);
            if(registerQuest == null)
            {
                return;
            }

            registerQuest.progress = progress;
            registerQuest.completed = complete;

        }

    }
}
