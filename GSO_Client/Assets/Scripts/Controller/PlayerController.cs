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
    public struct Rectangle
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        public Rectangle(float width, float height,Transform trans)
        {
            topLeft = new Vector2(trans.position.x - (width / 2), trans.position.y + (height / 2));
            topRight = new Vector2(trans.position.x + (width / 2), trans.position.y - (height / 2));
            bottomLeft = new Vector2(trans.position.x - (width / 2), trans.position.y - (height / 2));
            bottomRight = new Vector2(trans.position.x + (width / 2), trans.position.y + (height / 2));
        }
    };

    public Rectangle rect;
   //private SkillType skillType = SkillType.None; //애니메이션용

    public void SetDrawLine(float width , float height)
    {
        lineRenderer = GetComponent<LineRenderer>();
        _width = width;
        _height = height;

        rect = new Rectangle(width, height, gameObject.transform);
        // LineRenderer 설정
        lineRenderer.positionCount = 5; // 사각형을 만들기 위해 5개의 점이 필요합니다.
        lineRenderer.loop = false; // 마지막 점이 처음으로 연결되지 않도록 설정합니다.
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;


        // LineRenderer의 점 위치 설정
        lineRenderer.SetPosition(0, rect.topLeft);
        lineRenderer.SetPosition(1, rect.topRight);
        lineRenderer.SetPosition(2, rect.bottomRight);
        lineRenderer.SetPosition(3, rect.bottomLeft);
        lineRenderer.SetPosition(4, rect.topLeft);
    }


    protected void UpdateDrawLine()
    {

    }


    protected override void Init()
    {
        base.Init();
        Debug.Log("init");
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

    public void UpdatePosInfo(PositionInfo info)
    {
        Debug.Log("Update PosInfo");
        Dir = new Vector2(info.DirX, info.DirY);
        gameObject.transform.position = new Vector3(info.PosX, info.PosY, gameObject.transform.position.z);
        gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y, info.RotZ, gameObject.transform.rotation.w);
    }

    public override void UpdateMoving()
    {
        //if ((CellPos - transform.position).sqrMagnitude > 10)
        //    transform.position = CellPos;
        //else
        //    transform.position = Vector3.Lerp(transform.position, CellPos, Time.deltaTime * 10);

        Debug.Log("Update Moving");
        //Move
        Rigidbody2D rig = gameObject.GetComponent<Rigidbody2D>();
        Vector2 newVec2 = Dir * 5.0f * Time.fixedDeltaTime;
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