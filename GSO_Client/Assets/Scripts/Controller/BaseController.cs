using Google.Protobuf.Protocol;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Animator _animator;
    private PositionInfo _positionInfo = new();


    protected Rigidbody2D _rig2d;
    protected SpriteRenderer _sprite;
    private readonly StatInfo _stat = new();
    public int Id { get; set; }
    public int OwnerId { get; set; } = -1;

    public virtual StatInfo Stat
    {
        get => _stat;

        set
        {
            if (_stat.Equals(value))
                return;
            _stat.MergeFrom(value);
        }
    }

    public virtual int Hp
    {
        get => _stat.Hp;
        set => Stat.Hp = value;
    }

    public virtual int MaxHp
    {
        get => _stat.MaxHp;
        set => Stat.MaxHp = value;
    }

    float _speed = 0; //TODO : 임시 작업 서버랑 연결
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public float AttackRange
    {
        get => _stat.AttackRange;
        set => Stat.AttackRange = value;
    }

    public int CurrentPlanetId
    {
        get => _positionInfo.CurrentRoomId;
        set => _positionInfo.CurrentRoomId = value;
    }

    public PositionInfo PosInfo
    {
        get => _positionInfo;
        set
        {
            if (_positionInfo.Equals(value))
                return;

            _positionInfo = value;

            CellPos = new Vector3(value.PosX, value.PosY, 0);
            //State = value.State;
            Dir = new Vector2(value.DirX, value.DirY);
            CurrentPlanetId = value.CurrentRoomId;
            RotationZ = value.RotZ;
        }
    }

    public float RotationZ
    {
        get => PosInfo.RotZ;
        set
        {
            if (PosInfo.RotZ == value)
                return;
            PosInfo.RotZ = value;
        }
    }


    public Vector3 CellPos
    {
        get => new Vector3(PosInfo.PosX, PosInfo.PosY, 0);
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;
            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
        }
    }


    /*public virtual CreatureState State
    {
        get => PosInfo.State;
        set => PosInfo.State = value;
        //UpdateAnimation();
    }*/

    public Vector2 Dir
    {
        get => new Vector2(PosInfo.DirX, PosInfo.DirY);
        set
        {
            if (PosInfo.DirX == value.x && PosInfo.DirY == value.y)
                return;

            PosInfo.DirX = value.x;
            PosInfo.DirY = value.y;

            //UpdateAnimation();
        }
    }


    private void Start()
    {
        Init();
    }

    protected void Update()
    {
        UpdateController();
    }

    public void SyncPos(bool rotation = true)
    {
        if (float.IsNaN(CellPos.x) || float.IsNaN(CellPos.x))
            return;

        var dest = CellPos;
        transform.position = dest;
        if (rotation)
            transform.rotation = Quaternion.Euler(0, 0, RotationZ);
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _rig2d = GetComponent<Rigidbody2D>();
        //Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        //transform.position = CellPos;

      /*  if (Speed == 0)
            Speed = 3;*/

        AttackRange = 5;
        //UpdateAnimation();
    }


    protected virtual void UpdateController()
    {
        /*switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }*/
    }

    protected virtual void UpdateIdle()
    {
        //_rig2d.velocity = Vector2.zero;
        CellPos = transform.position;
        RotationZ = transform.rotation.eulerAngles.z;
    }

    public virtual void UpdateMoving()
    {
        //Debug.Log(Dir.normalized);
        //if ((CellPos - transform.position).magnitude < Speed * Time.deltaTime)
        //    return;

        //transform.Translate(Dir.normalized * Speed * Time.fixedDeltaTime, Space.Self);
        //CellPos = transform.position;
    }


    public virtual void UpdateSkill()
    {
    }

    public virtual void UpdateDead()
    {
    }


    protected int GetCurrentPlanetRotation(int id)
    {
        /*
         0 = z 0
         1 = z 90 
         2 = z 180
         3 = z 270
         */
        var type = (id >> 24) & 0x7f;
        return type;
    }
}