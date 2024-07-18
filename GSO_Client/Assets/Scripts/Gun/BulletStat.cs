using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New BulletStat", menuName = "GunStat/new BulletStat")]
public class BulletStat : ScriptableObject
{
    public float speed; 
    public GunType bulletType;

    public BulletStat(float speed, GameObject bulletObj, GunType bulletType)
    {
        this.speed = speed;
        this.bulletType = bulletType;
    }

    public void PrintBulletStatInfo()
    {
        Debug.Log($" Speed: {speed}, " +
                  $" BulletType: {bulletType}");
    }
}
