using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.Models.GameDB;

public class UI_Metadata : LobbyUI
{
    protected override ELobbyUI type => ELobbyUI.Metadata;

    /// <summary>
    /// Metadata
    /// </summary>
    [SerializeField]
    private TMP_Text total_games;

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
    private TMP_Text survival_time;

    /// <summary>
    /// Skills
    /// </summary>
    [SerializeField]
    private TMP_Text rating;

    [SerializeField]
    private TMP_Text deviation;

    [SerializeField]
    private TMP_Text volatility;

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

        total_games.text    = metadata.total_games.ToString();
        kills.text          = (metadata.kills / metadata.total_games).ToString();
        deaths.text         = (metadata.deaths / metadata.total_games).ToString();
        damage.text         = (metadata.damage / metadata.total_games).ToString();
        farming.text        = (metadata.farming / metadata.total_games).ToString();
        escape.text         = (metadata.escape / metadata.total_games).ToString();
        survival_time.text  = (metadata.survival_time / metadata.total_games).ToString();

        UserSkillInfo skill = Managers.Web.user.SkillInfo;
        if(skill == null)
        {
            return;
        }

        rating.text         = skill.rating.ToString();
        deviation.text      = skill.deviation.ToString();
        volatility.text     = skill.volatility.ToString();
    }
}
