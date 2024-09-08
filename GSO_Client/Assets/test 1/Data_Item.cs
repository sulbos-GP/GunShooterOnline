using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebCommonLibrary.Models.MasterDB;

public class Data_Item : BaseData<Data_Item>
{
    public string code;
    public string name;
    public float weight;
    public eITEM_TYPE type;
    public int description;
    public int scale_x;
    public int scale_y;
    public int purchase_price;
    public float inquiry_time;
    public int sell_price;
    public int amount;
    public string icon;
}

public class Data_RewardBase : BaseData<Data_RewardBase>
{
    public DB_RewardBase data;
}

public class Data_RewardLevel : BaseDataMulti<Data_RewardLevel>
{
    public DB_RewardLevel data;
}

public class Data_RewardBox : BaseData<Data_RewardBox>
{
    public DB_RewardBox data;
}