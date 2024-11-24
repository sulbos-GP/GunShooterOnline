using System.Collections;
using Google.Protobuf.Protocol;
using MathNet.Numerics;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    protected bool _rangedSkill = false;

    protected float _width;
    protected float _height;

    public LineRenderer lineRenderer;

    [SerializeField] private Material material;
    [SerializeField] private SpriteRenderer sprite;

    private Transform prevTrn;
    private float Timer = 0.0f;
    private float moveInterval = 0.5f;
    public AudioSource AudioSource;
    public struct Rectangle
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        public Rectangle(float width, float height)
        {
            topLeft = new Vector2(-width / 2, -height / 2);
            topRight = new Vector2(+width / 2, -height / 2);
            bottomLeft = new Vector2(-width / 2, +height / 2);
            bottomRight = new Vector2(+width / 2, +height / 2);
        }
    };

    public Rectangle rect;
   //private SkillType skillType = SkillType.None; //애니메이션용

    public void SetDrawLine(float width , float height)
    {
        //TO-DO : 임시
        
        _width = width;
        _height = height;

        rect = new Rectangle(width, height);
        // LineRenderer 설정
        lineRenderer.positionCount = 5; // 사각형을 만들기 위해 5개의 점이 필요합니다.
        lineRenderer.loop = false; // 마지막 점이 처음으로 연결되지 않도록 설정합니다.
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        // LineRenderer의 점 위치 설정
        UpdateDrawLine();
    }

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

    public void UpdateDrawLine()
    {
        return;
        lineRenderer.SetPosition(0, (Vector2)gameObject.transform.position + rect.topLeft * 2);
        lineRenderer.SetPosition(1, (Vector2)gameObject.transform.position + rect.topRight * 2);
        lineRenderer.SetPosition(2, (Vector2)gameObject.transform.position + rect.bottomRight * 2);
        lineRenderer.SetPosition(3, (Vector2)gameObject.transform.position + rect.bottomLeft * 2);
        lineRenderer.SetPosition(4, (Vector2)gameObject.transform.position + rect.topLeft * 2);
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
        
        lineRenderer = GetComponent<LineRenderer>();
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
        UpdateDrawLine();
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