using System.Collections;
using Google.Protobuf.Protocol;
using MathNet.Numerics;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    protected bool _rangedSkill = false;

    
    [SerializeField] private Material material;
    [SerializeField] private SpriteRenderer sprite;

    private Transform prevTrn;
    private float Timer = 0.0f;
    private float moveInterval = 0.5f;
    public AudioSource AudioSource;
   

    private void FixedUpdate()
    {
        Timer += Time.deltaTime;
    }

    private void moveSound()
    {
        if (Timer > moveInterval)
        {
            Timer = 0.0f;
            AudioSource.PlayOneShot(AudioSource.clip);
        }
    }

    

    public void SpawnPlayer(Vector2 vec2)
    {
        //임의 위치 값
        gameObject.transform.position = vec2;
        material = sprite?.material;
        //Spawn Particle
    }

    public void Hit() { StartCoroutine(HitEffect()); }

    public IEnumerator HitEffect()
    {
        var whiteMaterial = Resources.Load<Material>("Material/FlashWhite");
        sprite.material = whiteMaterial;
        yield return new WaitForSeconds(0.1f);
        sprite.material = material;
    }


    protected override void Init()
    {
        base.Init();
        Debug.Log("init");
        animator = transform.GetChild(1).GetComponent<Animator>();
        sprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    //protected override void UpdateAnimation()
    //{
    //	if (_animator == null || _sprite == null)
    //		return;

    //	if (State == CreatureState.Idle)
    //	{
    //		switch (Dir)
    //		{
    //			case MoveDir.Up:
    //				_animator.Play("IDLE_BACK");
    //				_sprite.flipX = false;
    //				break;
    //			case MoveDir.Down:
    //				_animator.Play("IDLE_FRONT");
    //				_sprite.flipX = false;
    //				break;
    //			case MoveDir.Left:
    //				_animator.Play("IDLE_RIGHT");
    //				_sprite.flipX = true;
    //				break;
    //			case MoveDir.Right:
    //				_animator.Play("IDLE_RIGHT");
    //				_sprite.flipX = false;
    //				break;
    //		}
    //	}
    //	else if (State == CreatureState.Moving)
    //	{
    //		switch (Dir)
    //		{
    //			case MoveDir.Up:
    //				_animator.Play("WALK_BACK");
    //				_sprite.flipX = false;
    //				break;
    //			case MoveDir.Down:
    //				_animator.Play("WALK_FRONT");
    //				_sprite.flipX = false;
    //				break;
    //			case MoveDir.Left:
    //				_animator.Play("WALK_RIGHT");
    //				_sprite.flipX = true;
    //				break;
    //			case MoveDir.Right:
    //				_animator.Play("WALK_RIGHT");
    //				_sprite.flipX = false;
    //				break;
    //		}
    //	}
    //	else if (State == CreatureState.Skill)
    //	{
    //		switch (Dir)
    //		{
    //			case MoveDir.Up:
    //				_animator.Play(_rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
    //				_sprite.flipX = false;
    //				break;
    //			case MoveDir.Down:
    //				_animator.Play(_rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
    //				_sprite.flipX = false;
    //				break;
    //			case MoveDir.Left:
    //				_animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
    //				_sprite.flipX = true;
    //				break;
    //			case MoveDir.Right:
    //				_animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
    //				_sprite.flipX = false;
    //				break;
    //		}
    //	}
    //	else
    //	{

    //	}
    //}

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    

    public override void UpdateMoving()
    {
        //플레이어 위치 변경
        //if ((CellPos - transform.position).sqrMagnitude > 10)
        //    transform.position = CellPos;
        //else
        //    transform.position = Vector3.Lerp(transform.position, CellPos, Time.deltaTime * 10);

        //Move
        Rigidbody2D rig = gameObject.GetComponent<Rigidbody2D>();
        Vector2 newVec2 = Dir * 5.0f * Time.fixedDeltaTime;
        moveSound();
        
        //rig.MovePosition(rig.position + newVec2);

        //float angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }

    protected override void UpdateIdle()
    {
    }


   /* public override void UseSkillTodo(SkillType inSkillType, float Time)
    {
        //중요
        switch (inSkillType)
        {
            case SkillType.None:

                break;
            case SkillType.Melee:
                StartCoroutine(CoStartMeleeSkill(Time));
                break;
            case SkillType.Range:
                StartCoroutine(CoStartRangeSkill(Time));
                break;
            case SkillType.Buff:
                StartCoroutine(CoStartBuffSkill(Time));
                break;
            case SkillType.Spawn:
                break;
        }
    }*/


    protected virtual void CheakUpdatedFlag(bool isForce = false)
    {
    }


   /* protected IEnumerator CoStartRangeSkill(float time)
    {
        this.skillType = SkillType.Range;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    protected IEnumerator CoStartMeleeSkill(float time)
    {
        this.skillType = SkillType.Melee;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    protected IEnumerator CoStartBuffSkill(float time)
    {
        this.skillType = SkillType.Buff;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(time);
        //State = CreatureState.Idle;
        _coSkill = null;
    }
*/

    public override void OnDamaged(Transform attacker)
    {
        base.OnDamaged(attacker);
        //Debug.Log("Player HIT !");
    }
}