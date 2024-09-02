using System.Collections;
using System.Collections.Generic;
using UnityEngine;




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
    public int reloadTime;
}
