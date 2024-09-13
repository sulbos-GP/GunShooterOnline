using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GunData
{
    public int item_id;
    public int range;          // 발사 각도(클수록 정확도 다운)
    public int damage;         // 데미지
    public int distance;       // 사거리
    public int reload_round;   // 재장전 되는 탄알 수
    public int attack_speed;   // 연사 속도
    public int reload_time;    // 재장전 시간 
    public int bulletId;  // 탄알 종류
}

//총 데이터베이스가 나오면 제거
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
            return null; // 존재하지 않는 경우 null 반환
        }
    }
}


