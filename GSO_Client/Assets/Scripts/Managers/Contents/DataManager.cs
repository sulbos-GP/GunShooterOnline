using System.Collections.Generic;
using Google.Protobuf.Protocol;

using Server.Data;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}


public class DataManager
{
    public enum CharacterClass   //class * 100 + (1~5) = skill
    {
        HolyKnight = 1,
        wizard = 2,
        BlackWizard = 3    
    }
    public static Dictionary<int, StatInfo> StatDict { get; private set; } = new();

    public static Dictionary<int, Skill> SkillDict { get; private set; } = new();

    //public static Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();
    public static Dictionary<int, MonsterData> MonsterDict { get; private set; } = new();

    public void Init()
    {
        LoadData();
    }


    public static void LoadData()
    {
        /*StatDict = LoadJson<StatData, int, StatInfo>("StatData").MakeDict();
        SkillDict = LoadJson<SkillData, int, Skill>("skillData").MakeDict();
        //ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData").MakeDict();
        MonsterDict = LoadJson<MonsterLoader, int, MonsterData>("MonsterData").MakeDict();*/
    }
    /*
    private static Loader LoadJson<Loader, Key, Value>(string name) where Loader : ILoader<Key, Value>
    {
        var t = Resources.Load($"Data/{name}").ToString();
        //string text = File.ReadAllText($"{Resources.Load($"Data/{name}")}");
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        return JsonConvert.DeserializeObject<Loader>(t, settings);
    }*/
}