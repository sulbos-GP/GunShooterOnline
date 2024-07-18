using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Pistol,
    AssultRifle,
    ShotGun,
    Sniper
}


[CreateAssetMenu(fileName = "New GunStat", menuName = "GunStat/new GunStat")]
public class GunStat : ScriptableObject
{
    public float range;       // 사거리.
    public float fireRate;    // 연사 속도. 값과 속도가 반비례
    public int ammo;          // 장탄수(보유장탄)
    public float accuracy;    // 발사 각도(정확도)
    public int damage;        // 데미지
    public GameObject bulletObj;
    public GunType gunType;

    public GunStat(float range,float fireRate, int ammo, float accuracy, int damage, GameObject bulletObj, GunType gunType)
    {
        this.range = range;
        this.fireRate = fireRate;
        this.ammo = ammo;
        this.accuracy = accuracy;
        this.damage = damage;
        this.bulletObj = bulletObj;
        this.gunType = gunType;
    }

    public void PrintGunStatInfo()
    {
        Debug.Log($" Range: {range}, " +
                  $" Fire Rate: {fireRate}," +
                  $" Ammo: {ammo}, " +
                  $" Accuracy: {accuracy}," +
                  $" Damage: {damage}," +
                  $" GunType: {gunType}");
    }
}
