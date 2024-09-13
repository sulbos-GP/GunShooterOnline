using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using static MatchmakerResource;

public class UI_MatchSetup : LobbyUI
{

    protected override ELobbyUI type => ELobbyUI.MatchSetup;

    private Dictionary<string, GameObject> contents = new Dictionary<string, GameObject>();

    [SerializeField]
    private GameObject matchWorldPrefab;

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private TMP_Text setupText;

    [SerializeField]
    private Button setupButton;

    public void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnClickClose);
        }

        if (setupButton != null)
        {
            setupButton.onClick.AddListener(OnClickSetup);
            Init();
        }
    }

    public void Init()
    {
        setupText.text = "미 선택";
        setupButton.interactable = false;
    }

    public void OnClickClose()
    {
        Init();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 매칭 참여 요청
    /// </summary>
    public void OnClickSetup()
    {
        GameObject matchingHub = GameObject.Find("@MatchmakerHub");
        if (matchingHub == null)
        {
            return;
        }
        MatchmakerHub hub = matchingHub.GetComponent<MatchmakerHub>();
        hub.OnJoinRequest(setupText.text);

        this.gameObject.SetActive(false);
    }

    public override void InitUI()
    {
        if(matchWorldPrefab == null)
        {
            matchWorldPrefab = Resources.Load<GameObject>("Prefabs/UI/Lobby/MatchWorld");
        }

        {
            string world = "Forest";
            Sprite sprite = Resources.Load<Sprite>($"Sprite/World/{world}");
            GameObject prefab = Instantiate(matchWorldPrefab, contentParent);
            prefab.GetComponentInChildren<UI_MatchWorld>().SetMatchWorld(world, sprite, HandleClickWorld);
            contents.Add(world, prefab);
        }

        {
            string world = "City";
            Sprite sprite = Resources.Load<Sprite>($"Sprite/World/{world}");
            GameObject prefab = Instantiate(matchWorldPrefab, contentParent);
            prefab.GetComponentInChildren<UI_MatchWorld>().SetMatchWorld(world, sprite, HandleClickWorld);
            contents.Add(world, prefab);
        }

        {
            string world = "MilitaryBase";
            Sprite sprite = Resources.Load<Sprite>($"Sprite/World/{world}");
            GameObject prefab = Instantiate(matchWorldPrefab, contentParent);
            prefab.GetComponentInChildren<UI_MatchWorld>().SetMatchWorld(world, sprite, HandleClickWorld);
            contents.Add(world, prefab);
        }

    }

    public void HandleClickWorld(string name)
    {
        setupText.text = name;
        setupButton.interactable = true;
    }

    public override void UpdateUI()
    {

    }

    public override void OnRegister()
    {
        this.gameObject.SetActive(false);
    }

    public override void OnUnRegister()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnClickClose);
        }

        if (setupButton != null)
        {
            setupButton.onClick.RemoveListener(OnClickSetup);
        }
    }
}
