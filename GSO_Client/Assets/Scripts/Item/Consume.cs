using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum EffectType
{
    Immediate,
    Buff
}

public class Consume
{
    public int item_id;
    public int energe;
    public double active_time;
    public double duration;
    public EffectType effect;
    public double cooltime;

    
}

public class ConsumeDB
{
    public static Dictionary<int, Consume> consumeDB = new Dictionary<int, Consume>();

    static ConsumeDB()
    {
        GunDataInit();
    }


    private static Consume medikit = new Consume
    {
        item_id = 401,
        energe = 70,
        active_time = 0,
        duration = 0,
        effect = EffectType.Immediate,
        cooltime = 4,

    };

    private static Consume band = new Consume
    {
        item_id = 402,
        energe = 10,
        active_time = 0,
        duration = 0,
        effect = EffectType.Immediate,
        cooltime = 2,

    };

    private static Consume adrenaline = new Consume
    {
        item_id = 403,
        energe = 3,
        active_time = 2,
        duration = 40,
        effect = EffectType.Buff,
        cooltime = 4,

    };

    private static Consume pill = new Consume
    {
        item_id = 404,
        energe = 1,
        active_time = 2,
        duration = 40,
        effect = EffectType.Buff,
        cooltime = 2,

    };

    public static void GunDataInit()
    {
        consumeDB.Clear();
        consumeDB.Add(medikit.item_id, medikit);
        consumeDB.Add(band.item_id, band);
        consumeDB.Add(adrenaline.item_id, adrenaline);
        consumeDB.Add(pill.item_id, pill);

    }

    public static Consume GetConsumeData(int itemId)
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
