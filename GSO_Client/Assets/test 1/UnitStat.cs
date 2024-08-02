using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStat
{
    private Dictionary<eStat,int> _statDic = new Dictionary<eStat,int>();


    //TO-DO : 사용 스텟 많아질 경우 추후에 고민해야 함.
    public void Init()
    {
        _statDic.Add(eStat.Health, 100);
        _statDic.Add(eStat.Armor, 0);
    }
    
    public bool Contains(eStat stat)
    {
        return _statDic.ContainsKey(stat); 
    }

    public void Add(eStat stat,int value)
    {
        if(Contains(stat))
            _statDic[stat] += value;
        else
            _statDic[stat] = value;
    }

    public void Remove(eStat stat,int value)
    {
        if( Contains(stat))
            _statDic[stat]-=value;
        else
            _statDic.Remove(stat);
    }    

    public int GetStat(eStat stat)
    {   
        return _statDic[stat]; 
    }

    public void SetStat(eStat stat,int value)
    {
        int prev = 0;
        if(Contains(stat))
            prev = _statDic[stat];
        _statDic[stat]=value;
        Debug.LogWarning("Prev Stat : "+prev +" Current Stat : "+GetStat(stat));
    }


}
