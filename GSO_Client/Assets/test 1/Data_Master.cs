using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Data_master_item_use : BaseData<Data_master_item_use>
{
    public int energy;
    public float active_time;
    public float duration;
    public EEffect effect; //EEffect
    public float cool_time;
}

public class Data_master_item_weapon : BaseData<Data_master_item_weapon>
{
    public int attack_range;
    public int damage;
    public int distance;
    public int reload_round;
    public float attack_speed;
    public int reload_time;
    public string bullet;
}

public class Data_master_reward_base : BaseData<Data_master_reward_base>
{
    public int money;
    public int ticket;
    public int gacha;
    public int experience;
    //TO-DO : 추후에 타입 변경 가능성 있음.
    public int reward_box_id;
}

public class Data_master_reward_level : BaseData<Data_master_reward_level>
{
    public int level;
    public string name;
    public string icon;
}

public class Data_master_reward_box : BaseDataMulti<Data_master_reward_box>
{
    public int box_scale_x;
    public int box_scale_y;
}

public class Data_master_reward_box_item : BaseData<Data_master_reward_box_item>
{
    public int reward_box_id;
    public string item_code;
    public int x;
    public int y;
    public int rotation;
    public int amount;
}
