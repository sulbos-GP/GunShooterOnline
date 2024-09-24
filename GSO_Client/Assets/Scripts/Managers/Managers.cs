using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Server.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebCommonLibrary.Models.MasterDatabase;

internal class Managers : MonoBehaviour
{
    private static Managers s_instance;

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

        if(s_instance == null)
        {
            //씬 로드 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

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

            //s_instance._version.SetAppVersion(Application.version);
            s_instance._environment.InitEnviromentSetting(EEnvironmentState.Emulator);

            //s_instance._data.InventorySet();

            s_instance._pool.Init();
            //s_instance._Data.Init();
            //s_instance._sound.InventorySet();

            ExcelReader.ReadExcel();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SystemLog.CreateSystemWindow();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SystemLog.DestroySystemWindow();
    }

    /// <summary>
    /// Application 처음 시작시 해주고 싶은거 있으면 여기
    /// </summary>
    private void OnApplicationFocus(bool focus)
    {
        // Application 처음 시작시 (True)
        // 이탈 False, 복귀 True
    }

    /// <summary>
    /// Application 중간에 이탈 또는 복귀 여부
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        //이탈 True, 복귀 False

        if (Web != null)
        {
            Web.OnPuase(pause);
        }

    }

    /// <summary>
    /// Application이 종료되었을 경우
    /// </summary>
    private void OnApplicationQuit()
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

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
    private readonly VersionConfig _version = new();

    public static EnvironmentSetting EnvConfig => Instance._environment;
    public static VersionConfig VersionConfig => Instance._version;
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

    private readonly SystemLogManager _systemLog = new();

    public static SystemLogManager SystemLog => Instance._systemLog;

    #endregion
}