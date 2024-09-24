using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text Health;
    public TMP_Text AmmoText;
    public GameObject DieUI;
    public float Duration = 2.0f;

    private bool isDie;
    private bool _init;
    private float _alphaTime = 3f;

    private Button reloadBtn;
    private MyPlayerController myPlayer;
    private Gun gun;
    void Start()
    {
        isDie = false;
        _init = false;
    }

    // TO-DO : 추후에 변경점 생길 시 데이터 변경으로 바꿔야 함.
    void Update()
    {
        if (Managers.Object.MyPlayer == null || isDie)
            return;
        if(!_init)
            Init();
        if(myPlayer.Hp<=0)
        {
            isDie = true;
            DieUI.SetActive(isDie);
            StartCoroutine(TextAlpha(DieUI.GetComponent<CanvasGroup>(), Duration));
            
        }
        float MaxHP = myPlayer.MaxHp;
        float HP = myPlayer.Hp;
        //HP
        Health.text =  HP+" / "+MaxHP;
        //GunAmmo
        Gun playerGun = myPlayer.transform.Find("Pivot/Gun").GetComponentInChildren<Gun>();
        if (playerGun.CurGunState == GunState.Reloading)
            AmmoText.text = "ReloadCoroutine Gun";
        else
        {
            if(playerGun.CurGunData == null)
            {
                AmmoText.text = "Gun is not Equipped";
            }
            else
            {
                AmmoText.text = playerGun.CurAmmo.ToString() + " / " + playerGun.getGunStat().reload_round.ToString();
            }
        }
           

        //Reload
        if (gun.CurGunState == GunState.Reloading)
            reloadBtn.interactable = false;
        else
            reloadBtn.interactable = true;
    }

    private void Init()
    {
        _init = true;

        myPlayer = Managers.Object.MyPlayer;
        
        foreach (Transform child in transform)
        {
            if (child.name == "Reload")
            {
                reloadBtn = child.GetComponent<Button>();
                break;
            }
        }
        gun = myPlayer.transform.Find("Pivot/Gun").GetComponent<Gun>();
        reloadBtn.onClick.AddListener(gun.Reload);

    }
    private IEnumerator TextAlpha(CanvasGroup group,float duration)
    {
        float elapsedTime = 0f;

        while(elapsedTime<duration)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return null;
        }

        group.alpha = 1f;
    }

    public void LobbyScene()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene("Lobby");
#else
        SceneManager.LoadScene("Shelter");
#endif

    }
}
