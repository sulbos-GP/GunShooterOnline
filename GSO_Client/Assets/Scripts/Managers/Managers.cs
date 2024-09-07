using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Server.Data;
using UnityEngine;

internal class Managers : MonoBehaviour
{
    private static Managers s_instance = new();

 

    private readonly List<Tuple<Action, short>> _actions = new();
    private readonly object _lock = new();

    public static Managers Instance
    {
        get
        {
            Init();
            return s_instance;
        }
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        _network.Update(); //네트워크

        if (_actions.Count > 0)
            lock (_lock)
            {
                foreach (var t in _actions)
                    StartCoroutine(CoStart(t.Item1, t.Item2));
                _actions.Clear();
            }
    }

    public void Add(Tuple<Action, short> tuple)
    {
        lock (_lock)
        {
            _actions.Add(tuple);
        }
    }

    private IEnumerator CoStart(Action action, short number)
    {
        Skill skill;
        if (DataManager.SkillDict.TryGetValue(number, out skill))
        {
            yield return new WaitForSeconds(skill.cooldown);
            action.Invoke();
        }
    }

    public static void Init()
    {
        if (s_instance == null)
        {
            var go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._environment.InitEnviromentSetting(EEnvironmentState.Release);

            //s_instance._data.InventorySet();

            s_instance._pool.Init();
            //s_instance._Data.Init();
            //s_instance._sound.InventorySet();

            ExcelReader.ReadExcel();
        }
    }

    private void OnApplicationQuit()
    {
        Clear();
    }

    public static void Clear()
    {
        //Sound.Clear();
        Scene.Clear();
        //UI.Clear();
        Pool.Clear();
        Network.Clear();
    }

    #region Config

    private readonly EnvironmentSetting _environment = new();

    public static EnvironmentSetting EnvConfig => Instance._environment;

    #endregion

    #region Contents

    private readonly NetworkManager _network = new();
    private readonly WebManager _web = new();
    private readonly MapManager _map = new();
    //private readonly DataManager _Data = new();
    private readonly SkillManager _skill = new();

    public static NetworkManager Network => Instance._network;
    public static WebManager Web => Instance._web;
    public static MapManager Map => Instance._map;
    //public static DataManager Data => Instance._Data;
    public static SkillManager Skill => Instance._skill;

    #endregion


    #region Core

    private readonly PoolManager _pool = new();
    private readonly SceneManagerEx _scene = new();
    private readonly ResourceManager _resource = new();
    private readonly ObjectManager _obj = new();
    public static ObjectManager Object => Instance._obj;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;


    #endregion

    #region Logger

    //private readonly SystemLogManager _systemLog = new();

    //public static SystemLogManager SystemLog => Instance._systemLog;

    #endregion
}