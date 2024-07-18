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
    public float range;       // ��Ÿ�.
    public float fireRate;    // ���� �ӵ�. ���� �ӵ��� �ݺ��
    public int ammo;          // ��ź��(������ź)
    public float accuracy;    // �߻� ����(��Ȯ��)
    public int damage;        // ������
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
