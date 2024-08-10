using System;
using Google.Protobuf.Protocol;
using UnityEngine;

public class CreatureController : BaseController
{
    private BaseInfoBar _baseInfoBar; //hp, exp UI manage
    public Action ChangeStat;


    public override StatInfo Stat
    {
        get => base.Stat;
        set
        {
            base.Stat = value;
            ChangeStat?.Invoke();
        }
    }

    public override int Hp
    {
        get => base.Hp;
        set
        {
            base.Hp = value;
            ChangeStat?.Invoke();
        }
    }

    /*
    public int Exp
    {
        get
        {
            return Stat.Exp;
        }
        set
        {
            Stat.Exp = value;
            ChangeStat?.Invoke();
        }
    }*/
    
    protected override void Init()
    {
        base.Init();
        MyPlayerController mc = this as MyPlayerController;
        ChangeStat += CheakUpdateBar;
        AddHpbar();

    }

    protected void AddHpbar()
    {
        var go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 0.58f, 0);
        go.name = "HpBar";
        _baseInfoBar = go.GetComponentInChildren<BaseInfoBar>();

        ChangeStat?.Invoke();
    }


    public override void UpdateMoving()
    {
        //StartCoroutine(Move());
    }

    //IEnumerator Move()
    //{
    //    float dis = Vector2.Distance(CellPos, transform.position);
    //    Debug.Log(dis);
    //    transform.Translate(Speed * Time.deltaTime * (CellPos - transform.position).normalized, Space.World);
    //    yield return null;
    //}

    

    public void CheakUpdateBar()
    {
        if (_baseInfoBar == null)
        {
            Invoke(nameof(CheakUpdateBar), 0.01f);
            return;
        }

        #region Hp
        {
            var ratio = 0f;
            if (Stat.MaxHp > 0)
            {
                ratio = Mathf.Max(0, (float)Hp / Stat.MaxHp);
                //Debug.Log($"updateHp {transform.name} : new hp = {Hp} ,ratio = {ratio}");
            }
            _baseInfoBar.SetHpBar(ratio);
        }
        #endregion
        
        #region Exp
        {
            var ratio = 0f;
            
            //ratio = Mathf.Max(0, (float)Exp / Stat.MaxExp);
            //Debug.Log($"updateHp {transform.name} : new Exp = {Exp} ,ratio = {ratio}");
            
            _baseInfoBar.SetExpBar(ratio);
        }

        #endregion

    }

    public virtual void OnDamaged(Transform attacker)
    {
        var pc = attacker.GetComponent<ProjectileContoller>();
        if (pc != null)
        {
            /*var hitPacket = new C_Hit
            {
                AttackId = pc.Id,
                HitId = Id
            };
            Managers.Network.Send(hitPacket);*/
        }

        var cc = attacker.GetComponent<CreatureController>();
        if (cc != null)
        {
        }
    }


    /*public virtual void UseSkillTodo(SkillType skillType, float Time)
    {
    }*/


    public virtual void OnDead(int attackerId = -1)
    {
        //State = CreatureState.Dead;

        Debug.Log(transform.name + "Dead");
        //GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        //effect.transform.position = transform.position;
        //effect.GetComponent<Animator>().Play("START");
        //GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remove(Id);
        Managers.Resource.Destroy(gameObject);
    }

    [ContextMenu("GetInfo")]
    public void GetInfo()
    {
        Debug.Log("Id = " + Id);

        Debug.Log("PosInfo = " + PosInfo);
        Debug.Log(" PosInfo.CurrentRoomId = " + PosInfo.CurrentRoomId);
        Debug.Log("Stat = " + Stat);
        Debug.Log("Hp = " + Hp);
        Debug.Log("OwnerId = " + OwnerId);
    }
}