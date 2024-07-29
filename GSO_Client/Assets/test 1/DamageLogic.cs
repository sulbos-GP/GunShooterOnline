using NPOI.OpenXmlFormats.Wordprocessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public static  class DamageLogic
{
    public static void DamageApply(Unit Owner,Unit Receive)
    {
        int damage = Owner.GetComponentInChildren<Gun>().getGunStat().damage;
        TakeDamage(damage,Receive);
    }

    public static void TakeDamage(int damage, Unit Receive)
    {
        int health = Receive._health;
        if(health-damage > 0) 
        {
            Receive.SetHealth(-damage);
            Debug.Log("Calc Damage Logic");
        }
        else
        {
            Receive.SetHealth(-health);
            Debug.LogError("Die Unit : "+Receive.gameObject.name);
        }
        
    }
}
