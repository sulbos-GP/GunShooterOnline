using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebCommonLibrary.Models.MasterDB;

public class Data_master_item_base : BaseData<Data_master_item_base>
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

public class Data_master_item_backpack : BaseData<Data_master_item_backpack>
{
    public int total_scale_x;
    public int total_scale_y;
    public int total_weight;
}

public class Data_RewardBase : BaseData<Data_RewardBase>
{
    public int money;
    public int ticket;
    public int gacha;
    //TO-DO : 추후에 타입 변경 가능성 있음.
    public int reward_box_id;
}

public class Data_RewardLevel : BaseData<Data_RewardLevel>
{
    public int level;
    public int experience;
    public string name;
    public string icon;
}

public class Data_RewardBox : BaseData<Data_RewardBox>
{

}