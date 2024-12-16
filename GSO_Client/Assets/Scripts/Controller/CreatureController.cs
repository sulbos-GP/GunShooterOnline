using System;
using System.Collections;
using Google.Protobuf.Protocol;
using Microsoft.AspNetCore.Connections;
using UnityEngine;

public class CreatureController : BaseController
{
    private BaseInfoBar _baseInfoBar; //hp, exp UI manage
    public Action ChangeStat;

    private Vector2 basicScale;
    private Vector2 reverseScale;

    public bool IsDead = false;

    //아래 변수들은 자식객체에서 할당을 해줘야함(크리쳐 별로 캐릭터 스프라이트나 애니메이터의 위치가 다르기때문)
    [SerializeField] protected Animator animator; 
    [SerializeField] protected SpriteRenderer characterSprite;

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
        basicScale = transform.localScale;
        reverseScale = new Vector2(-basicScale.x, basicScale.y);
        ChangeStat += CheakUpdateBar;
        AddHpbar();
        IsDead = false;
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
        var creaturePos = gameObject.transform.position;

        if (animator == null)
        {
            Debug.LogError($"{Id} : Animator is null");
        }

        if ((nextPos - transform.position).sqrMagnitude != 0)
        {
            animator.SetBool("IsMove", true);
            if (creaturePos.x < nextPos.x)
            {
                transform.localScale = reverseScale;
                Debug.Log("우");
            }
            else
            {
                transform.localScale = basicScale;
                Debug.Log("좌");
            }
        }
        else
        {
            if (nextPos == creaturePos && !animator.GetBool("IsMove"))
            {
                return;
            }
            animator.SetBool("IsMove", false);
        }
            

        //실질적인 이동
        gameObject.transform.position = nextPos;

        //이건 필요한가?
        //gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y, info.RotZ, gameObject.transform.rotation.w);
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

    public void Hit() 
    {
        if (characterSprite == null)
        {
            Debug.LogError("캐릭터의 스프라이트가 없거나 지정되지 않음");
            return;
        }
        animator.SetTrigger("IsHit");
        /*StartCoroutine(HitEffect()); */
    }

    
    public IEnumerator HitEffect()
    {
        var whiteMaterial = Resources.Load<Material>("Material/FlashWhite");
        Material matInstance = characterSprite.material;
        characterSprite.material = whiteMaterial;
        yield return new WaitForSeconds(0.1f);
        characterSprite.material = matInstance;
    }

    /*public virtual void UseSkillTodo(SkillType skillType, float Time)
    {
    }*/


    public virtual void OnDead(int attackerId = -1)
    {
        //State = CreatureState.Dead;
        if (attackerId == -1)
            return;
        IsDead = true;
        //TO - DO : 2024.10.08 수정 예정
        if (Managers.Object.MyPlayer.Hp==0)
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