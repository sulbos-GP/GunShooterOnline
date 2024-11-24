using System;
using System.Collections;
using Google.Protobuf.Protocol;
using Microsoft.AspNetCore.Connections;
using UnityEngine;

public class CreatureController : BaseController
{
    private BaseInfoBar _baseInfoBar; //hp, exp UI manage
    public Action ChangeStat;

    [SerializeField] protected Animator animator; //자식객체에서 할당을 해줘야함
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

    public override int MaxHp
    {
        get => base.MaxHp;
        set
        {
            base.MaxHp = value;
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
                Debug.Log($"updateHp {transform.name} : new hp = {Hp} ,ratio = {ratio}");
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

    public virtual void UpdatePosInfo(PositionInfo info)
    {
        //적 위치 변경
        Dir = new Vector2(info.DirX, info.DirY);
        var nextPos = new Vector3(info.PosX, info.PosY, gameObject.transform.position.z);

        if (animator == null)
        {
            Console.WriteLine("Animator is null");
            return;
        }

        if ((nextPos - transform.position).sqrMagnitude != 0)
            animator.SetBool("IsMove", true);
        else
            animator.SetBool("IsMove", false);

        if(nextPos.x - transform.position.x < 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if(nextPos.x - transform.position.x > 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }


        gameObject.transform.position = new Vector3(info.PosX, info.PosY, gameObject.transform.position.z);
        gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y, info.RotZ, gameObject.transform.rotation.w);
    }

    public virtual void OnHealed(int healAmount)
    {
        Debug.Log($"{gameObject.name}가 {healAmount}만큼 회복");
        Hp = Mathf.Min(Hp+healAmount , MaxHp);
    }

    public IEnumerator OnBuffed(Data_master_item_use consume)
    {
        float elapsedTime = 0f;
        UIManager.Instance.SetActiveHealImage(true);
        while (elapsedTime < consume.duration)
        {
            //힐 아이콘 활성화
            
            if (Hp >= MaxHp)
            {
                OnHealed(consume.energy);
            }

            yield return new WaitForSeconds(consume.active_time);

            elapsedTime += consume.active_time;
        }

        UIManager.Instance.SetActiveHealImage(false);
    }

    public virtual void OnDamaged(Transform attacker)
    {
        //var pc = attacker.GetComponent<ProjectileContoller>();
        //if (pc != null)
        //{
        //    /*var hitPacket = new C_Hit
        //    {
        //        AttackId = pc.Id,
        //        HitId = Id
        //    };
        //    Managers.Network.Send(hitPacket);*/
        //}

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
        if (attackerId == -1)
            return;
        //TO - DO : 2024.10.08 수정 예정
        if(Managers.Object.MyPlayer.Hp==0)
            UIManager.Instance.SetDieMessage(Managers.Object.FindById(attackerId).name);
        Debug.Log(transform.name + "Dead");
        //GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        //effect.transform.position = transform.position;
        //effect.GetComponent<Animator>().Play("START");
        //GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remove(Id);
        //Managers.Resource.Destroy(gameObject);
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