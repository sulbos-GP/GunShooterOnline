using System.Collections.Generic;
using UnityEngine;


public class DefensiveData
{
    public int item_id;            //마스터 db의 아이템 아이디
    public int durability;        //방어구의 내구도. 1회피격당 5의 내구도 감소
    public int defensive_power;   //해당 방어구의 데미지. 피격 데미지 = 데미지의 방어력% 한 값 ex) 데미지30, 방어력20일 경우 데미지 = 26
}

public class DefensiveDB
{
    public static Dictionary<int, DefensiveData> defensiveDB = new Dictionary<int, DefensiveData>();

    static DefensiveDB()
    {
        DefensiveDBInit();
    }


    public static DefensiveData police_vest = new DefensiveData
    {
        item_id = 201,
        durability = 100,
        defensive_power = 5
    };

    public static DefensiveData bulletProof_vest = new DefensiveData
    {
        item_id = 202,
        durability = 100,
        defensive_power = 10
    };

    public static DefensiveData army_vest = new DefensiveData
    {
        item_id = 203,
        durability = 100,
        defensive_power = 15
    };



    public static void DefensiveDBInit()
    {
        defensiveDB.Clear();
        defensiveDB.Add(police_vest.item_id, police_vest);
        defensiveDB.Add(bulletProof_vest.item_id, bulletProof_vest);
        defensiveDB.Add(army_vest.item_id, army_vest);

    }

    public static DefensiveData GetDefensiveInit(int itemId)
    {
        if (defensiveDB.ContainsKey(itemId))
        {
            return defensiveDB[itemId];
        }
        else
        {
            return null; // 존재하지 않는 경우 null 반환
        }
    }
}
