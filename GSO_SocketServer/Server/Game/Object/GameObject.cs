using System;
using System.Diagnostics;
using System.Numerics;
using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game.Object;
using ServerCore;

namespace Server.Game;

public class GameObject 
{

    public GameObject()
    {
        info.PositionInfo = PosInfo;
    }

    public GameObjectType ObjectType { get; protected set; } = GameObjectType.Noneobject;
    public ObjectInfo info { get; set; } = new() { OwnerId = -1, Shape = new ShapeInfo() };


    private Shape _shape;
    public Shape currentShape {
        get
        {
            return _shape;
        }
        set { 
            _shape = value;
            info.Shape.ShpapeType = (Google.Protobuf.Protocol.ShapeType)value.Type;
            info.Shape.CenterPosX      = value.position.x;
            info.Shape.CenterPosY      = value.position.y;
            info.Shape.Roatation = value.rotation;
          
            if (value.Type == Collision.Shapes.ShapeType.CIRCLE)
            {
                info.Shape.Radius = ((Circle)value).radius;
            }
            else if(value.Type == Collision.Shapes.ShapeType.RECTANGLE)
            {
                info.Shape.Left = ((Rectangle)value).Left;
                info.Shape.Bottom = ((Rectangle)value).Bottom;
                info.Shape.Width = ((Rectangle)value).Width;
                info.Shape.Height = ((Rectangle)value).Height;
            }
            else if(value.Type == Collision.Shapes.ShapeType.POLYGON)
            {
                Console.WriteLine("Polygon Error");
            }
            else if(value.Type == Collision.Shapes.ShapeType.ARCPOLY)
            {
                //TODO : 
            }

        }
    } // 충돌때만 하면 

    public PositionInfo PosInfo { get; } = new();

    public int UID
    {
        get => info.Uid;
        set => info.Uid = value;
    }


    public Vector2 Dir
    {
        get => new(PosInfo.DirX, PosInfo.DirY);
        set
        {
            PosInfo.DirX = value.X;
            PosInfo.DirY = value.Y;
        }
    }

    public int Id
    {
        get => info.ObjectId;
        set => info.ObjectId = value;
    }

    public int OwnerId //중요 : 초기값 -1
    {
        get => info.OwnerId;
        set => info.OwnerId = value;
    }


    public BattleGameRoom gameRoom { get; set; }


    //public virtual int TotalAttack { get { return stat.Attack; } }
    //public virtual int TotalDefence { get { return 0; } }


    //public int Exp => stat.Exp;



    /*public CreatureState State
    {
        get => PosInfo.State;
        set => PosInfo.State = value;
    }*/

    /*
    public int Attack
    {
        get => stat.Attack;
        set => stat.Attack = value;
    }*/

    /*public float Speed
    {
        get => stat.Speed;
        set => stat.Speed = value;
    }*/


    /// <summary>
    /// Set는 Init에 가장 마지막으로
    /// </summary>
    public Vector2 CellPos
    {
        get => new(PosInfo.PosX, PosInfo.PosY);
        set
        {
            info.PositionInfo.PosX = value.X;
            info.PositionInfo.PosY = value.Y;

            if (currentShape != null)
            {
                currentShape.x = value.X;
                currentShape.y = value.Y;
            }
            else
            {
                Console.WriteLine("currentShape is null");

            }
        }
    }

    public virtual void Update()
    {
    }

    /*  public Vector2 GetFrontCellPos()
      {
          return GetFrontCellPos(new Vector2(Dir.X, Dir.Y));
      }

      public Vector2 GetFrontCellPos(Vector2 dir)
      {
          var cellPos = CellPos;

          //Todo : dir크기를 재서 이상있으면 지우기
          cellPos += dir;

          return cellPos;
      }*/


    public virtual void OnDamaged(GameObject attacker, int damage)
    {
        Console.WriteLine(info.Name + "은" + attacker.info.Name + "에게 데미지 받음");
    }

    public virtual void OnHealed(GameObject healer, int heal)
    {
        Console.WriteLine(info.Name + "은" + healer.info.Name + "에게 힐 받음");
    }

    public virtual void OnCollision(GameObject other)
    {
       
        //상속받아 알아서 행동 ex) 스킬이면 데미지 사람이라면 충동처리
        //Console.Write(info.Name + "은" + other.info.Name + " 과 충돌함");
    }

    /// <summary>
    ///  아직 사용금지
    /// </summary>
    /// <param name="others"></param>
    public virtual void OnCollisionList(GameObject[] others)
    {

    }

    /// <summary>
    /// (일단 사용 중지)충돌한 객체에서 정보를 받음
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnCollisionFeedback(GameObject other)
    {
        if (other.ObjectType == GameObjectType.Projectile)
        {
            //OnDamaged(other,other.Attack);
            other.OnCollisionFeedback(this);
        }
        /* else if (other.ObjectType == GameObjectType.Scopeskill)
         {
             //할일 TODO : 장판 데이미지 일정량만 받게(일시 무적)
             OnDamaged(other,other.Attack);
             other.OnCollisionFeedback(this);
         }*/

    }


    public virtual GameObject GetOwner()
    {
        return this;
    }
} // class

//public struct Vector2
//{
//    public float x;
//    public float y;

//    public Vector2(float x, float y) { this.x = x; this.y = y; }

//    public static Vector2 up { get { return new Vector2(0, 1); } }
//    public static Vector2 down { get { return new Vector2(0, -1); } }
//    public static Vector2 left { get { return new Vector2(-1, 0); } }
//    public static Vector2 right { get { return new Vector2(1, 0); } }

//    public static Vector2 operator +(Vector2 a, Vector2 b)
//    {
//        return new Vector2(a.x + b.x, a.y + b.y);
//    }
//    public static Vector2 operator -(Vector2 a, Vector2 b)
//    {
//        return new Vector2(a.x - b.x, a.y - b.y);
//    }

//    public float magnitude { get { return (float)MathF.Sqrt(sqrMagnitude); } }
//    public float sqrMagnitude { get { return (x * x + y * y); } }

//    //public int cellDistFromZero { get { return Math.Abs(x) + Math.Abs(y); } }
//}