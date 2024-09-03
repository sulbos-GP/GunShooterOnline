using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TMP_Text Health;
    public TMP_Text AmmoText;
    public GameObject DieUI;
    public float Duration = 2.0f;

    private bool isDie;
    private float _alphaTime = 3f;
    void Start()
    {
        isDie = false;
    }

    // TO-DO : 추후에 변경점 생길 시 데이터 변경으로 바꿔야 함.
    void Update()
    {
        if (Managers.Object.MyPlayer == null)
            return;
        if (isDie)
            return;
        if(Managers.Object.MyPlayer.Hp<=0)
        {
            isDie = true;
            DieUI.SetActive(isDie);
            StartCoroutine(TextAlpha(DieUI.GetComponent<CanvasGroup>(), Duration));
            
        }
        float MaxHP = Managers.Object.MyPlayer.MaxHp;
        float HP = Managers.Object.MyPlayer.Hp;
        //HP
        Health.text =  HP+" / "+MaxHP;
        //GunAmmo
        Gun playerGun = Managers.Object.MyPlayer.GetComponentInChildren<Gun>();
        if (playerGun.gunState == GunState.Reloading)
            AmmoText.text = "Reloading Gun";
        else
            AmmoText.text = playerGun._curAmmo.ToString() + " / " + playerGun.getGunStat().ammo.ToString();
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
