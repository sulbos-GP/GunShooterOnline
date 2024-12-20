using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Game.Object;
using Server.Game.Object.Attack;
using Server.Game.Object.Item;
using Server.Game.Object.Shape;

namespace Server.Game;

internal class ObjectManager
{
    //[unused(1)] [type(7)] [Id(24)]
    private int _counter = 1;
    private readonly object _lock = new();
    private readonly Dictionary<int, Player> _players = new();
    private readonly Dictionary<int, BoxObject> _rootable = new();
    private readonly Dictionary<int, ExitZone> _exit = new();
    private readonly Dictionary<int, ItemObject> _items = new();
    private readonly Dictionary<int, SpawnZone> _playerSpawn = new();
    private readonly Dictionary<int, Mine> _mines = new();
    private readonly Dictionary<int, AttackObjectBase> _attacks = new();
    private readonly Dictionary<int, BaseAI> _enemys = new();



    private readonly Dictionary<int, ScopeObject> _scopes = new();  //DetectObject.OwnerId
    public static ObjectManager Instance { get; private set; } = new();


    public void Reset()
    {
        ObjectManager.Instance = new();
    }







    public T Add<T>() where T : GameObject, new()
    {
        var gameObjcet = new T();
        gameObjcet.gameRoom = Program.gameserver.gameRoom as BattleGameRoom;
        lock (_lock)
        {
            gameObjcet.Id = GenerateId(gameObjcet.ObjectType);
            if (gameObjcet.ObjectType == GameObjectType.Player) 
                _players.Add(gameObjcet.Id, gameObjcet as Player);

            else if (gameObjcet.ObjectType == GameObjectType.Item)
                _items.Add(gameObjcet.Id, gameObjcet as ItemObject);

            else if (gameObjcet.ObjectType == GameObjectType.Box)
                _rootable.Add(gameObjcet.Id, gameObjcet as BoxObject);   
            
            else if (gameObjcet.ObjectType == GameObjectType.Exitzone)
                _exit.Add(gameObjcet.Id, gameObjcet as ExitZone);
            
            else if (gameObjcet.ObjectType == GameObjectType.Playerspawnzone)
                _playerSpawn.Add(gameObjcet.Id, gameObjcet as SpawnZone);

            else if (gameObjcet.ObjectType == GameObjectType.Mine)
                _mines.Add(gameObjcet.Id, gameObjcet as Mine);

            else if (gameObjcet.ObjectType == GameObjectType.Attack)
                _attacks.Add(gameObjcet.Id, gameObjcet as AttackObjectBase);

            else if (gameObjcet.ObjectType == GameObjectType.Enemyai)
            {
                BaseAI enemyAI = gameObjcet as BaseAI;
                _enemys.Add(enemyAI.Id, enemyAI);
                _scopes.Add(enemyAI.Id, enemyAI.DetectObject);
            }


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
                _rootable.Add(obj.Id, obj as BoxObject);
            else if (obj.ObjectType == GameObjectType.Exitzone)
                _exit.Add(obj.Id, obj as ExitZone); 
            else if (obj.ObjectType == GameObjectType.Playerspawnzone)
                _playerSpawn.Add(obj.Id, obj as SpawnZone);
            else if (obj.ObjectType == GameObjectType.Mine)
                _mines.Add(obj.Id, obj as Mine);
            else if (obj.ObjectType == GameObjectType.Attack)
                _attacks.Add(obj.Id, obj as AttackObjectBase);
            else if (obj.ObjectType == GameObjectType.Enemyai)
            {
                BaseAI enemyAI = obj as BaseAI;
                _enemys.Add(enemyAI.Id, enemyAI);
                _scopes.Add(enemyAI.Id, enemyAI.DetectObject);
            }

        }

        return obj;
    }


    public void DebugObjectDics()
    {
        Console.WriteLine($"player : ");
        foreach (Player player in _players.Values){
            Console.WriteLine($"{player.Id} ");
        }
        Console.WriteLine();
        Console.WriteLine($"rootable : ");
        foreach (BoxObject root in _rootable.Values)
        {
            Console.Write($"{root.Id}, ");
        }
        Console.WriteLine();
        Console.WriteLine($"Item : ");
        foreach (ItemObject item in _items.Values)
        {
            Console.Write($"{item.Id}, ");
        }

        Console.WriteLine();
        Console.WriteLine($"exitZone : ");
        foreach (ExitZone exit in _exit.Values)
        {
            Console.Write($"{exit.Id}, ");
        }

        Console.WriteLine();
        Console.WriteLine($"spawnZone : ");
        foreach (SpawnZone pawn in _playerSpawn.Values)
        {
            Console.Write($"{pawn.Id} ,");
        }
        Console.WriteLine();

        Console.WriteLine($"mine : ");
        foreach (Mine mine in _mines.Values)
        {
            Console.Write($"{mine.Id} ,");
        
        }
        Console.WriteLine();

        Console.WriteLine($"Enemy : ");
        foreach (BaseAI enemy in _enemys.Values)
        {
            Console.Write($"{enemy.Id} ,");
        }
        Console.WriteLine();

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

    /// <summary>
    /// 최대한 LeaveGame 사용 할 것
    /// </summary>
    /// <param name="objectId"></param>
    /// <returns></returns>
    public bool Remove(int objectId)
    {
        var objectType = GetObjectTypeById(objectId);
        lock (_lock)
        {
            if (objectType == GameObjectType.Player)
                return _players.Remove(objectId);
            else if (objectType == GameObjectType.Item)
                return _items.Remove(objectId);
            else if (objectType == GameObjectType.Box)
                return _rootable.Remove(objectId);
            else if (objectType == GameObjectType.Exitzone)
                return _exit.Remove(objectId);
            else if (objectType == GameObjectType.Mine)
                return _mines.Remove(objectId);
            else if (objectType == GameObjectType.Attack)
                return _attacks.Remove(objectId);
            else if (objectType == GameObjectType.Enemyai)
            {
                bool t = _enemys.Remove(objectId);
                t &= _scopes.Remove(objectId);
                return t;
            }

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
                BoxObject obj = null;
                if (_rootable.TryGetValue(objectId, out obj))
                    return obj as T;

            }
            else if(objectType == GameObjectType.Player)
            {
                Player obj = null;
                if (_players.TryGetValue(objectId, out obj))
                    return obj as T;

            }
            else if (objectType == GameObjectType.Exitzone)
            {
                ExitZone obj = null;
                if (_exit.TryGetValue(objectId, out obj))
                    return obj as T;

            }
            else if (objectType == GameObjectType.Mine)
            {
                Mine obj = null;
                if (_mines.TryGetValue(objectId, out obj))
                    return obj as T;

            }
            else if (objectType == GameObjectType.Attack)
            {
                AttackObjectBase obj = null;
                if (_attacks.TryGetValue(objectId, out obj))
                    return obj as T;

            }
            else if (objectType == GameObjectType.Enemyai)
            {
                BaseAI obj = null;
                if (_enemys.TryGetValue(objectId, out obj))
                    return obj as T;

            }
            else
            {
                Console.WriteLine("Type is None");
            }

        }

       
            return null;
    }

    public BaseAI[] GetEnemyAIs()
    {
        return _enemys.Values.ToArray();
    }
    public GameObject[] GetAllShapes()
    {
        return _enemys.Values.ToArray<GameObject>().Concat(_players.Values).Concat(_mines.Values).Concat(_scopes.Values).Concat(_attacks.Values).ToArray();
    }



    public Shape[] GetShapeValue()
    {

        List<Shape> shape = new ();

        //The box doesn't use a lock because it doesn't allow for dynamic generation

        foreach (BoxObject r in _rootable.Values)
        {
            shape.Add(r.currentShape);
        }

        lock (_lock)
        {
            foreach (Player p in _players.Values)
            {
                shape.Add(p.currentShape);
            }
            foreach (BaseAI p in _enemys.Values)
            {
                shape.Add(p.currentShape);
            }
        }

        return shape.ToArray();

    }

    public void Update()
    {
        foreach (Player p in _players.Values)
        {
            p.Update();
        }
        foreach (BaseAI e in _enemys.Values)
        {
            e.Update();
        }
        foreach (ScopeObject s in _scopes.Values)
        {
            s.Update();
        }
        
    }


    private void CheakPlayerEixt()
    {

    }
}