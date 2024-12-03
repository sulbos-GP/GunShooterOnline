using System.Collections;
using Google.Protobuf.Protocol;
using MathNet.Numerics;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    protected bool _rangedSkill = false;

    private Transform prevTrn;
    private float Timer = 0.0f;
    private float moveInterval = 0.5f;
    public AudioSource AudioSource;
   

    private void FixedUpdate()
    {
        Timer += Time.deltaTime;
    }
    

    protected override void Init()
    {
        base.Init();
        Debug.Log("init");
        animator = transform.GetChild(1).GetComponent<Animator>();
        characterSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

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
        AudioSource.Play();

        //rig.MovePosition(rig.position + newVec2);

        //float angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }

    protected override void UpdateIdle()
    {
        AudioSource.Stop();
    }



    protected virtual void CheakUpdatedFlag(bool isForce = false)
    {
    }


    public override void OnDamaged(Transform attacker)
    {
        base.OnDamaged(attacker);
        //Debug.Log("Player HIT !");
    }
}