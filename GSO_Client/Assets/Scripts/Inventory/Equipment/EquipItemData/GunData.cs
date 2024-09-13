using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GunData
{
    public int item_id;
    public int range;          // �߻� ����(Ŭ���� ��Ȯ�� �ٿ�)
    public int damage;         // ������
    public int distance;       // ��Ÿ�
    public int reload_round;   // ������ �Ǵ� ź�� ��
    public int attack_speed;   // ���� �ӵ�
    public int reload_time;    // ������ �ð� 
    public int bulletId;  // ź�� ����
}

//�� �����ͺ��̽��� ������ ����
public class GunDB
{
    public static Dictionary<int, GunData> gunDB = new Dictionary<int, GunData>();

    static GunDB()
    {
        GunDataInit();
    }


    private static GunData colt45 = new GunData
    {
        item_id = 101,
        range = 8,
        damage = 10,
        distance = 6,
        reload_round = 8,
        attack_speed = 8,
        reload_time = 2,
        bulletId = 501
    };

    private static GunData ak47 = new GunData
    {
        item_id = 102,
        range = 20,
        damage = 18,
        distance = 10,
        reload_round = 40,
        attack_speed = 20,
        reload_time = 4,
        bulletId = 502
    };

    private static GunData aug = new GunData
    {
        item_id = 103,
        range = 8,
        damage = 10,
        distance = 6,
        reload_round = 8,
        attack_speed = 21,
        reload_time = 4,
        bulletId = 502
    };

    public static void GunDataInit()
    {
        gunDB.Clear();
        gunDB.Add(colt45.item_id, colt45);
        gunDB.Add(ak47.item_id, ak47);
        gunDB.Add(aug.item_id, aug);
    }

    public static GunData GetGunData(int itemId)
    {
        if (gunDB.ContainsKey(itemId))
        {
            return gunDB[itemId];
        }
        else
        {
            return null; // �������� �ʴ� ��� null ��ȯ
        }
    }
}


