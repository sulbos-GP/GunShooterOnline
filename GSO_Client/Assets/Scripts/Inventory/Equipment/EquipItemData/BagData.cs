using System.Collections.Generic;

public class BagData
{
    public int item_id;
    public int total_scale_x;
    public int total_scale_y;
    public double total_weight;
}


public class BagDB
{
    public static Dictionary<int, BagData> bagDB = new Dictionary<int, BagData>();

    static BagDB()
    {
        BagDBInit();
    }

    private static BagData none = new BagData
    {
        item_id = -1,
        total_scale_x = 2,
        total_scale_y = 3,
        total_weight = 5
    };

    private static BagData medibag = new BagData
    {
        item_id = 301,
        total_scale_x = 3,
        total_scale_y = 4,
        total_weight = 10
    };

    private static BagData armybag = new BagData
    {
        item_id = 302,
        total_scale_x = 5,
        total_scale_y = 5,
        total_weight = 15
    };

    private static BagData doublebag = new BagData
    {
        item_id = 303,
        total_scale_x = 6,
        total_scale_y = 7,
        total_weight = 20
    };


    public static void BagDBInit()
    {
        bagDB.Clear();
        bagDB.Add(none.item_id, none);
        bagDB.Add(medibag.item_id, medibag);
        bagDB.Add(armybag.item_id, armybag);
        bagDB.Add(doublebag.item_id, doublebag);
    }

    public static BagData GetConsumeData(int itemId)
    {
        if (bagDB.ContainsKey(itemId))
        {
            return bagDB[itemId];
        }
        else
        {
            return null; // 존재하지 않는 경우 null 반환
        }
    }
}


