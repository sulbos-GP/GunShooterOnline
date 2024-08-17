using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text Health;
    //TO-DO : 로컬 플레이어 생길 시 제거
    public Unit LocalUnit => UnitManager.Instance.CurrentPlayer;

    public TMP_Text AmmoText;
    public Gun Gun => LocalUnit.GetComponentInChildren<Gun>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Managers.Object.MyPlayer == null)
            return;
        float MaxHP = Managers.Object.MyPlayer.MaxHp;
        float HP = Managers.Object.MyPlayer.Hp;
        //HP
         Health.text =  HP+" / "+MaxHP;
        //GunAmmo
        AmmoText.text = Gun.getGunStat().name+" :: "+Gun._curAmmo.ToString();
    }
}
