using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Data
{
    #region Stat

    [Serializable]
    public class StatData : ILoader<int, StatInfo>
    {
        public List<StatInfo> stats = new();

        public Dictionary<int, StatInfo> MakeDict()
        {
            var dict = new Dictionary<int, StatInfo>();
            foreach (var stat in stats)
            {
                stat.Hp = stat.MaxHp;
                stat.Mp = stat.MaxMp;
                dict.Add(stat.Class * 100 + stat.Level, stat);
                //101 = 성기사 1레벨
            }

            return dict;
        }
    }

    #endregion

    #region Skill

    [Serializable]
    public class Skill
    {
        public int id;
        public int maxLevel;
        public string name;
        public float level_add;
        public string description;
        public float castTime;
        public float duration;
        public float cooldown;
        public int skilltype;
        public int cost;
        public int amount;
        public float attackbuff; //퍼센트
        public float defencebuff;
        public int damage;
        public int hp;
        public Creature creature;
        public ProjectileInfo projectile;
    }

    public class Creature
    {
        public int attack;
        public string attackRange; //1이면 근접
        public float attackSpeed; //초당 공격속도
        public int defence;
        public int exp;
        public int hp;
        public int mp;
        public string name;
        public float speed;
    }

    public class ProjectileInfo
    {
        public string name;
        public string prefab;
        public int range;
        public float speed;
    }


    [Serializable]
    public class SkillData : ILoader<int, Skill>
    {
        public List<Skill> skills = new();

        public Dictionary<int, Skill> MakeDict()
        {
            var dict = new Dictionary<int, Skill>();
            foreach (var skill in skills)
                dict.Add(skill.id, skill);
            return dict;
        }
    }

    #endregion


    //#region Item
    //[Serializable]
    //public class ItemData
    //{
    //	public int id;
    //	public string name;
    //	public ItemType itemType;

    //}

    //public class WeaponData : ItemData
    //{
    //	public WeaponType weaponType;
    //	public int damage;


    //}

    //public class ArmorData : ItemData
    //{
    //	public ArmorType armorType;
    //	public int defence;
    //}

    //public class ConsumableData : ItemData
    //{
    //	public ConsumableType consumableType;
    //	public int MaxCount;
    //}

    //[Serializable]
    //public class ItemLoader : ILoader<int, ItemData>
    //{
    //	public List<WeaponData> weapons = new List<WeaponData>();
    //	public List<ArmorData> armors = new List<ArmorData>();
    //	public List<ConsumableData> consumables = new List<ConsumableData>();

    //	public Dictionary<int, ItemData> MakeDict()
    //	{


    //		Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

    //		foreach (ItemData item in weapons)
    //		{
    //			item.itemType = ItemType.Weapon;
    //			dict.Add(item.id, item);
    //		}
    //		foreach (ItemData item in armors)
    //		{
    //			item.itemType = ItemType.Armor;
    //			dict.Add(item.id, item);
    //		}
    //		foreach (ItemData item in consumables)
    //		{
    //			item.itemType = ItemType.Consumable;
    //			dict.Add(item.id, item);
    //		}

    //		return dict;
    //	}
    //}

    //#endregion

    #region Mosnter

    [Serializable]
    public class RewardData
    {
        public int probability; // 100분율
        public int itemId;
        public int count;
    }


    [Serializable]
    public class MonsterData
    {
        public int id;
        public string name;
        public List<RewardData> rewards;

        public StatInfo stat;
        //public string prefabPath;
    }

    [Serializable]
    public class MonsterLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new();

        public Dictionary<int, MonsterData> MakeDict()
        {
            var dict = new Dictionary<int, MonsterData>();
            foreach (var monster in monsters) dict.Add(monster.id, monster);
            return dict;
        }
    }

    #endregion
}