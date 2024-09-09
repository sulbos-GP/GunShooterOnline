using System.Collections.Generic;
using UnityEngine;

public abstract class BaseData<T> where T : BaseData<T>, new()
{
    //TO-DO : Key Code
    public int Key;

    public static List<T> DataList { get { return _list; } }

    private static Dictionary<int, T> _dic = new Dictionary<int, T>();
    private static List<T> _list;

    public static List<KeyValuePair<int,T>> AllData()
    {
        List<KeyValuePair<int,T>> dicList = new List<KeyValuePair<int,T>>(_dic);
        return dicList;
    }

    // List �б�.
    public static void ReadList(List<T> list)
    {
        if (list == null)
            return;

        _list = list;
        _dic.Clear();

        int count = _list.Count;
        for (int i = 0; i < count; i++)
        {
            if (_dic.ContainsKey(_list[i].Key))
                Debug.LogError(typeof(T) + " �ߺ�: " + _list[i].Key);
            _dic[_list[i].Key] = _list[i];
        }
    }

    // List �߰�.
    public static void AddList(List<T> list)
    {
        if (list == null)
            return;

        if (_list == null)
            _list = new List<T>();

        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            if (_dic.ContainsKey(list[i].Key))
            {
                _dic[list[i].Key] = list[i];
                continue;
            }

            _dic[list[i].Key] = list[i];
            _list.Add(list[i]);
        }
    }

    // ������ ���� ����.
    public static bool HasData(int key)
    {
        return _dic.ContainsKey(key);
    }

    // ������ ���.
    public static T GetData(int key)
    {
        if (_dic.ContainsKey(key))
            return _dic[key];

        Debug.LogError(typeof(T) + " ������:" + key);
        return null;
    }

    //����
    public static void Clear()
    {
        _dic.Clear();
        _list.Clear();
    }
}

public abstract class BaseDataMulti<T> where T : BaseDataMulti<T>, new()
{
    public int Key;

    public static int Count => _list.Count;
    public static Dictionary<int, List<T>> Dictionary => _dic;
    public static List<T> DataList => _list;

    private static Dictionary<int, List<T>> _dic = new Dictionary<int, List<T>>();
    private static List<T> _list;

    // List �б�.
    public static void ReadList(List<T> list)
    {
        if (list == null)
            return;

        _list = list;
        _dic.Clear();

        int count = _list.Count;
        for (int i = 0; i < count; i++)
        {
            if (_dic.ContainsKey(_list[i].Key))
                _dic[_list[i].Key].Add(_list[i]);
            else
                _dic.Add(_list[i].Key, new List<T>() { _list[i] });
        }
    }

    // ������ ���.
    public static List<T> GetList(int key)
    {
        if (_dic.ContainsKey(key))
            return _dic[key];
        Debug.LogError(typeof(T) + " ������:" + key);
        return null;
    }

    // ����.
    public static void Clear()
    {
        _dic.Clear();
        _list.Clear();
    }
}
