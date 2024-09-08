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
    public int money { get; set; } = 0;
    public int ticket { get; set; } = 0;
    public int gacha { get; set; } = 0;
    public int? reward_box_id { get; set; } = null;
}

public class Data_RewardLevel : BaseData<Data_RewardLevel>
{
    public int level { get; set; } = 0;
    public int experience { get; set; } = 0;
    public string name { get; set; } = string.Empty;
    public string icon { get; set; } = string.Empty;
}

public class Data_RewardBox : BaseData<Data_RewardBox>
{

}