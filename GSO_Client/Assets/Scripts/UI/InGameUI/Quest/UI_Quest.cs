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
    private GameObject questPanel;

    [SerializeField]
    private GameObject contentPanel;

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private Button toggleButton;

    [SerializeField]
    private Image toggleImage;

    private bool isToggleUp;

    public void Awake()
    {
        isToggleUp = true;

        toggleButton.onClick.AddListener(OnToggleButton);
    }

    public void OnToggleButton()
    {
        if (isToggleUp)
        {
            questPanel.transform.Translate(new Vector3(0.0f, 115.0f, 0.0f));

            questPanel.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 70);

            contentPanel.SetActive(false);
        }
        else
        {
            questPanel.transform.Translate(new Vector3(0.0f, -115.0f, 0.0f));

            questPanel.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);

            contentPanel.SetActive(true);
        }

        toggleImage.transform.Rotate(new Vector3(180.0f, 0.0f, 0.0f));
        isToggleUp = !isToggleUp;
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
