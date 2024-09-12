using System.Collections.Generic;
using UnityEngine;


public class DefensiveData
{
    public int item_id;            //������ db�� ������ ���̵�
    public int durability;        //���� ������. 1ȸ�ǰݴ� 5�� ������ ����
    public int defensive_power;   //�ش� ���� ������. �ǰ� ������ = �������� ����% �� �� ex) ������30, ����20�� ��� ������ = 26
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
            return null; // �������� �ʴ� ��� null ��ȯ
        }
    }
}
