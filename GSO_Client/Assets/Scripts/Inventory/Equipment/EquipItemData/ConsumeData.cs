using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum EEffect
{
    None,
    Immediate,
    Buff,
}

public class ConsumeData
{
    public int item_id;
    public int energe;
    public double active_time;
    public double duration;
    public EEffect effect;
    public double cooltime;
}

public class ConsumeDB
{
    public static Dictionary<int, ConsumeData> consumeDB = new Dictionary<int, ConsumeData>();

    static ConsumeDB()
    {
        ConsumeDBInit();
    }


    private static ConsumeData medikit = new ConsumeData
    {
        item_id = 401,
        energe = 70,
        active_time = 0,
        duration = 0,
        effect = EEffect.Immediate,
        cooltime = 4,

    };

    private static ConsumeData band = new ConsumeData
    {
        item_id = 402,
        energe = 10,
        active_time = 0,
        duration = 0,
        effect = EEffect.Immediate,
        cooltime = 2,

    };

    private static ConsumeData adrenaline = new ConsumeData
    {
        item_id = 403,
        energe = 3,
        active_time = 2,
        duration = 40,
        effect = EEffect.Buff,
        cooltime = 4,

    };

    private static ConsumeData pill = new ConsumeData
    {
        item_id = 404,
        energe = 1,
        active_time = 2,
        duration = 40,
        effect = EEffect.Buff,
        cooltime = 2,

    };

    public static void ConsumeDBInit()
    {
        consumeDB.Clear();
        consumeDB.Add(medikit.item_id, medikit);
        consumeDB.Add(band.item_id, band);
        consumeDB.Add(adrenaline.item_id, adrenaline);
        consumeDB.Add(pill.item_id, pill);

    }

    public static ConsumeData GetConsumeData(int itemId)
    {
        if (consumeDB.ContainsKey(itemId))
        {
            return consumeDB[itemId];
        }
        else
        {
            return null; // 존재하지 않는 경우 null 반환
        }
    }
}
