using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.Models.GameDB;

public class UI_Metadata : LobbyUI
{
    protected override ELobbyUI type => ELobbyUI.Metadata;

    [SerializeField]
    private Button closeButton;

    /// <summary>
    /// Metadata
    /// </summary>
    [SerializeField]
    private TMP_Text totalGames;

    [SerializeField]
    private TMP_Text kills;

    [SerializeField]
    private TMP_Text deaths;

    [SerializeField]
    private TMP_Text damage;

    [SerializeField]
    private TMP_Text farming;

    [SerializeField]
    private TMP_Text escape;

    [SerializeField]
    private TMP_Text survivalTime;

    /// <summary>
    /// Skills
    /// </summary>
    [SerializeField]
    private TMP_Text rating;

    [SerializeField]
    private TMP_Text deviation;

    [SerializeField]
    private TMP_Text volatility;

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

    }

    public override void UpdateUI()
    {
        UserMetadataInfo metadata = Managers.Web.user.MetadataInfo;
        if (metadata == null)
        {
            return;
        }

        totalGames.text     = metadata.total_games.ToString();
        kills.text          = (metadata.kills == 0) ? "0" : (metadata.kills / metadata.total_games).ToString();
        deaths.text         = (metadata.kills == 0) ? "0" : (metadata.deaths / metadata.total_games).ToString();
        damage.text         = (metadata.kills == 0) ? "0" : (metadata.damage / metadata.total_games).ToString();
        farming.text        = (metadata.kills == 0) ? "0" : (metadata.farming / metadata.total_games).ToString();
        escape.text         = (metadata.kills == 0) ? "0" : (metadata.escape / metadata.total_games).ToString();
        survivalTime.text   = (metadata.kills == 0) ? "0" : (metadata.survival_time / metadata.total_games).ToString();

        UserSkillInfo skill = Managers.Web.user.SkillInfo;
        if(skill == null)
        {
            return;
        }

        rating.text         = skill.rating.ToString();
        deviation.text      = skill.deviation.ToString();
        volatility.text     = skill.volatility.ToString();
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
