using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillRequst : MonoBehaviour
{
    public GameObject Player;
    private Button button;
    private TMP_InputField textPro;

    private void Awake()
    {
        textPro = gameObject.GetComponentInChildren<TMP_InputField>();
        button = gameObject.GetComponentInChildren<Button>();

        button.onClick.AddListener(Requstskill);
    }

    public void Init()
    {
        button.onClick.AddListener(Requstskill);
    }

    public void Requstskill()
    {
        Debug.Log("Requstskill");
        Player.GetComponent<MyPlayerController>().UseSkill_Requst(int.Parse(textPro.text));
    }
}