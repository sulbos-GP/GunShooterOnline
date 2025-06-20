using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTABLE_TYPE
{
    None,
    TestItem,
    Item,

    master_item_base,
    master_item_backpack,
    master_item_use,
    master_item_weapon,

    master_reward_base,
    master_reward_box,
    master_reward_level,
    master_reward_box_item,

    master_quest_base,
}

public enum eSTAT
{
    None,
    Health,
    Armor
}

public enum eITEM_TYPE
{
    None,
    Weapone,
    Defensive,
    Bag,
    Recovery,
    Bullet,
    Spoil
}
public enum GunState
{
    Shootable,
    Empty,
    Reloading
}

public enum GunType
{
    Pistol,
    AssultRifle,
    ShotGun,
    Sniper
}

public enum BulletType
{
    b729mm,
    b559mm
}

public enum EEffect
{
    None,
    immediate,
    buff,
}
