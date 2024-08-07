using System;
using System.Collections.Generic;
using System.Diagnostics;
using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Game.Object;

namespace Server.Game;

internal class ObjectManager
{
    //[unused(1)] [type(7)] [Id(24)]
    private int _counter = 1;
    private readonly object _lock = new();
    private readonly Dictionary<int, Player> _players = new();
    private readonly Dictionary<int, RootableObject> _rootable = new();
    private readonly Dictionary<int, ItemObject> _items = new();
    public static ObjectManager Instance { get; } = new();

    public T Add<T>() where T : GameObject, new()
    {
        var gameObjcet = new T();

        lock (_lock)
        {
            gameObjcet.Id = GenerateId(gameObjcet.ObjectType);
            if (gameObjcet.ObjectType == GameObjectType.Player) 
                _players.Add(gameObjcet.Id, gameObjcet as Player);

            else if (gameObjcet.ObjectType == GameObjectType.Item)
                _items.Add(gameObjcet.Id, gameObjcet as ItemObject);

            else if (gameObjcet.ObjectType == GameObjectType.Box)
                _rootable.Add(gameObjcet.Id, gameObjcet as RootableObject);
        }
        return gameObjcet;
    }


    public GameObject Add(GameObject obj)
    {
        lock (_lock)
        {
            obj.Id = GenerateId(obj.ObjectType);

            if (obj.ObjectType == GameObjectType.Player)
                _players.Add(obj.Id, obj as Player);
            else if (obj.ObjectType == GameObjectType.Item)
                _items.Add(obj.Id, obj as ItemObject);
            else if (obj.ObjectType == GameObjectType.Box)
                _rootable.Add(obj.Id, obj as RootableObject);
        }

        return obj;
    }





    private int GenerateId(GameObjectType type) //[unused(1)] [type(7)] [Id(24)]
    {
        lock (_lock)
        {
            return ((int)type << 24) | _counter++;
        }
    }

    public static GameObjectType GetObjectTypeById(int id)
    {
        var type = (id >> 24) & 0x7f;
        return (GameObjectType)type;
    }

    public bool Remove(int objectId)
    {
        var objectType = GetObjectTypeById(objectId);
        lock (_lock)
        {
            if (objectType == GameObjectType.Player)
                return _players.Remove(objectId);
        }

        return false;
    }

   /* public Player Find(int objectId)
    {
        var objectType = GetObjectTypeById(objectId);

        lock (_lock)
        {
            if (objectType == GameObjectType.Player)
            {
                Player player = null;
                if (_players.TryGetValue(objectId, out player))
                    return player;

            }
        }

        return null;
    }*/

    public T Find<T>(int objectId) where T : GameObject
    {
        var objectType = GetObjectTypeById(objectId);

        lock (_lock)
        {
            if (objectType == GameObjectType.Item)
            {
                ItemObject obj = null;
                if (_items.TryGetValue(objectId, out obj))
                    return obj as T;

            }
            else if (objectType == GameObjectType.Box)
            {
                RootableObject obj = null;
                if (_rootable.TryGetValue(objectId, out obj))
                    return obj as T;

            }
            else if(objectType == GameObjectType.Player)
            {
                Player obj = null;
                if (_players.TryGetValue(objectId, out obj))
                    return obj as T;

            }

            
        }

       
            return null;
    }




    public Shape[] GetValue()
    {

        List<Shape> shape = new ();

        lock (_lock)
        {
            foreach (Player p in _players.Values)
            {
                shape.Add(p.currentShape);
            }
        }

        return shape.ToArray();

    }
}