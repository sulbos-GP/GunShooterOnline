using NPOI.OpenXmlFormats.Wordprocessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public static  class DamageLogic
{
    //데미지 계산
    public static void DamageApply(Unit Owner,Unit Receive)
    {
        //TO-DO : 추후에 데미지 계산 식 추가 예정
        int damage = Owner.GetComponentInChildren<Gun>().getGunStat().damage;
        TakeDamage(damage,Receive);
    }

    //데미지 적용
    public static void TakeDamage(int damage, Unit Receive)
    {
        int health = Receive.unitStat.GetStat(eSTAT.Health);
        if(health-damage > 0) 
        {
            Receive.unitStat.Add(eSTAT.Health,-damage);
            Debug.Log("Calc Damage Logic");
        }
        else
        {
            //TO-DO : Die Logic 구성
            Receive.unitStat.SetStat(eSTAT.Health, 0);
            Debug.LogError("Die Unit : "+Receive.gameObject.name);
        }
        
    }
}
